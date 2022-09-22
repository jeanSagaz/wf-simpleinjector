using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Diagnostics;
using SimpleInjector.Integration.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using WebForm.App.IoC;
using WebForm.Data.Interfaces;
using WebForm.Data.Repository;

using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(
    typeof(WebForm.App.PageInitializerModule),
    "Initialize")]

namespace WebForm.App
{
    public sealed class PageInitializerModule : IHttpModule
    {
        public static void Initialize()
        {
            DynamicModuleUtility.RegisterModule(typeof(PageInitializerModule));
        }

        void IHttpModule.Init(HttpApplication app)
        {
            app.PreRequestHandlerExecute += (sender, e) =>
            {
                var handler = app.Context.CurrentHandler;
                if (handler != null)
                {
                    string name = handler.GetType().Assembly.FullName;
                    if (!name.StartsWith("System.Web") &&
                        !name.StartsWith("Microsoft"))
                    {
                        Global.InitializeHandler(handler);
                    }
                }
            };
        }

        void IHttpModule.Dispose() { }
    }

    public class Global : HttpApplication
    {
        private static Container container;

        public static void InitializeHandler(IHttpHandler handler)
        {
            Type handlerType = handler is Page
                ? handler.GetType().BaseType
                : handler.GetType();
            container.GetRegistration(handlerType, true).Registration
                .InitializeInstance(handler);
        }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            InitializeContainer();
        }

        private void InitializeContainer()
        {
            // 1. Create a new Simple Injector container.
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            // Register a custom PropertySelectionBehavior to enable property injection.
            container.Options.PropertySelectionBehavior =
                new ImportAttributePropertySelectionBehavior();

            SimpleInjectorBootStrapper.InitializeContainer(container);

            // Register your Page classes to allow them to be verified and diagnosed.
            RegisterWebPages(container);

            // 3. Store the container for use by Page classes.
            Global.container = container;

            // 3. Verify the container's configuration.
            container.Verify();
        }

        private static void RegisterWebPages(Container container)
        {
            var pageTypes =
                from assembly in BuildManager.GetReferencedAssemblies().Cast<Assembly>()
                where !assembly.IsDynamic
                where !assembly.GlobalAssemblyCache
                from type in assembly.GetExportedTypes()
                where type.IsSubclassOf(typeof(Page))
                where !type.IsAbstract && !type.IsGenericType
                select type;

            foreach (Type type in pageTypes)
            {
                var reg = Lifestyle.Transient.CreateRegistration(type, container);
                reg.SuppressDiagnosticWarning(
                    DiagnosticType.DisposableTransientComponent,
                    "ASP.NET creates and disposes page classes for us.");
                container.AddRegistration(type, reg);
            }
        }

        class ImportAttributePropertySelectionBehavior : IPropertySelectionBehavior
        {
            public bool SelectProperty(Type implementationType, PropertyInfo property)
            {
                // Makes use of the System.ComponentModel.Composition assembly
                return typeof(Page).IsAssignableFrom(implementationType) &&
                    property.GetCustomAttributes(typeof(ImportAttribute), true).Any();
            }
        }
    }
}