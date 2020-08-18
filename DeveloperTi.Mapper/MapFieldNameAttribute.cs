using System;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperTi.Mapper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapFieldNameAttribute : Attribute
    {
        public List<string> AttributeNames { get; set; }
        public MapFieldNameAttribute(params string[] attributeName)
        {
            this.AttributeNames = attributeName.ToList();
        }
    }
}
