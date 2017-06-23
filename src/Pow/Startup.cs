using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pow.Data;
using Pow.Models;
using Pow.Services;
using Microsoft.AspNetCore.Mvc;
using Pow.Data.Entities;
using Microsoft.AspNetCore.SpaServices.Webpack;

namespace Pow
{
	public class Startup
	{
		public Startup( IHostingEnvironment env )
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath( env.ContentRootPath )
				.AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true )
				.AddJsonFile( $"appsettings.{env.EnvironmentName}.json", optional: true );

			if( env.IsDevelopment() )
			{
				// For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets<Startup>();
			}

			builder.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices( IServiceCollection services )
		{
			// Add framework services.
			services.AddDbContext<ApplicationDbContext>( options =>
				 options.UseSqlServer( Configuration.GetConnectionString( "DefaultConnection" ) ) );

			services.AddIdentity<User, Role>()
				.AddEntityFrameworkStores<ApplicationDbContext, Guid>()
				.AddDefaultTokenProviders();

			services.AddMvc( options =>
			{
				options.SslPort = int.Parse( Configuration["SslPort"] );
				options.Filters.Add( new RequireHttpsAttribute() );
			} );

			services.AddDistributedMemoryCache();
			services.AddSession();

			// Add application services.
			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
		{
			loggerFactory.AddConsole( Configuration.GetSection( "Logging" ) );
			loggerFactory.AddDebug();

			if( env.IsDevelopment() )
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
				//app.UseBrowserLink();
				app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
				{
					HotModuleReplacement = true,
					ReactHotModuleReplacement = true
				} );
			}
			else
			{
				app.UseExceptionHandler( "/Home/Error" );
			}

			app.UseStaticFiles();

			app.UseIdentity();

			// Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

			app.UseGoogleAuthentication( new GoogleOptions
			{
				ClientId = Configuration["Authentication:Google:ClientId"],
				ClientSecret = Configuration["Authentication:Google:ClientSecret"]
			} );

			app.UseFacebookAuthentication( new FacebookOptions
			{
				AppId = Configuration["Authentication:Facebook:AppId"],
				AppSecret = Configuration["Authentication:Facebook:AppSecret"]
			} );

			app.UseMicrosoftAccountAuthentication( new MicrosoftAccountOptions
			{
				ClientId = Configuration["Authentiction:Microsoft:ClientId"],
				ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"]
			} );

			app.UseTwitterAuthentication( new TwitterOptions
			{
				ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"],
				ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"]
			} );

			app.UseCookieAuthentication();

			app.UseMvc( routes =>
			{
				routes.MapRoute
				(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}"
				);

				routes.MapSpaFallbackRoute
				(
					name: "spa-fallback",
					defaults: new { controller = "Home", action = "Index" }
				);
			} );
		}
	}
}