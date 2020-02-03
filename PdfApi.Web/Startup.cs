using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Resources;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PdfApi.Data;
using PdfApi.Handling.Queries;
using PdfApi.Repository;
using PdfApi.Web.Infrastructure.Filters;
using PdfApi.Web.Infrastructure.Middleware;
using System;
using System.IO;
using System.Reflection;

namespace PdfApi.Web
{
    public class Startup
    {
        private readonly string _allowAllCorsPolicy = "AllowAll";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.Add<ValidationFilter>())
                 .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(typeof(Startup).Assembly));

            services.AddCors(options =>
                options.AddPolicy(_allowAllCorsPolicy, builder =>     
                        builder
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod()));

            services.AddDbContextPool<PdfApiDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("PfdApiDbConnectionString")));

            services.AddScoped<IPdfFileRepository, PdfFileRepository>();
            services.AddMediatR(typeof(GetPdfFileListQuery).Assembly);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pdf Document API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PdfApiDbContext context)
        {          
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Pdf Document API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors(_allowAllCorsPolicy);
            app.UseExceptionHandling();

            context.Database.EnsureCreated();

            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            ValidatorOptions.LanguageManager.Enabled = false;
        }
    }
}
