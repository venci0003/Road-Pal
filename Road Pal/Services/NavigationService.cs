namespace RoadPal.Services
{
	using Contracts;
	public class NavigationService : INavigationService
	{
		public Task NavigateToPage(Page page)
		{
			return Application.Current.MainPage.Navigation.PushAsync(page);
		}

		public Task GoBack()
		{
			return Application.Current.MainPage.Navigation.PopAsync();
		}
	}
}
