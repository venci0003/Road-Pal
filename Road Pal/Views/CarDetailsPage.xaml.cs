using RoadPal.ViewModels;

namespace RoadPal.Views;

public partial class CarDetailsPage : ContentPage
{
	public CarDetailsPage(CarDetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}