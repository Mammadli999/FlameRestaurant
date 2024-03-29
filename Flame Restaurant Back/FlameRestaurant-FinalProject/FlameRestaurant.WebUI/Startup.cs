using FlameRestaurant.Application.AppCode.Extensions;
using FlameRestaurant.Application.AppCode.Providers;
using FlameRestaurant.Application.AppCode.Services;
using FlameRestaurant.Domain.AppCode.Providers;
using FlameRestaurant.Domain.Models.DbContexts;
using FlameRestaurant.Domain.Models.Entities.Membership;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlameRestaurant.WebUI
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(cfg =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                        .RequireAuthenticatedUser()
                                        .Build();
                cfg.Filters.Add(new AuthorizeFilter(policy));
                cfg.ModelBinderProviders.Insert(0, new BooleanBinderProvider());
            });
            services.AddFluentValidationAutoValidation();
            services.AddDbContext<FlameRestaurantDbContext>(cfg =>
            {
                cfg.UseSqlServer(configuration["ConnectionStrings:cString"]);
            })
                .AddIdentity<FlameRestaurantUser, FlameRestaurantRole>()
                .AddEntityFrameworkStores<FlameRestaurantDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(cfg =>
            {
                cfg.Password.RequireDigit = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 3;

                cfg.User.RequireUniqueEmail = true;
                cfg.SignIn.RequireConfirmedEmail = true;
                cfg.Lockout.MaxFailedAccessAttempts = 3;
                cfg.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 3, 0);
            });

            services.ConfigureApplicationCookie(cfg =>
            {

                cfg.LoginPath = "/signin.html";
                cfg.AccessDeniedPath = "/accessdenied.html";
                cfg.ExpireTimeSpan = new TimeSpan(0, 50, 0);
                cfg.Cookie.Name = "FlameRestaurant";
            });

            services.AddAuthentication();
            services.AddAuthorization(cfg =>
            {
                foreach (var policyName in Extension.policies)
                {
                    cfg.AddPolicy(policyName, p =>
                    {
                        p.RequireAssertion(handler =>
                        {
                            return handler.User.IsInRole("sa")
                            || handler.User.HasClaim(policyName, "1");
                        });
                    });
                }
            });

            services.AddScoped<UserManager<FlameRestaurantUser>>();
            services.AddScoped<SignInManager<FlameRestaurantUser>>();
            services.AddScoped<RoleManager<FlameRestaurantRole>>();



            services.AddRouting(cfg =>
            {
                cfg.LowercaseUrls = true;
            });
            services.Configure<EmailServiceOptions>(cfg =>
            {
                configuration.GetSection("emailAccount").Bind(cfg);
            });
            services.AddSingleton<EmailService>();
            services.Configure<CryptoServiceOptions>(cfg =>
            {
                configuration.GetSection("cryptograpy").Bind(cfg);
            });
            services.AddSingleton<CryptoService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IClaimsTransformation, AppClaimProvider>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("FlameRestaurant."));
            services.AddMediatR(assemblies.ToArray());
            services.AddValidatorsFromAssemblies(assemblies);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<FlameRestaurantRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.SeedData();
            app.UseHttpsRedirection();
            app.SeedMembership();
            app.UseStaticFiles();
            FlameRestaurantDbSeed.SeedUserRole(roleManager);
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(cfg =>
            {
                cfg.MapControllerRoute(
                    name: "default-signin",
                    pattern: "signin.html",
                    defaults: new
                    {
                        area = "",
                        controller = "account",
                        action = "signin"
                    });
                cfg.MapControllerRoute(
                    name: "default-accessdenied",
                    pattern: "accessdenied.html",
                    defaults: new
                    {
                        area = "",
                        controller = "account",
                        action = "accessdenied"
                    });
                cfg.MapControllerRoute(
                    name: "default-register",
                    pattern: "register.html",
                    defaults: new
                    {
                        area = "",
                        controller = "account",
                        action = "register"
                    });

                cfg.MapAreaControllerRoute("defaultAdmin", "admin", "admin/{controller=reservations}/{action=index}/{id?}");
                cfg.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
