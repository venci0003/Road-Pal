using Microsoft.Maui.Controls;
using RoadPal.ViewModels;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RoadPal.Views;

public partial class CarDetailsPage : ContentPage
{
	private readonly CarDetailsViewModel _viewModel;

	public CarDetailsPage(CarDetailsViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		BindingContext = _viewModel;
	}

	private bool isInspectionProcessing = false;

	private async void OnCheckCarInspectionButtonClicked(object sender, EventArgs e)
	{
		try
		{
			_viewModel.CaptchaInput = null;
			_viewModel.OpenInspectionCaptcha();

			await InspectionHiddenWebView.EvaluateJavaScriptAsync($@"
        function simulateTyping(element, text) {{
            for (let i = 0; i < text.length; i++) {{
                let char = text[i];
                element.dispatchEvent(new KeyboardEvent('keydown', {{ key: char }}));
                element.value += char;
                element.dispatchEvent(new KeyboardEvent('keypress', {{ key: char }}));
                element.dispatchEvent(new KeyboardEvent('keyup', {{ key: char }}));
                element.dispatchEvent(new Event('input', {{ bubbles: true }}));
            }}
            element.dispatchEvent(new Event('change', {{ bubbles: true }}));
            element.dispatchEvent(new Event('blur', {{ bubbles: true }}));
        }}

        var inputElement1 = document.evaluate('/html/body/div[2]/div/div[6]/div/div[1]/div/div[1]/input', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
        if (inputElement1) {{
            inputElement1.value = '';
            simulateTyping(inputElement1, '{_viewModel.LicensePlate}');
        }}
        ");
			
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

		await InspectionHiddenWebView.EvaluateJavaScriptAsync($@"
    function simulateTyping(element, text) {{
        for (let i = 0; i < text.length; i++) {{
            let char = text[i];
            element.dispatchEvent(new KeyboardEvent('keydown', {{ key: char }}));
            element.value += char;
            element.dispatchEvent(new KeyboardEvent('keypress', {{ key: char }}));
            element.dispatchEvent(new KeyboardEvent('keyup', {{ key: char }}));
            element.dispatchEvent(new Event('input', {{ bubbles: true }}));
        }}
        element.dispatchEvent(new Event('change', {{ bubbles: true }}));
        element.dispatchEvent(new Event('blur', {{ bubbles: true }}));
    }}

    var inputElement2 = document.evaluate('/html/body/div[2]/div/div[6]/div/div[1]/div/div[2]/input', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
    if (inputElement2) {{
        inputElement2.value = '';
        simulateTyping(inputElement2, '{_viewModel.CaptchaInput}');
    }}
    ");

		await InspectionHiddenWebView.EvaluateJavaScriptAsync($@"
    var element = document.evaluate('/html/body/div[2]/div/div[6]/div/div[1]/div/a[2]/span', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
    if (element && element.offsetParent !== null) {{
        element.click();
    }}
    ");
		await Task.Delay(1600);

		string result = string.Empty;

		string validDate = await InspectionHiddenWebView.EvaluateJavaScriptAsync($@"document.evaluate('/html/body/div[2]/div/div[6]/div/div[2]/div[1]/div[2]/i[7]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue?.innerText;");
		string ecoCategory = await InspectionHiddenWebView.EvaluateJavaScriptAsync($@"document.evaluate('/html/body/div[2]/div/div[6]/div/div[2]/div[1]/div[2]/i[4]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue?.innerText;");

		if (!string.IsNullOrEmpty(validDate) && !string.IsNullOrEmpty(ecoCategory))
		{
			result = $"INSPECTION DETAILS\n- Valid technical inspection until {validDate}\nEco Category: {ecoCategory}";
		}
		else
		{
			result = "An error occured, please try again later.";
		}


		await _viewModel.CheckCarInspectionAsync(result);

		InspectionHiddenWebView.Source = "https://rta.government.bg/services/check-inspection/index.html";
	}

	private async void OnCheckCarInsuranceButtonClicked(object sender, EventArgs e)
	{
		bool isConfirmed = await DisplayAlert(
			"Leaving the App",
			"You are about to leave the app and open a web page. Do you want to continue?",
			"Yes",
			"No");

		if (isConfirmed)
		{
			string url = "https://www.guaranteefund.org/bg/%D0%B8%D0%BD%D1%84%D0%BE%D1%80%D0%BC%D0%B0%D1%86%D0%B8%D0%BE%D0%BD%D0%B5%D0%BD-%D1%86%D0%B5%D0%BD%D1%82%D1%8A%D1%80-%D0%B8-%D1%81%D0%BF%D1%80%D0%B0%D0%B2%D0%BA%D0%B8/%D1%83%D1%81%D0%BB%D1%83%D0%B3%D0%B8/%D0%BF%D1%80%D0%BE%D0%B2%D0%B5%D1%80%D0%BA%D0%B0-%D0%B7%D0%B0-%D0%B2%D0%B0%D0%BB%D0%B8%D0%B4%D0%BD%D0%B0-%D0%B7%D0%B0%D1%81%D1%82%D1%80%D0%B0%D1%85%D0%BE%D0%B2%D0%BA%D0%B0-%D0%B3%D1%80a%D0%B6%D0%B4a%D0%BD%D1%81%D0%BAa-%D0%BE%D1%82%D0%B3%D0%BE%D0%B2%D0%BE%D1%80%D0%BD%D0%BE%D1%81%D1%82-%D0%BD%D0%B0-%D0%B0%D0%B2%D1%82%D0%BE%D0%BC%D0%BE%D0%B1%D0%B8%D0%BB%D0%B8%D1%81%D1%82%D0%B8%D1%82%D0%B5";

			await Launcher.OpenAsync(url);
		}
		else
		{
			Console.WriteLine("User canceled opening the URL.");
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