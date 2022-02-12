using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Events.V2;
using TweetStream.Core.Abstractions;
using TweetStream.Core.Configuration;
using TweetStream.Core.Handlers;
using TweetStream.Core.Services;

namespace TweetStream.Web
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

            services.AddControllers();
            services.Configure<AppCredentials>(Configuration.GetSection(nameof(AppCredentials)));
            services.Configure<List<Rule>>(Configuration.GetSection(nameof(Rule)));
            services.Configure<Settings>(Configuration.GetSection(nameof(Settings)));
            services.AddSingleton<TweetConfiguration>();

            services.AddHostedService<EventListener>();

            services.AddSingleton<ITweetQueue<FilteredStreamTweetV2EventArgs>, EventQueue>();

            services.AddTransient<ITweetHandler, FollowerQuotaHandler>();
            services.AddHostedService<TweetWorker>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TweetStream.Web", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TweetStream.Web v1"));
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
