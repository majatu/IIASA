using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IIASA.Api.Global.Middleware;
using IIASA.Api.Global.Response;
using IIASA.Common;
using IIASA.Services.Context;
using IIASA.Services.Context.Factory;
using IIASA.Services.Context.Factory.Interfaces;
using IIASA.Services.Core;
using IIASA.Services.Core.Interfaces;
using IIASA.Services.Providers;
using IIASA.Services.Providers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace IIASA
{
    public class Startup
    {
        private IContainer ApplicationContainer { get; set; }
        private IHostingEnvironment HostingEnvironment { get; set; }

        private string _imgPath { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            _imgPath = Path.Combine(HostingEnvironment.ContentRootPath, "img");

            Directory.CreateDirectory(_imgPath);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISOptions>(options => { options.ForwardClientCertificate = false; });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<GzipCompressionProviderOptions>(options => {
                options.Level = CompressionLevel.Fastest;
            });

            services.AddCors();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext => new ValidationFailedResultModel(actionContext.ModelState);
            });

            #region swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "IIASA API",
                    Version = "v1"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                xmlFile = "IIASA.Api.xml";
                xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                xmlFile = "IIASA.Common.xml";
                xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                xmlFile = "IIASA.Services.xml";
                xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.IgnoreObsoleteActions();

                c.DescribeAllEnumsAsStrings();
            });

            #endregion

            var builder = new ContainerBuilder();

            #region autofac

            {
                // For NLog
                builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().InstancePerLifetimeScope();
                builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().InstancePerLifetimeScope();

                builder.Register(c => new ConnectionStringProvider
                {
                    ConnStringDefault = Configuration.GetValue<string>("Settings:ConnectionStrings:Default")
                }).As<IConnectionStringProvider>().InstancePerLifetimeScope();

                builder.Register(c => new PathsProvider
                {
                    ImagePath = _imgPath
                }).As<IPathsProvider>().InstancePerLifetimeScope();

                builder.RegisterType<ExtendedDataContextFactory>().As<IExtendedDataContextFactory>().InstancePerLifetimeScope();
                builder.Register(c => c.Resolve<IExtendedDataContextFactory>().CreateContext()).As<ExtendedDbContext>().InstancePerLifetimeScope();

                builder.RegisterType<ImageService>().As<IImageService>().InstancePerLifetimeScope();

                // Populate the services.
                builder.Populate(services);
            }

            #endregion

            // Build the container.
            ApplicationContainer = builder.Build();

            // Create and return the service provider.
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            GlobalLogging.LoggerFactory = loggerFactory;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseMiddleware(typeof(HandleErrorMiddleware));
            }
            else
            {
                app.UseExceptionHandler("/error");

                // Once a supported browser receives this header that browser will prevent any communications from being sent over HTTP to the specified domain and will instead send all communications over HTTPS. 
                // It also prevents HTTPS click through prompts on browsers.
                //app.UseHsts();

                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }

            app.UseResponseCompression();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(_imgPath),
                RequestPath = new PathString("/img")
            });

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            //redirect to swagger by default
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "IIASA API V1");
            });
        }
    }
}
