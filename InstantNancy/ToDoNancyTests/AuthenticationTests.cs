using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancyTests
{
    using Nancy;
    using Nancy.Testing;
    using Nancy.SimpleAuthentication;
    using SimpleAuthentication.Core;
    using Xunit;
    using ToDoNancy;

    public class AuthenticationTests
    {
        [Xunit.Fact]
        public void Should_redirect_to_root_on_social_authc_callback()
        {
            new Nancy.Testing.Browser(new ToDoNancy.Bootstrapper()).Get("/testing");
            var userNameToken = new ToDoNancy.TokenService().GetToken("chr_horsdal");
            var callbackData = new Nancy.SimpleAuthentication.AuthenticateCallbackData
            {
                AuthenticatedClient = new SimpleAuthentication.Core.AuthenticatedClient("")
                {
                    UserInformation = new SimpleAuthentication.Core.UserInformation
                    {
                        UserName = "chr_horsdal"
                    }
                }
            };

            var sut = new ToDoNancy.SocialAuthenticatoinCallbackProvider();

            var actual = (Response)sut.Process(TestingAuthenticationModule.actualModule, callbackData);

            Assert.Contains("todoUser", actual.Cookies.Select(cookie => cookie.Name));
            Assert.Contains(userNameToken, actual.Cookies.Select(cookie => cookie.Value));
        }

        [Fact]
        public void Should_set_user_identity_when_cookie_is_set()
        {
            var expected = "chr_horsdal";
            var userNameToken = new ToDoNancy.TokenService().GetToken(expected);

            var sut = new Browser(new ToDoNancy.Bootstrapper());

            sut.Get("/testing", with => with.Cookie("todoUser", userNameToken));

            Assert.Equal(expected, TestingAuthenticationModule.actualUser.UserName);
        }
    }
}
