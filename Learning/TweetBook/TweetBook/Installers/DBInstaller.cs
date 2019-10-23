using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TweetBook.Services;

namespace TweetBook.Installers
{
    public class DBInstaller : IInstaler
    {
        public void InstallServices(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<Data.DataContext>(options =>
             options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));
                    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                        .AddEntityFrameworkStores<Data.DataContext>();


            services.AddScoped<IPostService, PostService>();
        }
    }
}
