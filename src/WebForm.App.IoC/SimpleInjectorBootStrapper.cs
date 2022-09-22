using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebForm.Data.Interfaces;
using WebForm.Data.Repository;

namespace WebForm.App.IoC
{
    public static class SimpleInjectorBootStrapper
    {
        public static void InitializeContainer(Container container)
        {
            // 2. Configure the container (register)
            container.Register<IRepository, Repository>(Lifestyle.Scoped);
        }        
    }
}
