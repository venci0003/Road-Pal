using RoadPal.ViewModels;

namespace RoadPal.Views;

public partial class CreateCarPage : ContentPage
{
	public CreateCarPage(CreateCarViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	private async void OnModelPickerFocused(object sender, FocusEventArgs e)
	{
		CreateCarViewModel? viewModel = BindingContext as CreateCarViewModel;

		if (viewModel != null && !string.IsNullOrEmpty(viewModel.SelectedMake))
		{
			await viewModel.LoadCarModelsCommand.ExecuteAsync(viewModel.SelectedMake);
		}
	}

}