using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy;
    using Nancy.Conventions;
    using System.Configuration;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var connectionString = System.Configuration.ConfigurationManager.AppSettings.Get("MONGOLAB_URI");
            var mongoDataStore = new MongoDataStore(connectionString);
            //var mongoDataStore = new MongoDataStore("mongodb://localhost:27017/todos");
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