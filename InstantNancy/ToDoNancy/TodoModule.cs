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
                    .WithModel(todoStore.GetAll().ToArray())
                    //.WithModel(todoStore.GetAll())
                    .WithView("Todos");

            Post["/"] = _ =>
            {
                // TodoオブジェクトをPOSTのBodyにバインディング
                var newTodo = this.Bind<Todo>();

                if (newTodo.id == 0)
                {
                    newTodo.id = todoStore.Count + 1;
                }

                if (!todoStore.TryAdd(newTodo)) return HttpStatusCode.NotAcceptable;

                return Negotiate
                    .WithModel(newTodo)
                    .WithStatusCode(HttpStatusCode.Created);
            };

            Put["/{id}"] = p =>
            {
                var updatedTodo = this.Bind<Todo>();

                if (!todoStore.TryUpdate(updatedTodo)) return HttpStatusCode.NotFound;

                return updatedTodo;
            };

            Delete["/{id}/"] = p =>
            {
                if (!todoStore.TryRemove(p.id)) return HttpStatusCode.NotFound;

                return HttpStatusCode.OK;
            };
        }
    }
}