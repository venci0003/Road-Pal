using RoadPal.ViewModels;
using ZXing.Net.Maui;

namespace RoadPal.Views;

public partial class BarcodeReader : ContentPage
{
	private readonly BarcodeReaderViewModel _viewModel;

	private bool _isScanningAllowed = true;
	private const int ScanDelayMilliseconds = 3000;

	public BarcodeReader(BarcodeReaderViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;

		barcodeReader.Options = new BarcodeReaderOptions()
		{
			Formats = BarcodeFormat.QrCode | BarcodeFormat.Code128 | BarcodeFormat.Code39 | BarcodeFormat.Ean13,
			AutoRotate = true,
			TryHarder = true,
		};
	}

	private async void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
	{
		if (_isScanningAllowed && e.Results.Any())
		{
			_isScanningAllowed = false;
			var first = e.Results?.FirstOrDefault();

			if (first is null)
				return;

			var viewModel = (BarcodeReaderViewModel)BindingContext;
			await viewModel.BarcodeScannedCommand.ExecuteAsync(first.Value);

			await Task.Delay(ScanDelayMilliseconds);

			_isScanningAllowed = true;
		}
	}

	private void ToggleTorchButton_Clicked(object sender, EventArgs e)
	{
		barcodeReader.IsTorchOn = !barcodeReader.IsTorchOn;

		if (barcodeReader.IsTorchOn)
		{
			TorchButton.Source = "torch_yellow_icon.png";
		}
		else
		{
			TorchButton.Source = "torch_white_icon.png";
		}
	}
}
