using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy;
    using Nancy.Cookies;
    using Nancy.SimpleAuthentication;
    public class SocialAuthenticatoinCallbackProvider
        : Nancy.SimpleAuthentication.IAuthenticationCallbackProvider
    {
        public dynamic Process(
            Nancy.NancyModule module,
            Nancy.SimpleAuthentication.AuthenticateCallbackData callbackData)
        {
            module.Context.CurrentUser = new User
            {
                UserName = callbackData.AuthenticatedClient.UserInformation.UserName
            };

            return module.Response
                .AsRedirect("/")
                .WithCookie(new Nancy.Cookies.NancyCookie("todoUser",
                    new ToDoNancy.TokenService().GetToken(module.Context.CurrentUser.UserName)));       
        }

        public dynamic OnRedirectToAuthenticationProviderError(
            NancyModule module, string errorMessage)
        {
            return "login failed:" + errorMessage;
        }
    }
}