using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancyTests
{
    using CsQuery.ExtensionMethods;
    using Nancy;
    using Nancy.Testing;
    using Xunit;
    using MongoDB.Driver;
    using ToDoNancy;

    public class TodoModulesTests
    {
        private Browser sut;
        private Todo aTodo;
        private Todo anEditedTodo;

        public TodoModulesTests()
        {
            var connectionString = "mongodb://localhost:27017/todos";
            var server = new MongoClient(connectionString).GetServer();
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            var database = server.GetDatabase(databaseName);

            database.Drop();

            sut = new Browser(new Bootstrapper());
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
            var actual = sut.Get("/todos", with => with.Accept("application/json"));

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
            Assert.Empty(actual.Body.DeserializeJson<Todo[]>());
        }

        [Fact]
        public void Should_return_201_create_with_a_todo_is_posted()
        {
            var actual = sut.Post("/todos/", with =>
            {
                with.JsonBody(aTodo);
                with.Accept("application/json");
            });

            Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
        }

        [Fact]
        public void Should_return_created_todo_as_json_when_a_todo_is_posted()
        {
            var actual = sut.Post("/todos/",
              with =>
              {
                  with.JsonBody(aTodo);
                  with.Accept("application/json");
              });

            var actualBody = actual.Body.DeserializeJson<Todo>();
            Assertions.AreSame(aTodo, actualBody);
        }

        [Fact]
        public void Should_return_created_todo_as_xml_when_a_todo_is_posted()
        {
            var actual = sut.Post("/todos/", with =>
            {
                with.JsonBody(aTodo);
                with.Accept("application/xml");
            });
            var actualBody = actual.Body.DeserializeXml<Todo>();

            Assertions.AreSame(aTodo, actualBody);
        }

        [Fact]
        public void Should_not_accept_posting_to_with_duplicate_id()
        {
            var actual = sut.Post("/todos/", with => {
                    with.JsonBody(anEditedTodo);
                    with.Accept("application/json");
                })
                .Then
                .Post("/todos/", with => {
                    with.JsonBody(anEditedTodo);
                    with.Accept("application/json");
                });

            Assert.Equal(HttpStatusCode.NotAcceptable, actual.StatusCode);
        }

        [Fact]
        public void Should_be_able_to_get_posted_todo()
        {
            var actual = sut.Post("/todos/", with => {
                with.JsonBody(aTodo);
                with.Accept("application/json");
                })
                .Then
                .Get("/todos/", with => with.Accept("application/json"));

            // JSONフォーマットのテキストを、Todoオブジェクトの配列にデシリアライズ
            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            Assert.Equal(1, actualBody.Length);
            AssertAreSame(aTodo, actualBody[0]);
        }

        [Fact]
        public void Should_be_able_to_edit_todo_with_put()
        {
            var actual = sut.Post("/todos/", with => {
                with.JsonBody(aTodo);
                with.Accept("application/json");
            })
            .Then
            .Put("/todos/1", with => {
                with.JsonBody(anEditedTodo);
                with.Accept("application/json");
            })
            .Then
            .Get("/todos/", with => with.Accept("application/json"));

            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            Assert.Equal(1, actualBody.Length);
            AssertAreSame(anEditedTodo, actualBody[0]);
        }

        [Fact]
        public void Should_be_able_to_get_posted_xml_todo()
        {
            var actual = sut.Post("/todos/", with =>
            {
                with.XMLBody(aTodo);
                with.Accept("application/xml");
            })
            .Then
            .Get("todos/", with => with.Accept("application/json"));

            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            Assert.Equal(1, actualBody.Length);
            AssertAreSame(aTodo, actualBody[0]);
        }

        // Cannot pass
        [Fact]
        public void Should_be_able_to_get_posted_todo_as_xml()
        {
            var actual = sut.Post("/todos/", with =>
            {
                with.XMLBody(aTodo);
                with.Accept("application/xml");
            })
            .Then
            .Get("/todos/", with => with.Accept("application/xml"));

            // !! System.InvalidOperationException !!
            // 追加情報:XML シリアル化を可能にするには、IEnumerable から継承される型は、継承階層のすべてのレベルで Add(System.Object) が実装される必要があります。
            // MongoDB.Driver.MongoCursor`1[[ToDoNancy.Todo, ToDoNancy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]] には Add(System.Object) が実装されていません。
            var actualBody = actual.Body.DeserializeXml<Todo[]>();

            Assert.Equal(1, actualBody.Length);
            Assertions.AreSame(aTodo, actualBody[0]);
        }

        [Fact]
        public void Should_be_able_to_get_posted_todo_as_protobuf()
        {
            var actual = sut.Put("/todos/", with =>
            {
                var stream = new System.IO.MemoryStream();
                ProtoBuf.Serializer.Serialize(stream, aTodo);
                with.Body(stream, "application/x-protobuf");
                with.Accept("application/xml");
            })
            .Then
            .Get("/todos/", with => with.Accept("application/x-protobuf"));

            // !! System.InvalidOperationException !!
            // Type is not expected. and no contract can be inferred: ToDoNancy.Todo
            var actualBody = ProtoBuf.Serializer.Deserialize<Todo[]>(actual.Body.AsStream());

            Assert.Equal(1, actualBody.Length);
            Assertions.AreSame(aTodo, actualBody[0]);
        }

        [Fact]
        public void Should_be_able_to_delete_todo_with_delete()
        {
            // CsQuery.ExtensionMethodsにToJSONメソッドがある
            var actual = sut.Post("/todos/", with =>
            {
                with.Body(aTodo.ToJSON());
                with.Accept("application/json");
            })
            .Then
            .Delete("/todos/1")
            .Then
            .Get("/todos/", with => with.Accept("application/json"));

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
            Assert.Empty(actual.Body.DeserializeJson<Todo[]>());
        }

        [Fact]
        public void Should_support_head()
        {
            var actual = sut.Head("/todos/", with => with.Accept("application/json"));

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public void Should_support_options()
        {
            var actual = sut.Options("/todos/");

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }


        //---------------

        private void AssertAreSame(Todo expected, Todo actual)
        {
            Assert.Equal(expected.title, actual.title);
            Assert.Equal(expected.order, actual.order);
            Assert.Equal(expected.completed, actual.completed);
        }
    }
}
