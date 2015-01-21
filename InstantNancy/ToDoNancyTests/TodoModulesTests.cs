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

    public class TodoModulesTests
    {
        private Browser sut;
        private Todo aTodo;
        private Todo anEditedTodo;

        public TodoModulesTests()
        {
            TodoModule.store.Clear();
            sut = new Browser(new DefaultNancyBootstrapper());
            aTodo = new Todo
            {
                title = "task 1",
                order = 0,
                completed = false
            };
            anEditedTodo = new Todo
            {
                id = 42,
                title = "edited name",
                order = 0,
                completed = false
            };
        }

        [Fact]
        public void Should_return_empty_list_on_get_when_no_todos_have_been_posted()
        {
            var actual = sut.Get("/todos");

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);

            Assert.Empty(actual.Body.DeserializeJson<Todo[]>());
        }

        [Fact]
        public void Should_return_201_create_with_a_todo_is_posted()
        {
            var actual = sut.Post("/todos/", with => with.JsonBody(aTodo));

            Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
        }

        [Fact]
        public void Should_not_accept_posting_to_with_duplicate_id()
        {
            var actual = sut.Post("/todos/", with => with.JsonBody(anEditedTodo))
                .Then
                .Post("/todos/", with => with.JsonBody(anEditedTodo));

            Assert.Equal(HttpStatusCode.NotAcceptable, actual.StatusCode);
        }

        [Fact]
        public void Should_be_able_to_get_posted_todo()
        {
            var actual = sut.Post("/todos/", with => with.JsonBody(aTodo))
                .Then
                .Get("/todos/");

            // JSONフォーマットのテキストを、Todoオブジェクトの配列にデシリアライズ
            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            Assert.Equal(1, actualBody.Length);

            AssertAreSame(aTodo, actualBody[0]);
        }

        private void AssertAreSame(Todo expected, Todo actual)
        {
            Assert.Equal(expected.title, actual.title);
            Assert.Equal(expected.order, actual.order);
            Assert.Equal(expected.completed, actual.completed);
        }
    }
}
