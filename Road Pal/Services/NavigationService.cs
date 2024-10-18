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

		public async Task RefreshCurrentPage(Page page)
		{
			var navigationStack = Application.Current.MainPage.Navigation.NavigationStack;

			// Remove the current page (top of the stack)
			if (navigationStack.Count > 0)
			{
				// Insert the new page before the current one
				Application.Current.MainPage.Navigation.InsertPageBefore(page, navigationStack[navigationStack.Count - 1]);
				// Remove the top page (the current one)
				await GoBack();
			}
		}
	}
}
