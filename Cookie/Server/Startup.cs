using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Cookie.Server.Model;

namespace Cookie.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // This method gets called by the runtime. Use this method to add services to the container.
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            services.AddControllersWithViews();
            services.AddRazorPages();

            // Which Authentication Scheme for 
            // maintaining the state of the user - Cookie Authentication is chosen
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }
            ).AddCookie(options => { options.LoginPath = "/api/Sportschool/ongeautoriseerd"; });

            // Registreer de MemoryModel-implementatie van interface IModel
            // Voor de Memory model wil je niet dat de List 
            // elke keer weer wordt geïnitialiseerd naar de beginsituatie 
            // bij elke HTTP-request, dus daarom een SingleTon:
            services.AddSingleton<IModel, MemoryModel>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            // Authentication en Authorization MiddleWare
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
