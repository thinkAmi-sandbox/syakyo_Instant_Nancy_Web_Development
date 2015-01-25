using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancyTests
{
    using Xunit.Sdk;

    public class KnownDevMachinesOnly : ITestClassCommand
    {
        private TestClassCommand command;
        private ITypeInfo typeInf;

        public int ChooseNextTest(ICollection<IMethodInfo> testsLeftToRun)
        {
            return command != null ? command.ChooseNextTest(testsLeftToRun) : -1;
        }

        public Exception ClassFinish()
        {
            return command != null ? command.ClassFinish() : null;
        }

        public Exception ClassStart()
        {
            return command != null ? command.ClassStart() : null;
        }

        public IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo testMethod)
        {
            return command != null ? command.EnumerateTestCommands(testMethod) : new ITestCommand[0];
        }

        public IEnumerable<IMethodInfo> EnumerateTestMethods()
        {
            return command != null ? command.EnumerateTestMethods() : new IMethodInfo[0];
        }

        public bool IsTestMethod(IMethodInfo testMethod)
        {
            return command != null ? command.IsTestMethod(testMethod) : false;
        }

        public object ObjectUnderTest
        {
            get { return command != null ? command.ObjectUnderTest : null; }
        }

        public ITypeInfo TypeUnderTest
        {
            get { return typeInf; }
            set
            {
                typeInf = value;
                if (IsDevMachine())
                {
                    command = new TestClassCommand(value);
                }
            }
        }

        public bool IsDevMachine()
        {
            if (System.Environment.OSVersion.Version.Minor == 1)
            {
                // Windows7と考える
                return true;
            }

            return false;
        }
    }
}
