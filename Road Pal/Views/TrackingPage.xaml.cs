using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using RoadPal.ViewModel;

namespace RoadPal.Views;

public partial class TrackingPage : ContentPage
{
	private readonly TrackingViewModel _viewModel;

	public TrackingPage(TrackingViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		BindingContext = _viewModel;

		InitializeMap();
	}

	private async void InitializeMap()
	{
		var map = new Mapsui.Map();

		var tileLayer = OpenStreetMap.CreateTileLayer();
		map.Layers.Add(tileLayer);

		MapControl.Map = map;


		var location = await GetCurrentLocationAsync();

		if (location != null)
		{
			var lat = location.Latitude;
			var lon = location.Longitude;

			var center = new Mapsui.MPoint(SphericalMercator.FromLonLat(lon, lat).x, SphericalMercator.FromLonLat(lon, lat).y);

			MapControl.Map.Navigator.RotationLock = true;

			MapControl.Map?.Navigator?.CenterOn(center);
			MapControl.Map?.Navigator?.ZoomTo(3);
		}

		MapControl.Refresh();
	}



	private async Task<Location> GetCurrentLocationAsync()
	{
		var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
		var location = await Geolocation.GetLocationAsync(request);

		if (location != null)
		{
			Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
			return location;
		}

		return null;
	}
}