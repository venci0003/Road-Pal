using Microsoft.Extensions.Logging;
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


			builder.Services.AddTransient<CarService>();






#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
