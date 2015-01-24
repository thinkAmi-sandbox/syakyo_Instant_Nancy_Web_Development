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

        public TodoModule(IDataStore todoStore) : base("todos")
        {
            Get["/"] = _ => todoStore.GetAll();

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
                
                return Response.AsJson(updatedTodo);
            };

            Delete["/{id}/"] = p =>
            {
                if (!todoStore.TryRemove(p.id)) return HttpStatusCode.NotFound;

                return HttpStatusCode.OK;
            };
        }
    }
}