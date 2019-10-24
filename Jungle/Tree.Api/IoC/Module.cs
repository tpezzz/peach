using Tree.Api.Filter;
using Tree.Api.Map.CommandMap;
using Tree.Api.Map.QueryMap;
using Tree.Domain.IoC;
using Autofac;
using Autofac.Extras.NLog;
using Autofac.Integration.WebApi;
using FluentValidation;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Linq;

namespace Tree.Api.IoC {
    public class Module : Autofac.Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<LoggingFilter>().AsWebApiActionFilterFor<ApiController>();
            builder.RegisterType<ExceptionFilter>().AsWebApiExceptionFilterFor<ApiController>();
            builder.RegisterType<UnitOfWorkFilter>().AsWebApiActionFilterFor<ApiController>();

            RegisterCommandMaps(builder);
            RegisterFilterMaps(builder);

            builder.RegisterTypes(ThisAssembly.GetTypes())
                .Where(x => x.IsImplementationOf<IValidator>())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>().SingleInstance();

            builder.RegisterModule<NLogModule>();
            builder.RegisterModule<SimpleNLogModule>();
            builder.RegisterModule(new App.Administration.IoC.Module());
            builder.RegisterModule(new App.Authorization.IoC.Module());
            builder.RegisterModule(new App.Measurement.IoC.Module());
            builder.RegisterModule(new Repository.IoC.Module());
        }

        private void RegisterFilterMaps(ContainerBuilder builder) {
            builder.RegisterType<UserQueryFilter>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }

        private void RegisterCommandMaps(ContainerBuilder builder) {
            var iCommandMapperType = typeof(ICommandMapper<,>);
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(iCommandMapperType)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}