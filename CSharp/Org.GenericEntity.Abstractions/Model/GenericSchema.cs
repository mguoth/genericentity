﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Org.GenericEntity.Abstractions
{
    public class GenericSchema
    {
        public string RawSchema { get; set; }
        public IList<FieldDefinition> Fields { get; set; }
    }
}
