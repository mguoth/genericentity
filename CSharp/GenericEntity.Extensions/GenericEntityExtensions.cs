﻿using GenericEntity.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace GenericEntity.Extensions
{
    public static class GenericEntityExtensions
    {
        public static void AddStandard(this IGenericEntityExtensions extensions)
        {
            extensions.RegisterExtension(Assembly.GetExecutingAssembly());
        }
    }
}
