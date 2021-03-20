﻿using System;
using System.Linq;
using System.Reflection;

namespace NinjaBay.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static string Description(this Enum value)
        {
            // get attributes  
            var field = value.GetType().GetRuntimeField(value.ToString());
            var attributes = field.GetCustomAttributes(false);

            // Description is in a hidden Attribute class called DisplayAttribute
            // Not to be confused with DisplayNameAttribute
            dynamic displayAttribute = null;

            if (attributes.Any()) displayAttribute = attributes.ElementAt(0);

            // return description
            return displayAttribute?.Description ?? "Description Not Found";
        }
    }
}