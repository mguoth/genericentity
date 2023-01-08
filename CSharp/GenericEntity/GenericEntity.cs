﻿using GenericEntity.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text.Json.Serialization;
using System.IO;
using System.Runtime.CompilerServices;

namespace GenericEntity
{
    /// <summary>
    /// Generic entity
    /// </summary>
    [JsonConverter(typeof(GenericEntityConverter))]
    public partial class GenericEntity
    {
        private static readonly object syncRoot = new object();
        private static readonly IDictionary<string, GenericSchema> compiledSchemaCache = new Dictionary<string, GenericSchema>();

        public GenericEntity(SchemaInfo schemaInfo, ISchemaParser schemaParser)
        {
            this.SchemaInfo = schemaInfo;

            GenericSchema schema = null;
            lock (syncRoot)
            {
                //Get from cache or create new
                if (!compiledSchemaCache.TryGetValue(this.SchemaInfo.Uri.ToString(), out schema))
                {
                    //Compile and index into cache
                    schema = schemaParser.Parse(this.SchemaInfo.RawSchema);
                    compiledSchemaCache[this.SchemaInfo.Uri.ToString()] = schema;
                }
            }
            
            this.Fields = BuildFields(schema);
        }

        internal GenericEntity(GenericEntityDto dto, SchemaInfo schemaInfo, ISchemaParser schemaParser): this(schemaInfo, schemaParser)
        {
            foreach (var field in dto.Data)
            {
                if (field.Value is IFieldValueProvider jsonValueProvider)
                {
                    //Set value provider
                    this.ImportFieldValue(this.Fields[field.Key], jsonValueProvider);
                }
                else
                {
                    //Set scalar value
                    this.Fields[field.Key].SetValue(field.Value);
                }
            }
        }

        /// <summary>
        /// Gets extensions.
        /// </summary>
        public static IGenericEntityExtensions Extensions { get; } = new GenericEntityExtensions();

        public SchemaInfo SchemaInfo { get; }

        /// <summary>
        /// Gets fields
        /// </summary>
        public FieldCollection Fields { get; }

        private FieldCollection BuildFields(GenericSchema schema)
        {
            FieldCollectionBuilder builder = new FieldCollectionBuilder();

            foreach (FieldDefinition fieldDefinition in schema.Fields)
            {
                IField field = this.CreateField(fieldDefinition);
                builder.Add(field);
            }

            return builder.Build();
        }
        
        private IField CreateField(FieldDefinition fieldDefinition)
        {
            Type targetFieldType = typeof(Field<>).MakeGenericType(new Type[] { fieldDefinition.ValueType });
            IField field = (IField)targetFieldType.GetConstructor(new Type[] { typeof(FieldDefinition) }).Invoke(new object[] { fieldDefinition });
            return field;
        }

        private void ImportFieldValue(IField field, IFieldValueProvider fieldValueProvider)
        {
            if (field.ValueType == typeof(string))
            {
                field.SetValue(fieldValueProvider.GetString());
            }
            else if (field.ValueType == typeof(int))
            {
                field.SetValue(fieldValueProvider.GetInt32());
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
