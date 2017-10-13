using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VSTSCore.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace VSTSCore
{
    public class VSTSConnector
    {
        private string Account;
        private string AccessToken;
        private string Username;
        private string apiversion = "1.0";
        public VSTSConnector(string account, string username, string accesstoken)
        {
            this.AccessToken = accesstoken;
            this.Account = account;
            this.Username = username;
        }

        #region Http methods
        public string Get(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{this.Username}:{this.AccessToken}")));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync($"https://{this.Account}.visualstudio.com/{url}").Result;
                string data = response.Content.ReadAsStringAsync().Result;
                return data;
            }
        }
        public string Post(string url, string postData)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{this.Username}:{this.AccessToken}")));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://{this.Account}.visualstudio.com/{url}");
                request.Content = new StringContent(postData,Encoding.UTF8, "application/json");
                var response = client.SendAsync(request).Result;
                string data = response.Content.ReadAsStringAsync().Result;
                return data;
            }
        }

        public string Patch(string url, string patchData)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{this.Username}:{this.AccessToken}")));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), $"https://{this.Account}.visualstudio.com/{url}");
                request.Content = new StringContent(patchData,Encoding.UTF8, "application/json-patch+json");
                var response = client.SendAsync(request).Result;
                string data = response.Content.ReadAsStringAsync().Result;
                return data;
            }
        }

        public string Delete(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{this.Username}:{this.AccessToken}")));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"https://{this.Account}.visualstudio.com/{url}");
                var response = client.SendAsync(request).Result;
                string data = response.Content.ReadAsStringAsync().Result;
                return data;
            }
        }
        #endregion

        #region Projects
        public List<Project> GetAllProjects()
        {
            return JsonConvert.DeserializeObject<VSTSResponse<Project>>(this.Get($"/DefaultCollection/_apis/projects?api-version={this.apiversion}")).value;
        }

        public Project GetProject(Guid projectId)
        {
            return JsonConvert.DeserializeObject<Project>(this.Get($"/DefaultCollection/_apis/projects/{projectId.ToString()}?api-version={this.apiversion}"));
        }
        #endregion

        #region Work Items
        public List<Query> GetListOfQueries(Guid projectId)
        {
            return JsonConvert.DeserializeObject<VSTSResponse<Query>>(this.Get($"DefaultCollection/{projectId.ToString()}/_apis/wit/queries/?api-version={this.apiversion}")).value;
        }
        public List<Query> GetQueryFolderContents(Guid projectId, string folder)
        {
            Query queryFolder = JsonConvert.DeserializeObject<Query>(this.Get($"DefaultCollection/{projectId.ToString()}/_apis/wit/queries/{folder}/?api-version={this.apiversion}&$depth=1"));
            return queryFolder.children;
        }
        public WIQLResult WorkItemQuery(Guid projectId, string workItemQuery)
        {
            object postData = new { query = workItemQuery };
            return JsonConvert.DeserializeObject<WIQLResult>(this.Post($"/DefaultCollection/{projectId.ToString()}/_apis/wit/wiql?api-version={this.apiversion}", JsonConvert.SerializeObject(postData)));
        }
        public List<WorkItem> GetWorkItems(List<int> ids, List<string> fields, DateTime asOf = new DateTime())
        {
            if (ids.Count == 0)
            {
                return new List<WorkItem>();
            }
            if (asOf == DateTime.MinValue)
            {
                asOf = DateTime.Now;
            }
            if (fields.Count == 0)
            {
                return JsonConvert.DeserializeObject<VSTSResponse<WorkItem>>(this.Get($"/DefaultCollection/_apis/wit/WorkItems?ids={string.Join(",", ids)}&asOf={asOf.ToUniversalTime()}&api-version={apiversion}")).value;
            }
            return JsonConvert.DeserializeObject<VSTSResponse<WorkItem>>(this.Get($"/DefaultCollection/_apis/wit/WorkItems?ids={string.Join(",", ids)}&fields={string.Join(",", fields)}&asOf={asOf.ToUniversalTime()}&api-version={apiversion}")).value;
        }

        public List<WorkItem> GetWorkItems(List<WorkItem> workitems, List<string> fields, DateTime asOf = new DateTime())
        {
            List<int> workItemIds = new List<int>();
            foreach(WorkItem i in workitems)
            {
                workItemIds.Add(i.id);
            }
            return GetWorkItems(workItemIds,fields,asOf);
        }

        public WorkItem CreateWorkItem(Guid projectId, string type, WorkItem item)
        {
            List<FieldOperation> operations = new List<FieldOperation>();
            foreach (var field in item.fields)
            {
                operations.Add(new FieldOperation("add", $"/fields/{field.Key}", field.Value));
            }

            return JsonConvert.DeserializeObject<WorkItem>(this.Patch($"DefaultCollection/{projectId}/_apis/wit/workitems/${type}/?api-version={apiversion}", JsonConvert.SerializeObject(operations)));
        }

        public WorkItem CreateWorkItem(string project, string type, WorkItem item)
        {
            List<FieldOperation> operations = new List<FieldOperation>();
            foreach (var field in item.fields)
            {
                operations.Add(new FieldOperation("add", $"/fields/{field.Key}", field.Value));
            }

            return JsonConvert.DeserializeObject<WorkItem>(this.Patch($"DefaultCollection/{project}/_apis/wit/workitems/${type}/?api-version={apiversion}", JsonConvert.SerializeObject(operations)));
        }

        public WorkItem UpdateWorkItem(WorkItem item)
        {
            List<FieldOperation> operations = new List<FieldOperation>();
            foreach (var field in item.fields)
            {
                operations.Add(new FieldOperation("add", $"/fields/{field.Key}", field.Value));
            }

            return JsonConvert.DeserializeObject<WorkItem>(this.Patch($"DefaultCollection/_apis/wit/workitems/{item.id}/?api-version={apiversion}", JsonConvert.SerializeObject(operations)));
        }
        public void DeleteWorkItem(WorkItem item)
        {
            this.Delete($"DefaultCollection/_apis/wit/workitems/{item.id}/?api-version={apiversion}");
        }

        #endregion

        #region Work Item Comments
        public List<Comment> GetWorkItemComments(int workItemId, int startRevision = 1, int numberToGet = 200, bool orderAscending = true)
        {
            string order = orderAscending ? "asc" : "desc";
            return JsonConvert.DeserializeObject<VSTSResponse<Comment>>(this.Get($"/DefaultCollection/_apis/wit/WorkItems/{workItemId}/comments/?fromRevision={startRevision}&$top={numberToGet}&order={order}&api-version={apiversion}")).value;
        }

        public Comment GetWorkItemComment(int workItemId, int revision)
        {
            return JsonConvert.DeserializeObject<Comment>(this.Get($"/DefaultCollection/_apis/wit/WorkItems/{workItemId}/comments/{revision}?api-version={apiversion}"));
        }

        #endregion
        #region Teams
        public List<Team> GetTeams(Guid projectId, int top = 100, int skip = 0)
        {
            return JsonConvert.DeserializeObject<VSTSResponse<Team>>(this.Get($"/DefaultCollection/_apis/projects/{projectId}/teams?api-version={apiversion}&$top={top}&$skip={skip}")).value;
        }
        public Team GetTeam(Guid projectId, Guid teamId)
        {
            return JsonConvert.DeserializeObject<Team>(this.Get($"/DefaultCollection/_apis/projects/{projectId}/teams/{teamId}?api-version={apiversion}"));
        }
        public List<TeamMember> GetTeamMembers(Guid projectId, Guid teamId)
        {
            return JsonConvert.DeserializeObject<VSTSResponse<TeamMember>>(this.Get($"/DefaultCollection/_apis/projects/{projectId}/teams/{teamId}?api-version={apiversion}")).value;
        }

        public Team CreateTeam(Guid projectId, string name, string description)
        {
            object newTeam = new { name = name, description = description };
            return JsonConvert.DeserializeObject<Team>(this.Post($"DefaultCollection/_apis/projects/{projectId}/teams?api-version={this.apiversion}",JsonConvert.SerializeObject(newTeam)));
        }

        public Team UpdateTeam(Guid projectId, Guid teamId, string name, string description)
        {
            object newTeam = new { name = name, description = description };
            return JsonConvert.DeserializeObject<Team>(this.Patch($"DefaultCollection/_apis/projects/{projectId}/teams/?api-version={this.apiversion}",JsonConvert.SerializeObject(newTeam)));
        }
        #endregion

        #region Work Item Types
        public List<WorkItemType> GetWorkItemTypes(Guid projectId)
        {
            return JsonConvert.DeserializeObject<VSTSResponse<WorkItemType>>(this.Get($"/DefaultCollection/{projectId}/_apis/wit/workItemTypes?api-version={apiversion}")).value;
        }

        public WorkItemType GetWorkItemType(Guid projectId, string typeName)
        {
            return JsonConvert.DeserializeObject<WorkItemType>(this.Get($"/DefaultCollection/{projectId}/_apis/wit/workItemTypes/{typeName}?api-version={apiversion}"));
        }

        public List<WorkItemField> GetWorkItemFields()
        {
            return JsonConvert.DeserializeObject<VSTSResponse<WorkItemField>>(this.Get($"/DefaultCollection/_apis/wit/fields?api-version={apiversion}")).value;
        }

        public WorkItemField GetWorkItemField(string fieldName)
        {
            return JsonConvert.DeserializeObject<WorkItemField>(this.Get($"/DefaultCollection/_apis/wit/fields/{fieldName}&api-version={apiversion}"));
        }
        #endregion

        #region repository
        public List<GitRepository> GetGitRepositoriesForProject(Guid projectId)
        {
            return JsonConvert.DeserializeObject<VSTSResponse<GitRepository>>(this.Get($"/DefaultCollection/{projectId}/_apis/git/repositories?api-version={apiversion}")).value;
        }

        public List<GitRepository> GetAllGitRepositories()
        {
            return JsonConvert.DeserializeObject<VSTSResponse<GitRepository>>(this.Get($"/DefaultCollection/_apis/git/repositories?api-version={apiversion}")).value;
        }

        public GitRepository GetGitRepository(Guid projectId, Guid repositoryId)
        {
            return JsonConvert.DeserializeObject<GitRepository>(this.Get($"/DefaultCollection/{projectId}/_apis/git/repositories/{repositoryId}?api-version={apiversion}"));
        }

        public GitRepository GetGitRepositoryByUrl(string gitCloneUrl)
        {
            return JsonConvert.DeserializeObject<GitRepository>(this.Get(gitCloneUrl.Replace($"https://{this.Account}.visualstudio.com/","") + "/vsts/info"));
        }
        #endregion
        #region Pull Requests
        #endregion
    }
}
