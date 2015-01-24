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
            yield return "HyRes.Models";
            yield return "HyRes.Website";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "HyRes.Models";
            yield return "HyRes.Website.Infrastructure.Helpers";
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

    }
}