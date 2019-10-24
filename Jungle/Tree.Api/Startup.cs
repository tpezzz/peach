using Tree.Api.Model;
using Tree.Api.OAuth;
using Autofac;
using Autofac.Extras.NLog;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Diagnostics;
using System.Reflection;
using Tree.Api.Filter;
using System.Web.Http;

[assembly: OwinStartup(typeof(Tree.Api.Startup))]

namespace Tree.Api {
    public partial class Startup {
        private IContainer container;

        public void Configuration(IAppBuilder app) {
            Map.MapperRegister.RegisterMaps();
            ExpressMapper.Mapper.Compile();

            var config = new HttpConfiguration();

            app.Use<ODataResponseFormatterMiddleware>();
            app.UseCors(CorsOptions.AllowAll);

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("api", "api/{controller}/{key}", new { key = RouteParameter.Optional });

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            var displayedVersion = GetDisplayedApiVersion();

            config.ConfigureSwagger(displayedVersion);

            config.ConfigureOData();

            var tokenFormat = new TicketDataFormat(app.CreateDataProtector(
              typeof(OAuthAuthorizationServerMiddleware).Namespace,
              "Access_Token", displayedVersion));

            oAuthBearerOptions = new OAuthBearerAuthenticationOptions {
                Provider = new OAuthBearerAuthenticationProvider(),
                AccessTokenFormat = tokenFormat
            };

            container = GetIocContainer(config);

            ConfigureOAuth(app, displayedVersion);

            app.UseAutofacMiddleware(container);
            app.UseWebApi(config);
            app.UseAutofacWebApi(config);

            config.ConfigureJsonFormatter(container);

            config.EnsureInitialized();
        }

        private IContainer GetIocContainer(HttpConfiguration config) {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new IoC.Module());
            builder.RegisterWebApiFilterProvider(config);

            builder.Register(context => oAuthBearerOptions).AsSelf();
            builder.RegisterType<AuthorizationServerProvider>().AsSelf();
            builder.RegisterType<RefreshTokenProvider>().AsSelf();

            return builder.Build();
        }

        private string GetDisplayedApiVersion() {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(currentAssembly.Location);
            return fileVersionInfo.FileVersion.Replace(".", "-");
        }
    }

    internal static class JsonFormatterConfiguration {
        internal static void ConfigureJsonFormatter(this HttpConfiguration config, IContainer container) {
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new DateTimeOffsetToStringCoverter());
            config.Formatters.JsonFormatter.SerializerSettings.Error += delegate(object sender, ErrorEventArgs args) {
                if (args.CurrentObject == args.ErrorContext.OriginalObject) {
                    container.Resolve<ILogger>().Error(args.ErrorContext.Error);
                }
            };

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }
    }
}