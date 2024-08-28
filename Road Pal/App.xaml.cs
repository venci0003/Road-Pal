using RoadPal.Views;

namespace RoadPal
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new AppShell();

			Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

		}
	}
}