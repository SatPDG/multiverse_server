using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AuthTest.API.Middleware;
using MultiverseServer.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace MultiverseServer
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
            services.AddControllers();
            services.AddTokenAuthentication(Configuration);

            string sqlServerAddress = Configuration.GetSection("SQLDatabase").GetSection("address").Value;
            string sqlServerPort = Configuration.GetSection("SQLDatabase").GetSection("port").Value;
            string sqlServerUsername = Configuration.GetSection("SQLDatabase").GetSection("username").Value;
            string sqlServerPassword = Configuration.GetSection("SQLDatabase").GetSection("password").Value;
            string mySqlConnectionStr = "server=" + sqlServerAddress + ";user=" + sqlServerUsername + ";database=multiverse;port=" + sqlServerPort + ";password=" + sqlServerPassword;
            services.AddDbContext<MultiverseDbContext>(options => options
                .UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr), b => b.UseNetTopologySuite())//ServerVersion.AutoDetect(mySqlConnectionStr))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
            //services.AddDbContext<MultiverseDbContext>(options => options.UseSqlServer(mySqlConnectionStr, x => x.UseNetTopologySuite()).EnableSensitiveDataLogging());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
