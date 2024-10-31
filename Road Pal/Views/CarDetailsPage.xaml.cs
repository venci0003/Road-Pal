using RoadPal.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RoadPal.Views;

public partial class CarDetailsPage : ContentPage
{
	private readonly CarDetailsViewModel _viewModel;

	public CarDetailsPage(CarDetailsViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		BindingContext = _viewModel;
		InsuranceHiddenWebView.Navigated += InsuranceHiddenWebView_Navigated;
		InspectionHiddenWebView.Navigated += InspectionHiddenWebView_Navigated;
	}

	private bool isInspectionProcessing = false;

	private async void OnCheckCarInspectionButtonClicked(object sender, EventArgs e)
	{
		try
		{
			_viewModel.CaptchaInput = null;

			_viewModel.OpenInspectionCaptcha();

			var imageUrl = await InspectionHiddenWebView.EvaluateJavaScriptAsync("document.querySelector('img.mb-3').src || 'Image not found';");

			CaptchaImage.Source = imageUrl;

			await InspectionHiddenWebView.EvaluateJavaScriptAsync($"document.getElementById('regPlate').value = '{_viewModel.LicensePlate}';");

		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error occurred: {ex.Message}");
			await _viewModel.CheckCarInsuranceAsync("An error occurred while checking the car info.");
		}
	}

	private async void CaptchaSubmitButtonClicked(object sender, EventArgs e)
	{
		isInspectionProcessing = true;

		await InspectionHiddenWebView.EvaluateJavaScriptAsync($"document.getElementById('captcha').value = '{_viewModel.CaptchaInput}';");
		await InspectionHiddenWebView.EvaluateJavaScriptAsync("document.querySelector('button.btn.btn-primary').click();");
	}

	private async void InspectionHiddenWebView_Navigated(object? sender, WebNavigatedEventArgs e)
	{
		if (!isInspectionProcessing)
		{
			return;
		}

		try
		{
			var alertMessage = await InspectionHiddenWebView.EvaluateJavaScriptAsync("document.querySelector('.alert.alert-success.text-center.loading-content')?.innerText.trim() || 'Not found';");

			string[] splittedResult = alertMessage.Split(' ');

			string result = $"INSPECTION DETAILS\n- Valid technical inspection until {splittedResult[splittedResult.Length - 1]}";

			await _viewModel.CheckCarInspectionAsync(result);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error occurred during navigation: {ex.Message}");
			await _viewModel.CheckCarInspectionAsync("An error occurred while checking the car info.");
		}
		finally
		{
			isInspectionProcessing = false;
		}
	}

	private bool isInsuranceProcessing = false;

	private async void OnCheckCarInsuranceButtonClicked(object sender, EventArgs e)
	{
		try
		{
			await InsuranceHiddenWebView.EvaluateJavaScriptAsync($"document.getElementById('regPlate').value = '{_viewModel.LicensePlate}';");

			await InsuranceHiddenWebView.EvaluateJavaScriptAsync("document.querySelector('button.btn.btn-primary').click();");

			isInsuranceProcessing = true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error occurred: {ex.Message}");
			await _viewModel.CheckCarInsuranceAsync("An error occurred while checking the car info.");
		}
	}

	private async void InsuranceHiddenWebView_Navigated(object? sender, WebNavigatedEventArgs e)
	{
		if (!isInsuranceProcessing)
		{
			return;
		}

		try
		{
			var insurer = await InsuranceHiddenWebView.EvaluateJavaScriptAsync("document.querySelector('dd.col-sm-6.text-md-left.text-sm-center:nth-of-type(1)')?.innerText || 'Not found';");
			var validFrom = await InsuranceHiddenWebView.EvaluateJavaScriptAsync("document.querySelector('dd.col-sm-6.text-md-left.text-sm-center:nth-of-type(2)')?.innerText || 'Not found';");
			var validTo = await InsuranceHiddenWebView.EvaluateJavaScriptAsync("document.querySelector('dd.col-sm-6.text-md-left.text-sm-center:nth-of-type(3)')?.innerText || 'Not found';");

			TimeSpan timeLeft = DateTime.Parse(validTo) - DateTime.UtcNow;
			int daysLeft = (int)timeLeft.TotalDays;

			string result = $"INSURANCE DETAILS\n- Insurer: {insurer}\n- Valid from: {validFrom}\n- Valid to: {validTo}\n- Days left: {daysLeft}\n";

			await _viewModel.CheckCarInsuranceAsync(result);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error occurred during navigation: {ex.Message}");
			await _viewModel.CheckCarInsuranceAsync("An error occurred while checking the car info.");
		}
		finally
		{
			isInsuranceProcessing = false;
		}
	}


	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadDetailsAsync();
	}

	private bool _isCollapsedServiceNote = false;

	private bool _isCollapsedBarcode = false;

	private async void BarcodeHeaderTapped(object sender, EventArgs e)
	{
		if (_isCollapsedBarcode)
		{
			//BarcodesCollectionView.IsVisible = true;

			await ArrowBarcode.FadeTo(0, 100);

			await Task.WhenAll(
			BarcodesCollectionView.FadeTo(1, 800),
			ArrowBarcode.RotateTo(0, 150));

			await ArrowBarcode.FadeTo(1, 100);

			_isCollapsedBarcode = false;
		}
		else
		{
			await ArrowBarcode.FadeTo(0, 100);

			await Task.WhenAll(
			BarcodesCollectionView.FadeTo(0, 400),
			ArrowBarcode.RotateTo(180, 150));

			await ArrowBarcode.FadeTo(1, 100);

			//BarcodesCollectionView.IsVisible = false;


			_isCollapsedBarcode = true;
		}
	}

	private async void ServiceNoteHeaderTapped(object sender, EventArgs e)
	{
		if (_isCollapsedServiceNote)
		{
			ServiceNotesCollectionView.IsVisible = true;
			FinishedFrame.IsVisible = true;
			UnfinishedFrame.IsVisible = true;

			await ArrowServiceNote.FadeTo(0, 100);

			await Task.WhenAll(
			ServiceNotesCollectionView.FadeTo(1, 800),
			FinishedFrame.FadeTo(1, 800),
			UnfinishedFrame.FadeTo(1, 800),
			ArrowServiceNote.RotateTo(0, 150));

			await ArrowServiceNote.FadeTo(1, 100);

			_isCollapsedServiceNote = false;
		}
		else
		{
			await ArrowServiceNote.FadeTo(0, 100);

			await Task.WhenAll(
			ServiceNotesCollectionView.FadeTo(0, 400),
			FinishedFrame.FadeTo(0, 400),
			UnfinishedFrame.FadeTo(0, 400),
			ArrowServiceNote.RotateTo(180, 150));

			await ArrowServiceNote.FadeTo(1, 100);

			ServiceNotesCollectionView.IsVisible = false;
			FinishedFrame.IsVisible = false;
			UnfinishedFrame.IsVisible = false;

			_isCollapsedServiceNote = true;
		}
	}

	private void AnimateColorTransition(VisualElement element, Color startColor, Color endColor, uint duration = 500)
	{
		var animation = new Animation(v =>
		{
			if (element is Label label)
			{
				label.TextColor = new Color(
					(float)(Colors.Gray.Red + (Colors.White.Red - Colors.Gray.Red) * v),
					(float)(Colors.Gray.Green + (Colors.White.Green - Colors.Gray.Green) * v),
					(float)(Colors.Gray.Blue + (Colors.White.Blue - Colors.Gray.Blue) * v),
					(float)(Colors.Gray.Alpha + (Colors.White.Alpha - Colors.Gray.Alpha) * v)
				);
			}
			else if (element is Frame frame)
			{
				element.BackgroundColor = new Color(
					(float)(startColor.Red + (endColor.Red - startColor.Red) * v),
					(float)(startColor.Green + (endColor.Green - startColor.Green) * v),
					(float)(startColor.Blue + (endColor.Blue - startColor.Blue) * v),
					(float)(startColor.Alpha + (endColor.Alpha - startColor.Alpha) * v)
				);
			}
		}, 0, 1);

		animation.Commit(element, "ColorAnimation", length: duration, easing: Easing.Linear);
	}

	private bool _isInFinishedSection = false;

	private async void OnFinishedTapped(object sender, EventArgs e)
	{
		if (_isInFinishedSection) return;

		_isInFinishedSection = true;

		await ServiceNotesCollectionView.FadeTo(0, 400);

		AnimateColorTransition(UnfinishedFrame, UnfinishedFrame.BackgroundColor, Color.FromArgb("#171E25"));
		AnimateColorTransition(FinishedFrame, FinishedFrame.BackgroundColor, Color.FromArgb("#3A4755"));

		AnimateColorTransition(UnfinishedLabel, UnfinishedLabel.TextColor, Colors.White, 1000);
		AnimateColorTransition(FinishedLabel, FinishedLabel.TextColor, Colors.Gray, 1000);

		await _viewModel.ChangeToFinished();

		await ServiceNotesCollectionView.FadeTo(1, 400);
	}

	private async void OnUnfinishedTapped(object sender, EventArgs e)
	{
		if (!_isInFinishedSection)
		{
			return;
		}

		_isInFinishedSection = false;

		await ServiceNotesCollectionView.FadeTo(0, 400);

		AnimateColorTransition(UnfinishedFrame, UnfinishedFrame.BackgroundColor, Color.FromArgb("#3A4755"));
		AnimateColorTransition(FinishedFrame, FinishedFrame.BackgroundColor, Color.FromArgb("#171E25"));

		AnimateColorTransition(UnfinishedLabel, UnfinishedLabel.TextColor, Colors.Gray, 1000);
		AnimateColorTransition(FinishedLabel, FinishedLabel.TextColor, Colors.White, 1000);

		await _viewModel.ChangeToUnfinished();

		await ServiceNotesCollectionView.FadeTo(1, 400);
	}
}