using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSTSCore.Models
{
    public class FieldOperation
    {
        public FieldOperation(string op, string path, object value)
        {
            this.op = op;
            this.path = path;
            this.value = value;
        }
        public string op;
        public string path;
        public object value;
    }
}
