using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancyTests
{
    using Nancy;
    using Nancy.Testing;
    using Xunit;

    public class HomeModuleTests
    {
        [Fact]
        public void Should_answer_200_on_root_path()
        {
            // テスト対象
            var sut = new Browser(new DefaultNancyBootstrapper());

            // 検証対象
            var actual = sut.Get("/");

            // 検証
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }
    }
}
