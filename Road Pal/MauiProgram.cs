﻿using Microsoft.Extensions.Logging;
using RoadPal.Contracts;
using RoadPal.Infrastructure;
using RoadPal.Services;
using RoadPal.ViewModels;
using RoadPal.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;
using ZXing.Net.Maui.Controls;

namespace RoadPal
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			MauiAppBuilder builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				})
				.UseBarcodeReader()
				.UseSkiaSharp();

			builder.Services.AddSingleton<RoadPalDatabase>();

			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<MainPageViewModel>();
			builder.Services.AddTransient<CreateCarPage>();
			builder.Services.AddTransient<CreateCarViewModel>();

			builder.Services.AddTransient<BarcodeReader>();
			builder.Services.AddTransient<BarcodeReaderViewModel>();



			builder.Services.AddSingleton<ICarService, CarService>();
			builder.Services.AddTransient<IBarcodeService, BarcodeService>();
			builder.Services.AddSingleton<INavigationService, NavigationService>();
			builder.Services.AddTransient<INoteService, NoteService>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
