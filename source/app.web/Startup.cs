using app.data;
using app.domain.Enums;
using app.domain.Mappings;
using app.service;
using app.service.Model.Mail;
using app.web.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Net;

namespace app.web
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
            services.Configure<MailConfigOptions>(options => Configuration.GetSection("MailConfig").Bind(options));
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            //repos
            services.AddSingleton<IEntityRepository>(provider => new EntityRepository(Configuration["ConnectionStrings:connectionString"].ToString()));
            services.AddSingleton<IPromoCodeRepository>(provider => new PromoCodeRepository(Configuration["ConnectionStrings:connectionString"].ToString()));

            //services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddSingleton<ICipherService, CipherService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IEntityService, EntityService>();
            services.AddSingleton<IAntiForgeryCookieService, AntiForgeryCookieService>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddSingleton<IOptionService, OptionService>();
            services.AddSingleton<ISectionService, SectionService>();

            services.AddSingleton<ImageFileService>();
            services.AddSingleton<VideoFileService>();

            //services.AddSingleton<IFileService, ImageFileService>();
            //services.AddSingleton<IFileService, VideoFileService>();

            services.AddSingleton<Func<EnumFileType, IFileService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case EnumFileType.Image:
                        return serviceProvider.GetService<ImageFileService>();
                    case EnumFileType.Video:
                        return serviceProvider.GetService<VideoFileService>();
                    default:
                        return null;
                }
            });


            Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins("http://localhost:4200") //Note:  The URL must be specified without a trailing slash (/).
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            //For Redis
            services.AddSession();
            //services.AddDistributedRedisCache(option =>
            //{
            //    option.Configuration = Configuration["Redis:Host"] + ":" + Configuration["Redis:Port"];
            //    option.InstanceName = Configuration["Redis:InstanceName"];
            //});

            services.AddDataProtection().
                                        UseCryptographicAlgorithms(new Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.AuthenticatedEncryptorConfiguration()
                                        {
                                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
                                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                                        });

            services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddMvc().AddXmlSerializerFormatters();

            services.AddMvcCore(options =>
                {
                    options.RespectBrowserAcceptHeader = true;

                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.Filters.Add(typeof(CustomExceptionFilterAttribute));
                })
                .AddViewLocalization(
                                        LanguageViewLocationExpanderFormat.Suffix,
                                        opts =>
                                        {
                                            opts.ResourcesPath = "Resources";
                                        })
                .AddDataAnnotationsLocalization()
                .AddRazorPages(options =>
                {
                    options.AllowAreas = true;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler(
                options =>
                {
                    options.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "text/html";
                            var ex = context.Features.Get<IExceptionHandlerFeature>();
                            if (ex != null)
                            {
                                var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace}";
                                await context.Response.WriteAsync(err).ConfigureAwait(false);
                            }
                        });
                }
            );

            //Localization
            var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("az-Latn-AZ") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });


            //cookie
            //app.UseCookiePolicy(new CookiePolicyOptions
            //{
            //    HttpOnly = HttpOnlyPolicy.Always,
            //    Secure = CookieSecurePolicy.Always,
            //    MinimumSameSitePolicy = SameSiteMode.None
            //});

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.Strict,
                Secure = CookieSecurePolicy.SameAsRequest
            });


            app.UseExceptionHandler("/Home/Error");

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                //app.UseExceptionHandler("/Error");
                //app.UseStatusCodePagesWithReExecute("/Errors/{0}");  // I use this version
                //after ssl
                //app.UseHsts();
            }

            var forwardingOptions = new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            forwardingOptions.KnownNetworks.Clear(); //its loopback by default
            forwardingOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardingOptions);

            //app.UseIISPlatformHandler();
            //app.UseDefaultFiles();
            //app.UseFileServer(true);
            app.UseStatusCodePages();
            app.UseCookiePolicy();
            app.UseStaticFiles();
            app.UseSession();

            //after ssl
            //app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                context.Request.Scheme = "http";
                context.Response.Headers.Add("X-Xss-Protection", "1");
                await next();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "area",
                    template: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                        name: "DefaultApi",
                        template: "api/{controller}/{id}");
            });
        }
    }
}
