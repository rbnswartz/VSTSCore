using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSTSCore.Models
{
    public class WorkItemType
    {
        public string name;
        public string description;
        public string xmlForm;
        public List<FieldDefinition> fieldInstances;
        public Dictionary<string, List<FieldTransitions>> transitions;
    }
}
