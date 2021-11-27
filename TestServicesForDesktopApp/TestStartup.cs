using CommonData;
using CommonData.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TestServicesForDesktopApp
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MeetingApplicationContext>(options =>
                options.UseInMemoryDatabase("MeetingOrganiserTest"));

            services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<MeetingApplicationContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 3;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var dbContext = serviceProvider.GetRequiredService<MeetingApplicationContext>();
            dbContext.Database.EnsureCreated();
            dbContext.Organisations.AddRange(MeetingOrganiserIntegrationTest.OrganisationData);
            dbContext.Events.AddRange(MeetingOrganiserIntegrationTest.EventData);
            dbContext.SaveChanges();

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            
            var adminRole = new IdentityRole<int>("administrator");

            var result1 = userManager.CreateAsync(
                MeetingOrganiserIntegrationTest.UserData.ElementAt(0),
                MeetingOrganiserIntegrationTest.AdminPassword).Result;
            var result2 = roleManager.CreateAsync(adminRole).Result;
            var result3 = userManager.AddToRoleAsync(MeetingOrganiserIntegrationTest.UserData.ElementAt(0), adminRole.Name).Result;

            var userRole = new IdentityRole<int>("user");

            var result4 = userManager.CreateAsync(
                MeetingOrganiserIntegrationTest.UserData.ElementAt(1),
                MeetingOrganiserIntegrationTest.UserPassword).Result;
            var result5 = roleManager.CreateAsync(userRole).Result;
            var result6 = userManager.AddToRoleAsync(MeetingOrganiserIntegrationTest.UserData.ElementAt(1), userRole.Name).Result;
        }
    }
}
