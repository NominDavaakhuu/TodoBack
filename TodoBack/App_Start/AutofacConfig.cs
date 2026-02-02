using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Reflection;
using System.Web.Http;
using TodoBack.Data;
using TodoBack.Models;
using TodoBack.Repos.Implementations;
using TodoBack.Repos.Interfaces;
using TodoBack.Services.Implementations;
using TodoBack.Services.Interfaces;

namespace TodoBack.App_Start
{
    public static class AutofacConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            // register API controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // EF DbContext - per request
            builder.RegisterType<TodoDbContext>()
                .AsSelf()
                .InstancePerRequest();

            // repository
            builder.RegisterType<UserRepository>()
                .As<IUserRepository>()
                .InstancePerRequest();

            // auth service
            builder.RegisterType<AuthService>()
                .As<IAuthService>()
                .InstancePerRequest();

            builder.RegisterType<PasswordHasher>()
                .As<IPasswordHasher>()
                .InstancePerRequest();


            // Register repository + service
            builder.RegisterType<TodoItemRepository>()
                   .As<ITodoItemRepository>()
                   .InstancePerRequest();


            builder.RegisterType<TodoItemService>()
                           .As<ITodoItemService>()
                           .InstancePerRequest();


            //build the container
            var container = builder.Build();

            //dep resolver
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}