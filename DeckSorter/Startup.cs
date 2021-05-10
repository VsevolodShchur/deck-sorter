using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeckSorter.Models;

namespace DeckSorter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var shufflerClassName = Configuration.GetChildren().First(item => item.Key == "Shuffler").Value;
            var deckStorageClassName = Configuration.GetChildren().First(item => item.Key == "DeckStorage").Value;
            var assemblyName = typeof(Deck).Assembly.GetName().Name;
            var namespaceName = typeof(Deck).Namespace;

            var shufflerType = Type.GetType($"{namespaceName}.{shufflerClassName}, {assemblyName}");
            var deckStorageType = Type.GetType($"{namespaceName}.{deckStorageClassName}, {assemblyName}");

            services.AddSingleton(typeof(IShuffler), shufflerType);
            services.AddSingleton(typeof(IDeckStorage), deckStorageType);
            services.AddControllers()
                    .AddJsonOptions(option =>
                        option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
