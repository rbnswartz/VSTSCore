using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSTSCore.Models
{
    public class Query
    {
        public Guid id;
        public string name;
        public string path;
        public Identity createdBy;
        public DateTime createdDate;
        public Identity lastModifiedBy;
        public DateTime lastModifiedDate;
        public bool isFolder;
        public bool hasChildren;
        public List<Query> children;
        public bool isPublic;
    }
}
