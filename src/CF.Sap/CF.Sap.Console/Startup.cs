using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CF.Sap.ConsoleTask
{
    public class Startup
    {
        internal class HangfireConfig
        {
            public static void Register(IAppBuilder app)
            {
                GlobalConfiguration.Configuration
                                   .UseSqlServerStorage("Data Source=10.1.1.181;Initial Catalog=Hangfire;User ID=sa;Password=Chen@full", new SqlServerStorageOptions
                                   {
                                       CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                       SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                       QueuePollInterval = TimeSpan.Zero,
                                       UseRecommendedIsolationLevel = true,
                                       DisableGlobalLocks = true, // Migration to Schema 7 is required
                                       PrepareSchemaIfNecessary = true,
                                   })
                                   .UseConsole();

                app.UseHangfireDashboard("/hangfire");
                var jobServerOptions = new BackgroundJobServerOptions()
                {
                    Queues = new[] { "kpi" }
                };
                app.UseHangfireServer(jobServerOptions);
            }
        }
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            var config = new HttpConfiguration();
            HangfireConfig.Register(app);
            config.Routes.MapHttpRoute("DefaultApi",
                                       "api/{controller}/{id}",
                                       new { id = RouteParameter.Optional }
                                      );

            app.UseWelcomePage("/");
            app.UseWebApi(config);
            app.UseErrorPage();
        }
    }
}
