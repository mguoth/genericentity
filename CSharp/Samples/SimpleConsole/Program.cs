﻿using Org.GenericEntity.Abstractions;
using Org.GenericEntity.Extensions.FileSystem;
using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Reflection;
using Org.GenericEntity.Extensions.Avro;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Org.GenericEntity.Model.Samples.SimpleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine($"Syntax: {Assembly.GetExecutingAssembly().GetName().Name}.exe $schema");
                return;
            }

            string schemaId = args[0];

            //Adds the Avro extension (registers "avsc" schema parser)
            GenericEntity.Extensions.AddAvroSchema();

            //Creating generic entity instance
            ISchemaRepository schemaRepository = new FileSystemSchemaRepository("Schemas");
            SchemaInfo schemaInfo = schemaRepository.GetSchema(schemaId);
            GenericEntity genericEntity = new GenericEntity(schemaInfo);

            PrintToConsole(genericEntity, true);

            PrintToConsole(genericEntity, false);
        }

        private static void PrintToConsole(GenericEntity genericEntity, bool edit)
        {
            int[] columnWidths = new int[] { 15, 18, 30, 20, 20 };

            if (edit)
            {
                Console.WriteLine($@"Editing ""{genericEntity.SchemaInfo.Id}"" fields:");
            }
            else
            {
                Console.WriteLine($@"Printing ""{genericEntity.SchemaInfo.Id}"" fields:");
            }
            Console.WriteLine();

            Console.WriteLine($"{"Name".PadRight(columnWidths[0])}{"DisplayName".PadRight(columnWidths[1])}{"Description".PadRight(columnWidths[2])}{"Value Type (.NET)".PadRight(columnWidths[3])}{"Value"}");
            Console.WriteLine($"{String.Empty.PadRight(columnWidths.Sum(), '-')}");

            foreach (IField field in genericEntity.Fields)
            {
                string fieldRow = $"{field.Name.PadRight(columnWidths[0])}{(field.DisplayName ?? "").PadRight(columnWidths[1])}{(field.Description ?? "").PadRight(columnWidths[2])}{field.ValueType.ToString().PadRight(columnWidths[3])}";

                if (edit)
                {
                    while (true)
                    {
                        Console.Write(fieldRow);

                        string value = Console.ReadLine();

                        if (value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                        {
                            value = null;
                        }

                        if (field.TrySetValue(value))
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine(@$"Can't set ""{value}"" into ""{field.ValueType}"" field type. Try again.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"{fieldRow}{field.GetValue<string>() ?? "null"}");
                }
            }

            Console.WriteLine();
        }
    }
}
