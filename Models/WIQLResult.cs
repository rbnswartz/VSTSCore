using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSTSCore.Models
{
    public class WIQLResult
    {
        public string queryType;
        public DateTime asOf;
        public List<FieldDefinition> columns;
        public List<Target> workItems;
    }
}
