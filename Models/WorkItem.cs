using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSTSCore.Models
{
    public class WorkItem
    {
        public WorkItem()
        {
            this.fields = new Dictionary<string, object>();
        }
        public int id;
        public int rev;
        public Dictionary<string, object> fields;
        public string url;
    }
}
