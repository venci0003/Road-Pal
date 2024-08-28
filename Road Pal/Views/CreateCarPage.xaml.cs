using RoadPal.ViewModels;

namespace RoadPal.Views;

public partial class CreateCarPage : ContentPage
{
	public CreateCarPage(CreateCarViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}