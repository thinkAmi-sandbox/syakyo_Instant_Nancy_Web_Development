using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    public class Todo
    {
        public string userName { get; set; }
        public long id { get; set; }
        public string title { get; set; }
        public int order { get; set; }
        public bool completed { get; set; }
    }
}