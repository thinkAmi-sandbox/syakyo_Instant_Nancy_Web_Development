using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancyTests
{
    using Nancy;
    using Nancy.Testing;
    using FakeItEasy;
    using Xunit;
    using Xunit.Extensions;
    using ToDoNancy;
    public class UserSpecificTodoTests
    {
        private const string UserName = "Alice";
        private readonly Nancy.Testing.Browser sut;
        private readonly ToDoNancy.IDataStore fakeDataStore;

        public UserSpecificTodoTests()
        {
            fakeDataStore = FakeItEasy.A.Fake<IDataStore>();
            sut = new Browser(with =>
            {
                with.Module<TodoModule>();
                with.ApplicationStartup((contaner, pipelines) =>
                {
                    contaner.Register(fakeDataStore);
                    pipelines.BeforeRequest += ctx =>
                    {
                        ctx.CurrentUser = new User { UserName = UserName };
                        return null;
                    };
                });
            });
        }

        [Xunit.Extensions.Theory]
        [Xunit.Extensions.InlineData(0, 0, 0)]
        [InlineData(0, 10, 0)]
        [InlineData(0, 0, 10)]
        [InlineData(0, 10, 10)]
        [InlineData(1, 0, 0)]
        [InlineData(1, 10, 0)]
        [InlineData(1, 0, 10)]
        [InlineData(1, 10, 10)]
        [InlineData(42, 0, 0)]
        [InlineData(42, 10, 0)]
        [InlineData(42, 0, 10)]
        [InlineData(42, 10, 10)]
        public void Should_only_get_user_own_todos(
            int nofTodosForUser,
            int nofTodosForAnonymousUser,
            int nofTodosForOtherUser)
        {
            var todosForUser = Enumerable.Range(0, nofTodosForUser)
                .Select(i => new Todo { id = i, userName = UserName });

            var todosForAnonymousUser = Enumerable.Range(0, nofTodosForAnonymousUser)
                .Select(i => new Todo { id = i });

            var todosForOtherUser = Enumerable.Range(0, nofTodosForOtherUser)
                .Select(i => new Todo { id = i, userName = "Bob" });

            A.CallTo(() => fakeDataStore.GetAll())
                .Returns(todosForUser.Concat(todosForAnonymousUser.Concat(todosForOtherUser)));

            var actual = sut.Get("/todos/", with => with.Accept("application/json"));

            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            Assert.Equal(nofTodosForUser, actualBody.Length);
        }
    }
}
