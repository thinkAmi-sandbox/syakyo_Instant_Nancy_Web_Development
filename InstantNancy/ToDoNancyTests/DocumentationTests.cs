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
    using ToDoNancy;

    [RunWith(typeof(KnownDevMachinesOnly))]
    public class DocumentationTests
    {
        [Fact]
        public void Should_give_access_to_overview_documentation()
        {
            var sut = new Browser(new Bootstrapper());
            var actual = sut.Get("/docs/overview.html", with => with.Accept("text/html"));

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }
    }
}
