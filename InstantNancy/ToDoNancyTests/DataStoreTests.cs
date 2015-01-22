using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoNancyTests
{
    using Nancy.Testing;
    using FakeItEasy;
    using Xunit;
    //using Xunit.Extensions;
    using ToDoNancy;


    public class DataStoreTests
    {
        private readonly IDataStore fakeDataStore;
        private readonly Nancy.Testing.Browser sut;
        private readonly Todo aTodo;

        public DataStoreTests()
        {
            fakeDataStore = FakeItEasy.A.Fake<IDataStore>();
            sut = new Browser(with =>
                {
                    with.Dependency(fakeDataStore);
                    with.Module<TodoModule>();
                }
            );

            aTodo = new Todo()
            {
                id = 5,
                title = "task 10",
                order = 100,
                completed = true
            };
        }

        [Xunit.Fact]
        public void Should_store_posted_todos_in_datastore()
        {
            sut.Post("/todos/", with => with.JsonBody(aTodo));
        }


        private void AssertCalledTryAddOnDataStoreWith(Todo expected)
        {
            // サンプルコードの場合
            FakeItEasy.A.CallTo(() => fakeDataStore.TryAdd(A<Todo>
                .That.Matches(actual => ToDoNancyTests.Assertions.AreSame(expected, actual))))
                .MustHaveHappened();

            // 書籍内コードの場合
            // 「ステートメント本体を含むラムダ式は、式ツリーに変換できません」のエラーになる
            //FakeItEasy.A.CallTo(() => fakeDataStore.TryAdd(A<Todo>.That.Matches(actual => 
            //{
            //    Assert.Equal(expected.title, actual.title);
            //    Assert.Equal(expected.order, actual.order);
            //    Assert.Equal(expected.completed, actual.completed);

            //    return true;
            //}))).MustHaveHappened();
        }
    }
}
