using RoadPal.ViewModels;
using ZXing;

namespace RoadPal.Views;

public partial class CarDetailsPage : ContentPage
{
	public CarDetailsPage(CarDetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}