using RoadPal.ViewModels;
using ZXing.Net.Maui;

namespace RoadPal.Views;

public partial class BarcodeReader : ContentPage
{
	private readonly BarcodeReaderViewModel _viewModel;

	public BarcodeReader(BarcodeReaderViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;

		barcodeReader.Options = new BarcodeReaderOptions()
		{
			Formats = BarcodeFormat.QrCode | BarcodeFormat.Code128 | BarcodeFormat.Code39 | BarcodeFormat.Ean13,
			AutoRotate = true,
			TryHarder = true
		};
	}

	private async void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
	{
		var first = e.Results?.FirstOrDefault();

		if (first is null)
			return;

		var viewModel = (BarcodeReaderViewModel)BindingContext;
		await viewModel.BarcodeScannedCommand.ExecuteAsync(first.Value);
	}

	private void ToggleTorchButton_Clicked(object sender, EventArgs e)
	{
		barcodeReader.IsTorchOn = !barcodeReader.IsTorchOn;
	}
}
