using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancyTests
{
    using Nancy;
    using Nancy.Security;
    public class TestingAuthenticationModule : Nancy.NancyModule
    {
        public static Nancy.Security.IUserIdentity actualUser;
        public static TestingAuthenticationModule actualModule;

        public TestingAuthenticationModule()
        {
            Get["/testing"] = _ =>
            {
                actualUser = Context.CurrentUser;
                actualModule = this;
                return HttpStatusCode.OK;
            };
        }
    }
}
