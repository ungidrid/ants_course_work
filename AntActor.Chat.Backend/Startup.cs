using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AntActor.Chat.Backend.Ants;
using AntActor.Chat.Backend.DAL;
using AntActor.Chat.Backend.Hubs;
using AntActor.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace AntActor.Chat.Backend
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(basePath: env.ContentRootPath);

            if (env.IsDevelopment())
            {
                configuration.AddJsonFile("appsettings.json", false, true);
            }

            if (env.IsProduction())
            {
                configuration.AddJsonFile("appsettings.Production.json", false, true);
            }

            Configuration = configuration.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(_ => new MessageRepository(Configuration.GetConnectionString("AntChat")));
            services.AddAntActor();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat-hub");
            });
        }
    }
}
