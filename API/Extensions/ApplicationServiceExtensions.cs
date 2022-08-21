using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Data;
using Service.Contracts;
using Service.Models;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AppDB"))
            );

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/api/Administrator/AccessDenied");
            });

            services.AddControllers();

            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.AddScoped<IEmailSenderService, EmailSenderService>();

            return services;
        }
    }
}
