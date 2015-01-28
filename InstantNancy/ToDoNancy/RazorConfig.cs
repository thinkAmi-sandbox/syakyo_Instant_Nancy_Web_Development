using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy.ViewEngines.Razor;

    public class RazorConfig : Nancy.ViewEngines.Razor.IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "ToDoNancy";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "ToDoNancy";
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

    }
}