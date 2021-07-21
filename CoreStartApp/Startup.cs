using CoreStartApp.Middlewares;
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
		public IConfiguration Configuration { get; }
		private readonly IWebHostEnvironment _env;

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			_env = env;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			Console.WriteLine($"Launching project from: {_env.ContentRootPath}");

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

			// 1. Добавим файл appSettings.Staging.json для новой конфигурации
			// 2. Добавим проверку при запуске, что проект запущен именно в этой конфигурации:
			if (_env.IsDevelopment() || _env.IsStaging())
			{
				app.UseDeveloperExceptionPage();
			}


			app.UseHttpsRedirection();

			// Поддержка статических файлов
			app.UseStaticFiles();

			app.UseRouting();

			// Доработайте ваш LoggingMiddleware так, чтобы логирование в файл тоже происходило в нём.
			/*
			// Разместим данный метод сразу после app.UseRouting() или app.UseStaticFiles(), чтобы в конвейере запросов наш обработчик срабатывал в самом начале:
			// Используем метод Use, чтобы запрос передавался дальше по конвейеру
			app.Use(async (context, next) =>
			{
				// Строка для публикации в лог
				string logMessage = $"[{DateTime.Now}]: New request to http://{context.Request.Host.Value + context.Request.Path}{Environment.NewLine}";

				// Путь до лога (опять-таки, используем свойства IWebHostEnvironment)
				string logFilePath = Path.Combine(_env.ContentRootPath, "Logs", "RequestLog.txt");

				// Используем асинхронную запись в файл
				await File.AppendAllTextAsync(logFilePath, logMessage);

				await next.Invoke();
			});
			*/

			// Подключаем логирование с использованием ПО промежуточного слоя
			app.UseMiddleware<LoggingMiddleware>();
			



			/*
			//Добавляем компонент для логирования запросов с использованием метода Use.
			app.Use(async (context, next) =>
			{
				// Для логирования данных о запросе используем свойста объекта HttpContext
				Console.WriteLine($"[{DateTime.Now}]: New request to http://{context.Request.Host.Value + context.Request.Path}");
				await next.Invoke();
			});
			*/

			app.UseAuthorization();

			// Перепишем все обработчики с использованием метода Map, а также сделаем для нашего приложения
			// обработчик ошибки Page Not Found (когда пользователь ввёл несуществующую страницу):
			/*
			// Сначала используем метод Use, чтобы не прерывать ковейер
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

			// Завершим вызовом метода Run
			app.Run(async (context) =>
			{
				await context.Response.WriteAsync($"Welcome to the {env.ApplicationName}!");
			});
			*/

			//Добавляем компонент с настройкой маршрутов для главной страницы
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync($"Welcome to the {_env.ApplicationName}!");
				});
			});

			// Все прочие страницы имеют отдельные обработчики
			app.Map("/about", About);
			app.Map("/config", Config);

			// обрабатываем ошибки HTTP
			app.UseStatusCodePages();
			/*
			// Обработчик для ошибки "страница не найдена"
			app.Run(async (context) =>
			{
				await context.Response.WriteAsync($"Page not found");
			});
			*/
		}

		// Обработчики отдельных страниц:
		/// <summary>
		///  Обработчик для страницы About
		/// </summary>
		private void About(IApplicationBuilder app)
		{
			app.Run(async context =>
			{
				await context.Response.WriteAsync($"{_env.ApplicationName} - ASP.Net Core tutorial project");
			});
		}

		/// <summary>
		///  Обработчик для главной страницы
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
