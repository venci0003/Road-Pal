using RoadPal.Views;

namespace RoadPal
{
	public partial class App : Application
	{
		public App(IServiceProvider serviceProvider)
		{
			InitializeComponent();

			MainPage = new AppShell();

			//MainPage = new NavigationPage(serviceProvider.GetRequiredService<MainPage>());

			Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

		}
	}
}