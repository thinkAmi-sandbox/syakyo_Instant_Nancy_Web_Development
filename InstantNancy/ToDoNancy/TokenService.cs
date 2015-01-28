using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy.Security;
    public class TokenService
    {
        public string GetToken(string userName)
        {
            return userName;
        }

        public Nancy.Security.IUserIdentity GetUserFromToken(string token)
        {
            return new ToDoNancy.User { UserName = token };
        }
    }
}