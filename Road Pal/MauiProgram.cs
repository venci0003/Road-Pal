using Microsoft.Extensions.Logging;
using RoadPal.Contracts;
using RoadPal.Infrastructure;
using RoadPal.Services;
using RoadPal.ViewModels;
using RoadPal.Views;
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
				.UseBarcodeReader();

			builder.Services.AddSingleton<RoadPalDatabase>();

			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<MainPageViewModel>();
			builder.Services.AddTransient<CreateCarPage>();
			builder.Services.AddTransient<CreateCarViewModel>();

			builder.Services.AddTransient<BarcodeReader>();
			builder.Services.AddTransient<BarcodeReaderViewModel>();



			builder.Services.AddTransient<CarService>();
			builder.Services.AddTransient<BarcodeService>();
			builder.Services.AddSingleton<INavigationService, NavigationService>();




#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
