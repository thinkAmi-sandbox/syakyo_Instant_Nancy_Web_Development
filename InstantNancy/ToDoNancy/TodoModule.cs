using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    public class TodoModule
    {
        public static Dictionary<long, Todo> store = new Dictionary<long, Todo>();
    }
}