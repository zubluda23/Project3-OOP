using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Project3;

public partial class ConferenceEditPage : ContentPage
{
    private readonly Action<Conference> _onSaveCallback;
    private readonly Conference? currentConference;

    public ConferenceEditPage(Action<Conference> onSaveCallback, Conference? existingConference = null)
    {
        InitializeComponent();
        _onSaveCallback = onSaveCallback;
        currentConference = existingConference;

        if (currentConference is not null)
        {
            lblPageTitle.Text = "Редагування конференції";
            LoadConferenceData(currentConference);
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
        if (!IsValidConferenceDates(startDateText, out string dateError))
        {
            await DisplayAlert("Помилка формату дати", dateError, "OK");
            return;
        }

        
        string feeText = txtFee.Text?.Trim().Replace('.', ',') ?? "0";
        if (!decimal.TryParse(feeText, NumberStyles.Number, new CultureInfo("uk-UA"), out decimal fee) || fee < 0)
        {
            await DisplayAlert("Помилка формату вартості",
                "Вартість участі має бути невід'ємним числом (наприклад: 450 або 450,50)!",
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

    private static bool IsValidConferenceDates(string input, out string errorMessage)
    {
        errorMessage = "Терміни проведення мають бути реальною датою у форматі ДД.ММ.РРРР (наприклад: 20.10.2026 або 20.10.2026 - 22.10.2026)!";
        if (string.IsNullOrWhiteSpace(input)) return false;

        string[] parts = input.Split('-', StringSplitOptions.TrimEntries);
        if (parts.Length == 1)
        {
            return DateTime.TryParseExact(parts[0], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
        if (parts.Length == 2)
        {
            bool okStart = DateTime.TryParseExact(parts[0], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start);
            bool okEnd = DateTime.TryParseExact(parts[1], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end);
            if (!okStart || !okEnd) return false;

            if (start > end)
            {
                errorMessage = "Дата початку конференції не може бути пізнішою за дату завершення!";
                return false;
            }
            return true;
        }
        return false;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}