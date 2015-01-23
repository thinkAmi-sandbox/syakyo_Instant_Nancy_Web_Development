using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancyTests
{
    using ToDoNancy;
    using Xunit;

    class Assertions
    {
        public static bool AreSame(Todo expected, Todo actual)
        {
            Assert.Equal(expected.title, actual.title);
            Assert.Equal(expected.order, actual.order);
            Assert.Equal(expected.completed, actual.completed);
            return true;
        }
    }
}
