using System;

namespace VSTSCore.Models
{
    public class GitRepository
    {
        public Guid id;
        public string url;
        public string remoteUrl;
        public Project project;
    }
}