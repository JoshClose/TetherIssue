﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Pow
{
	public class Program
	{
		public static void Main( string[] args )
		{
			var host = new WebHostBuilder()
				.CaptureStartupErrors( true )
				.UseSetting( "detailedErrors", "true" )
				.UseKestrel()
				.UseContentRoot( Directory.GetCurrentDirectory() )
				.UseIISIntegration()
				.UseStartup<Startup>()
				.UseApplicationInsights()
				.Build();

			host.Run();
		}
	}
}