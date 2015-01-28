using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy.Security;
    public class User : IUserIdentity
    {
        public static IUserIdentity Anonymous { get; private set; }

        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; private set; }

        static User()
        {
            Anonymous = new User { UserName = "anonymous" };
        }

    }
}