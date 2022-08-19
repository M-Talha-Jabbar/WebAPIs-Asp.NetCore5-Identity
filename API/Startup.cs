using API.Authorization.Handlers;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Repository.Data;
using Repository.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) 
        {
            Configuration = configuration;
        }
        // IConfiguration service is setup to read configuration information from all the various configuration sources (appsettings.json, appsettings.{Environment}.json, secrets.json, Environment Variables, Command-line Arguments) in asp.net core.
        // Please note that, if you have a configuration setting with the same key in multiple configuration sources, the later configuration sources override the earlier configuration sources. 

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AppDB"))
            );

            // configuring ASP.NET Core Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options => // We are using the IdentityOptions object to configure PasswordOptions.
            {
                // We could also use this IdentityOptions object to configure: UserOptions, SignInOptions, LockoutOptions, TokenOptions, StoreOptions, ClaimsIdentityOptions.
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders(); // Adds the default token providers used to generate tokens for reset passwords, change email and change telephone number operations, and for two factor authentication token generation.

            services.AddControllers();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/api/Administrator/AccessDenied");
            });

            services.AddIdentityServices();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
