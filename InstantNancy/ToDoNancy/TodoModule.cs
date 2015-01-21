using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy;
    using Nancy.ModelBinding;

    public class TodoModule : NancyModule
    {
        public static Dictionary<long, Todo> store = new Dictionary<long, Todo>();

        public TodoModule() : base("todos")
        {
            Get["/"] = _ => Response.AsJson(store.Values);

            Post["/"] = _ =>
            {
                // TodoオブジェクトをPOSTのBodyにバインディング
                var newTodo = this.Bind<Todo>();

                if (newTodo.id == 0)
                {
                    newTodo.id = store.Count + 1;
                }

                if (store.ContainsKey(newTodo.id)) return HttpStatusCode.NotAcceptable;

                store.Add(newTodo.id, newTodo);


                return Response.AsJson(newTodo)
                    .WithStatusCode(HttpStatusCode.Created);
            };
        }
    }
}