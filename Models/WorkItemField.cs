using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSTSCore.Models
{
    public class WorkItemField
    {
        public string name;
        public string referenceName;
        public string type;
        public bool readOnly;
        public List<WorkItemFieldOperations> supportedOperations;
        public string url;
    }
}
