using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy;
    using Nancy.Conventions;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var mongoDataStore = new MongoDataStore("mongodb://localhost:27017/todos");
            container.Register<IDataStore>(mongoDataStore);
        }

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            Conventions.StaticContentsConventions.Add(
                Nancy.Conventions.StaticContentConventionBuilder.AddDirectory("/docs", "Docs"));
        }
    }
}