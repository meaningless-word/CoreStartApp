using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStartApp
{
	public class Startup
	{
		//public IConfiguration Configuration { get; }

		//public Startup(IConfiguration configuration)
		//{
		//	Configuration = configuration;
		//}


		private IWebHostEnvironment _env;

		public Startup(IWebHostEnvironment env)
		{
			_env = env;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app/*, IWebHostEnvironment env*/)
		{
			if (_env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			// 1. ������� ���� appSettings.Staging.json ��� ����� ������������
			// 2. ������� �������� ��� �������, ��� ������ ������� ������ � ���� ������������:
			if (_env.IsDevelopment() || _env.IsStaging())
			{
				app.UseDeveloperExceptionPage();
			}


			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();


			// ��������� ������ ����� ����� ����� app.UseRouting() ��� app.UseStaticFiles(), ����� � ��������� �������� ��� ���������� ���������� � ����� ������:
			// ���������� ����� Use, ����� ������ ����������� ������ �� ���������
			app.Use(async (context, next) =>
			{
				// ������ ��� ���������� � ���
				string logMessage = $"[{DateTime.Now}]: New request to http://{context.Request.Host.Value + context.Request.Path}{Environment.NewLine}";

				// ���� �� ���� (�����-����, ���������� �������� IWebHostEnvironment)
				string logFilePath = Path.Combine(_env.ContentRootPath, "Logs", "RequestLog.txt");

				// ���������� ����������� ������ � ����
				await File.AppendAllTextAsync(logFilePath, logMessage);

				await next.Invoke();
			});

			//��������� ��������� ��� ����������� �������� � �������������� ������ Use.
			app.Use(async (context, next) =>
			{
				// ��� ����������� ������ � ������� ���������� ������� ������� HttpContext
				Console.WriteLine($"[{DateTime.Now}]: New request to http://{context.Request.Host.Value + context.Request.Path}");
				await next.Invoke();
			});

			app.UseAuthorization();

			// ��������� ��� ����������� � �������������� ������ Map, � ����� ������� ��� ������ ����������
			// ���������� ������ Page Not Found (����� ������������ ��� �������������� ��������):
			/*
			// ������� ���������� ����� Use, ����� �� ��������� �������
			app.UseEndpoints(endpoints =>
			{
				//endpoints.MapRazorPages();

				//endpoints.MapGet("/", async context => { await context.Response.WriteAsync(env.EnvironmentName); });

				endpoints.MapGet("/config", async context =>
				{
					await context.Response.WriteAsync($"App name: {env.ApplicationName}. App running configuration: {env.EnvironmentName}");
				});
			});

			app.Map("/about", About);

			// �������� ������� ������ Run
			app.Run(async (context) =>
			{
				await context.Response.WriteAsync($"Welcome to the {env.ApplicationName}!");
			});
			*/

			//��������� ��������� � ���������� ��������� ��� ������� ��������
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync($"Welcome to the {_env.ApplicationName}!");
				});
			});

			// ��� ������ �������� ����� ��������� �����������
			app.Map("/about", About);
			app.Map("/config", Config);

			// ���������� ��� ������ "�������� �� �������"
			app.Run(async (context) =>
			{
				await context.Response.WriteAsync($"Page not found");
			});
		}

		// ����������� ��������� �������:
		/// <summary>
		///  ���������� ��� �������� About
		/// </summary>
		private void About(IApplicationBuilder app)
		{
			app.Run(async context =>
			{
				await context.Response.WriteAsync($"{_env.ApplicationName} - ASP.Net Core tutorial project");
			});
		}

		/// <summary>
		///  ���������� ��� ������� ��������
		/// </summary>
		private void Config(IApplicationBuilder app)
		{
			app.Run(async context =>
			{
				await context.Response.WriteAsync($"App name: {_env.ApplicationName}. App running configuration: {_env.EnvironmentName}");
			});
		}
	}
}
