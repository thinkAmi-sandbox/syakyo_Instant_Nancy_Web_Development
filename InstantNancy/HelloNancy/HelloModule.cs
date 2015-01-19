using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloNancy
{
    using Nancy;

    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get["/"] = _ => "Hello Nancy World";
        }
    }
}