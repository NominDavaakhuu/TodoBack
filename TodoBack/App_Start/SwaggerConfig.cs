using System.Web.Http;
using Swashbuckle.Application;

namespace TodoBack.App_Start
{
    public static class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "TodoBack");
                })
                .EnableSwaggerUi(c =>
                {
                });
        }
    }
}