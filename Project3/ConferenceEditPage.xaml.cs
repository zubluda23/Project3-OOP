using System;
using Microsoft.Maui.Controls;

namespace  Project3;


public partial class ConferenceEditPage : ContentPage
{
    private readonly Action<Conference> _onSaveCallback;
    private readonly Conference? _editingConference;

    public ConferenceEditPage(Action<Conference> onSaveCallback, Conference? conferenceToEdit = null)
    {
        InitializeComponent();
        _onSaveCallback = onSaveCallback;
        _editingConference = conferenceToEdit;

        if (_editingConference != null)
        {
            lblPageTitle.Text = "Редагування конференції";
            LoadConferenceData(_editingConference);
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
        txtFee.Text = item.Fee.ToString();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            await DisplayAlert("Попередження", "Введіть назву конференції!", "OK");
            return;
        }

        decimal.TryParse(txtFee.Text, out decimal fee);

        var result = new Conference(
            txtTitle.Text.Trim(),
            txtField.Text?.Trim() ?? string.Empty,
            txtOrganizer.Text?.Trim() ?? string.Empty,
            txtLocation.Text?.Trim() ?? string.Empty,
            txtStartDate.Text?.Trim() ?? string.Empty,
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
