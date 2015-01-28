using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using Nancy;
    using Nancy.Conventions;
    using Nancy.Bootstrapper;
    using NLog;
    using NLog.Targets;
    using NLog.Targets.Wrappers;
    using System.Configuration;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private NLog.Logger log = NLog.LogManager.GetLogger("RequestLogger");

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(
                new NLog.Targets.Wrappers.AsyncTargetWrapper(new NLog.Targets.EventLogTarget()));

            LogAllRequests(pipelines);
            LogAllResponseCodes(pipelines);
            LogUnhandledExceptions(pipelines);

            SetCurrentUserWhenLoggedIn(pipelines);
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var connectionString = System.Configuration.ConfigurationManager.AppSettings.Get("MONGOLAB_URI");
            var mongoDataStore = new MongoDataStore(connectionString);
            container.Register<IDataStore>(mongoDataStore);
        }

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            Conventions.StaticContentsConventions.Add(
                Nancy.Conventions.StaticContentConventionBuilder.AddDirectory("/docs", "Docs"));
        }


        //-----------------
        private void LogAllRequests(Nancy.Bootstrapper.IPipelines pipelines)
        {
            pipelines.BeforeRequest += ctx =>
            {
                log.Info("Handling request {0} \"{1}\"", ctx.Request.Method, ctx.Request.Path);
                return null;
            };
        }

        private void LogAllResponseCodes(IPipelines pipelines)
        {
            pipelines.AfterRequest += ctx =>
                log.Info("Responding {0} to {1} \"{2}\"",
                    ctx.Response.StatusCode, ctx.Request.Method, ctx.Request.Path);
        }

        private void LogUnhandledExceptions(IPipelines pipelines)
        {
            pipelines.OnError.AddItemToEndOfPipeline((ctx, err) =>
            {
                log.ErrorException(string.Format(
                    "Request {0} \"{1}\" failed",
                    ctx.Request.Method, ctx.Request.Path), err);
                return null;
            });
        }


        private static void SetCurrentUserWhenLoggedIn(IPipelines pipelines)
        {
            pipelines.BeforeRequest += ctx =>
            {
                if (ctx.Request.Cookies.ContainsKey("todoUser"))
                {
                    ctx.CurrentUser = new TokenService().GetUserFromToken(
                        ctx.Request.Cookies["todoUser"]);
                }
                else
                {
                    ctx.CurrentUser = User.Anonymous;
                }
                return null;
            };
        }
    }
}