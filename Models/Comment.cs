using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSTSCore.Models
{
    public class Comment
    {
        public int revision;
        public string text;
        public Identity revisedBy;
    }
}
