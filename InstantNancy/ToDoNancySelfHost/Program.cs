using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancySelfHost
{
    using Nancy.Hosting.Self;
    using ToDoNancy;
        
    class Program
    {
        static void Main(string[] args)
        {
            TodoModule artificiaReference;
            var nancyHost = new Nancy.Hosting.Self.NancyHost(new Uri("http://localhost:8080"));
            nancyHost.Start();

            Console.ReadKey();

            nancyHost.Stop();
        }
    }
}
