using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

namespace Project3;

public partial class ConferenceEditPage : ContentPage
{
    private readonly Action<Conference> _onSaveCallback;

    public ConferenceEditPage(Action<Conference> onSaveCallback, Conference? existingConference = null)
    {
        InitializeComponent();
        _onSaveCallback = onSaveCallback;

        if (existingConference != null)
        {
            lblPageTitle.Text = "Редагування конференції";
            LoadConferenceData(existingConference);
        }
        else
        {
            lblPageTitle.Text = "Додати нову конференцію";
            dtpDeadline.Date = DateTime.Today;
        }
    }

    public void LoadConferenceData(Conference item)
    {
        txtTitle.Text = item.Title;
        txtField.Text = item.Field;
        txtOrganizer.Text = item.Organizer;
        txtLocation.Text = item.Location;
        txtStartDate.Text = item.StartDate;
        dtpDeadline.Date = item.Deadline;
        txtFee.Text = item.Fee.ToString("0.##");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        
        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            await DisplayAlert("Помилка вводу", "Будь ласка, введіть назву конференції!", "OK");
            return;
        }

    
        string startDateText = txtStartDate.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(startDateText) || !Regex.IsMatch(startDateText, @"^\d{2}\.\d{2}\.\d{4}(\s*-\s*\d{2}\.\d{2}\.\d{4})?$"))
        {
            await DisplayAlert("Помилка формату дати",
                "Терміни проведення мають бути введені числами у форматі ДД.ММ.РРРР (наприклад: 20.10.2026 або 20.10.2026 - 22.10.2026)!",
                "OK");
            return;
        }


        string feeText = txtFee.Text?.Trim().Replace('.', ',') ?? "0";
        if (!decimal.TryParse(feeText, NumberStyles.Any, new CultureInfo("uk-UA"), out decimal fee) || fee < 0)
        {
            await DisplayAlert("Помилка формату вартості",
                "Вартість участі має бути введена виключно числами (наприклад: 450 або 0)!",
                "OK");
            return;
        }

        var result = new Conference(
            txtTitle.Text.Trim(),
            txtField.Text?.Trim() ?? string.Empty,
            txtOrganizer.Text?.Trim() ?? string.Empty,
            txtLocation.Text?.Trim() ?? string.Empty,
            startDateText,
            dtpDeadline.Date,
            fee
        );

        _onSaveCallback?.Invoke(result);
        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}