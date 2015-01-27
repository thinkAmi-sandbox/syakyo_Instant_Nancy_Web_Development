using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;

    public class TodoModule : NancyModule
    {
        public static Dictionary<long, Todo> store = new Dictionary<long, Todo>();

        public TodoModule(IDataStore todoStore) : base("todos")
        {
            Get["/"] = _ =>
                Negotiate
                    // `.WithModel(todoStore.GetAll())`だと、例外発生
                    // MongoDB.Driver.MongoCursor`1[ToDoNancy.Todo]' のオブジェクトを型 'ToDoNancy.Todo[]' にキャストできません。
                    // `.WithModel(todoStore.GetAll().ToArray())`でArrayを返すように変更
                    //.WithModel(todoStore.GetAll())            // NG
                    //.WithModel(todoStore.GetAll().ToArray()   // OK

                    // No.1058で以下のように修正が入ると、エラーにならない
                    .WithModel(todoStore.GetAll()
                        .Where(todo => todo.userName == Context.CurrentUser.UserName)
                        .ToArray())

                    .WithView("Todos");

            Post["/"] = _ =>
            {
                // TodoオブジェクトをPOSTのBodyにバインディング
                var newTodo = this.Bind<Todo>();
                newTodo.userName = Context.CurrentUser.UserName;

                if (newTodo.id == 0)
                {
                    newTodo.id = todoStore.Count + 1;
                }

                if (!todoStore.TryAdd(newTodo)) return HttpStatusCode.NotAcceptable;

                return Negotiate
                    .WithModel(newTodo)
                    .WithStatusCode(HttpStatusCode.Created)
                    .WithView("Created");
            };

            Put["/{id}"] = p =>
            {
                var updatedTodo = this.Bind<Todo>();

                if (!todoStore.TryUpdate(updatedTodo, Context.CurrentUser.UserName))
                {
                    return HttpStatusCode.NotFound;
                }
                
                return updatedTodo;
            };

            Delete["/{id}/"] = p =>
            {
                if (!todoStore.TryRemove(p.id, Context.CurrentUser.UserName))
                {
                    return HttpStatusCode.NotFound;
                }

                return HttpStatusCode.OK;
            };
        }
    }
}