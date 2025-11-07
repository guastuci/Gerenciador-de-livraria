using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using GerenciadorDeLivraria.Data;
using GerenciadorDeLivraria.Domain;
using GerenciadorDeLivraria.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using System;

namespace GerenciadorDeLivraria
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(opt =>
                 opt.UseSqlite(Configuration.GetConnectionString("Default")));

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GerenciadorDeLivraria API", Version = "v1" });

                var xmlName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env, AppDbContext db)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/problem+json";

                    var feature = context.Features.Get<IExceptionHandlerFeature>();
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Erro inesperado no servidor",
                        Detail = env.IsDevelopment() ? feature?.Error.ToString() : "Ocorreu uma exceção não tratada."
                    });
                });
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GerenciadorDeLivraria v1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            if (!db.Books.Any())
            {
                db.Books.AddRange(
                    Domain.Book.Create("Dom Casmurro", "Machado de Assis", Genre.Ficcao, 39.9m, 10),
                    Domain.Book.Create("O Alienista", "Machado de Assis", Genre.Ficcao, 29.9m, 5),
                    Domain.Book.Create("Clean Code", "Robert C. Martin", Genre.Tecnologia, 199.9m, 7)
                );
                db.SaveChanges();
            }
        }
    }
}
