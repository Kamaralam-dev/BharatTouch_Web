using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using BharatTouch.CommonHelper;
using DataAccess;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
using BharatTouch.CommonHelper;


[assembly: OwinStartup(typeof(BharatTouch.App_Start.Startup))] // This line tells OWIN to use the Startup class

namespace BharatTouch.App_Start
{
    public class Startup
    {
        ScheduleEmailManager _emailManager = new ScheduleEmailManager();
        public void Configuration(IAppBuilder app)
        {

            GlobalConfiguration.Configuration
                .UseSqlServerStorage("SqlConn");

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AllowAuthorizationFilter() }
            });

            

            BackgroundJobServerOptions options = new BackgroundJobServerOptions();
            app.UseHangfireServer(options);

            //RecurringJob.AddOrUpdate("Profile-completion-Email",
            //    () => _emailManager.SendProfileCompletionEmails(), // Target method to execute
            //    Cron.Daily(10));  // call daily at 11am

            //RecurringJob.AddOrUpdate("Send_Failed_Emails",
            //    () => _emailManager.SendFailedEmails(), // Target method to execute
            //    Cron.Hourly());  // call every hour
        }

    }

}

public class AllowAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return IsAdmin();
    }

    private bool IsAdmin()
    {
        var userType = Utility.GetCookie("IsAdmin_admin");
        if (!string.IsNullOrWhiteSpace(userType))
        {
            if (userType.NullToString() == "True")
                return true;
            else
                return false;
        }
        else
            return false;
    }
}