using System;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using masz.data;
using masz.Models;
using masz.Services;
using masz.Logger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using masz.Middlewares;
using masz.Plugins;
using System.Linq;
using Scrutor;

namespace masz
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
            services.AddDbContext<DataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddScoped<IDatabase, Database>();
            services.AddScoped<ITranslator, Translator>();
            services.AddSingleton<IDiscordAnnouncer, DiscordAnnouncer>();
            services.AddSingleton<IFilesHandler, FilesHandler>();
            services.AddSingleton<INotificationEmbedCreator, NotificationEmbedCreator>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IDiscordBot, DiscordBot>();
            services.AddSingleton<IDiscordAPIInterface, DiscordAPIInterface>();
            services.AddSingleton<IIdentityManager, IdentityManager>();
            services.AddSingleton<IPunishmentHandler, PunishmentHandler>();
            services.AddSingleton<IEventHandler, Services.EventHandler>();
            services.AddSingleton<IScheduler, Scheduler>();

            // Plugin
            // ######################################################################################################
            if (String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_CUSTOM_PLUGINS")))
            {
                Console.WriteLine("########################################################################################################");
                Console.WriteLine("ENABLED CUSTOM PLUGINS!");
                Console.WriteLine("This might impact the performance or security of your masz instance!");
                Console.WriteLine("Use this only if you know what you are doing!");
                Console.WriteLine("For support and more information, refer to the creator or community of your plugin!");
                Console.WriteLine("########################################################################################################");

                services.Scan(scan => scan
                    .FromAssemblyOf<IBasePlugin>()
                    .AddClasses(classes => classes.InNamespaces("masz.Plugins"))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime());
            }
            // ######################################################################################################

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/api/v1/login";
                    options.LogoutPath = "/api/v1/logout";
                    options.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0);
                    options.Cookie.MaxAge = new TimeSpan(7, 0, 0, 0);
                    options.Cookie.Name = "masz_access_token";
                    options.Cookie.HttpOnly = false;
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.Headers["Location"] = context.RedirectUri;
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                })
                .AddDiscord(options =>
                {
                    options.ClientId = Configuration.GetSection("InternalConfig").GetValue<string>("DiscordClientId");
                    options.ClientSecret = Configuration.GetSection("InternalConfig").GetValue<string>("DiscordClientSecret");
                    options.Scope.Add("guilds");
                    options.Scope.Add("identify");
                    options.SaveTokens = true;
                    options.Prompt = "none";
                    options.AccessDeniedPath = "/oauthfailed";
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.CorrelationCookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                    options.CorrelationCookie.HttpOnly = false;
                });


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Tokens", x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("InternalConfig").GetValue<string>("DiscordBotToken"))),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder("Cookies", "Tokens")
                        .RequireAuthenticatedUser()
                        .Build();
                });

            // services.AddCors(o => o.AddPolicy("AngularDevCors", builder =>
            // {
            //     builder.WithOrigins("http://127.0.0.1:4200")
            //         .AllowAnyMethod()
            //         .AllowAnyHeader()
            //         .AllowCredentials();
            // }));

            services.Configure<InternalConfig>(Configuration.GetSection("InternalConfig"));

            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new CustomLoggerProvider());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<APIExceptionHandlingMiddleware>();

            //  app.UseCors("AngularDevCors");

            app.UseIpRateLimiting();

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<DataContext>().Database.Migrate();
            }

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<IPunishmentHandler>().StartTimer();
                scope.ServiceProvider.GetService<IScheduler>().StartTimers();
                scope.ServiceProvider.GetService<IDiscordBot>().Start();
                if (String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_CUSTOM_PLUGINS")))
                {
                    scope.ServiceProvider.GetServices<IBasePlugin>().ToList().ForEach(x => x.Init());
                }
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
