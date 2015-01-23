using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy;
    

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var mongoDataStore = new MongoDataStore("mongodb://localhost:27017/todos");
            container.Register<IDataStore>(mongoDataStore);
        }
    }
}