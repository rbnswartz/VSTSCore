﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSTSCore.Models
{
    public class VSTSResponse<T>
    {
        public int count;
        public List<T> value;
    }
}
