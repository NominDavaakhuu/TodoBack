using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using TodoBack.App_Start;

namespace TodoBack
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // if you're using Autofac, configure it with the same HttpConfiguration
            AutofacConfig.Configure(GlobalConfiguration.Configuration);

            // register swagger (after WebApi config is set up)
            SwaggerConfig.Register();
        }

        protected void Application_PostAuthorizeRequest()
        {
            // Enable Session for Web API
            if (HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api"))
            {
                HttpContext.Current.SetSessionStateBehavior(
                    System.Web.SessionState.SessionStateBehavior.Required);
            }
        }

    }
}
