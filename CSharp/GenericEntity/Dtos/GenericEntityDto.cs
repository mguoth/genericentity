﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GenericEntity
{
    internal class GenericEntityDto
    {
        private IDictionary<string, object> fields = new Dictionary<string, object>();

        public string Schema { get; set; }

        [JsonConverter(typeof(GenericEntityDtoFieldsConverter))]
        public IDictionary<string, object> Fields { get => fields; set => fields = value; }
    }
}
