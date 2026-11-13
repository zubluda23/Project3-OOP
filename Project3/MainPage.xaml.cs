using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Project3;

public partial class MainPage : ContentPage
{
    private readonly ConferenceManager manager;
    private readonly ObservableCollection<Conference> displayedConferences;
    private string currentFilePath = string.Empty;

    public MainPage()
    {
        InitializeComponent();
        manager = new ConferenceManager();
        displayedConferences = new ObservableCollection<Conference>();
        conferencesCollection.ItemsSource = displayedConferences;

        lblFilePath.Text = "Файл не обрано";
    }

    public async void OnOpenFileClicked(object? sender, EventArgs e)
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.MacCatalyst, new[] { "public.json" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть файл JSON",
                FileTypes = customFileType
            });

            if (result != null)
            {
                currentFilePath = result.FullPath;
                manager.LoadData(currentFilePath);
                lblFilePath.Text = result.FileName;
                RefreshGrid();
                await DisplayAlert("Успіх", "Файл успішно завантажено!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося відкрити файл: {ex.Message}", "OK");
        }
    }

    public async void OnSaveFileClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(currentFilePath))
        {
            await DisplayAlert("Увага", "Спочатку відкрийте JSON-файл через диспетчер!", "OK");
            return;
        }

        try
        {
            manager.SaveData(currentFilePath);
            await DisplayAlert("Успіх", $"Дані збережено у файл:\n{lblFilePath.Text}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Помилка збереження: {ex.Message}", "OK");
        }
    }

    public async void OnAddClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(currentFilePath))
        {
            await DisplayAlert("Увага", "Спочатку відкрийте JSON-файл через диспетчер!", "OK");
            return;
        }

        var editPage = new ConferenceEditPage(async (newConf) =>
        {
            try
            {
                manager.Add(newConf);
                manager.SaveData(currentFilePath);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка збереження", ex.Message, "OK");
            }
        });

        await Navigation.PushModalAsync(editPage);
    }

    public async void OnEditClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(currentFilePath))
        {
            await DisplayAlert("Увага", "Спочатку відкрийте JSON-файл через диспетчер!", "OK");
            return;
        }

        if (conferencesCollection.SelectedItem is not Conference selected)
        {
            await DisplayAlert("Попередження", "Оберіть рядок для редагування у таблиці!", "OK");
            return;
        }

        int index = manager.GetAll().FindIndex(c => c.Title == selected.Title && c.Deadline == selected.Deadline);

        var editPage = new ConferenceEditPage(async (updatedConf) =>
        {
            try
            {
                manager.Update(index, updatedConf);
                manager.SaveData(currentFilePath);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка збереження", ex.Message, "OK");
            }
        }, selected);

        await Navigation.PushModalAsync(editPage);
    }

    public async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(currentFilePath))
        {
            await DisplayAlert("Увага", "Спочатку відкрийте JSON-файл через диспетчер!", "OK");
            return;
        }

        if (conferencesCollection.SelectedItem is not Conference selected)
        {
            await DisplayAlert("Попередження", "Оберіть рядок для видалення у таблиці!", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Підтвердження", $"Видалити конференцію \"{selected.Title}\"?", "Так", "Ні");
        if (confirm)
        {
            try
            {
                int index = manager.GetAll().FindIndex(c => c.Title == selected.Title && c.Deadline == selected.Deadline);
                manager.Delete(index);
                manager.SaveData(currentFilePath);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка видалення", ex.Message, "OK");
            }
        }
    }

    private void OnFilterByFieldClicked(object sender, EventArgs e)
    {
        var filtered = manager.FilterByField(txtFilterField.Text);
        UpdateGridItems(filtered);
    }

    private void OnFilterByDeadlineClicked(object sender, EventArgs e)
    {
        var filtered = manager.FilterByDeadline(dtpFilterDeadline.Date);
        UpdateGridItems(filtered);
    }

    private async void OnFilterByFeeClicked(object sender, EventArgs e)
    {
        string feeText = txtFilterMaxFee.Text?.Trim().Replace('.', ',') ?? string.Empty;
        if (decimal.TryParse(feeText, NumberStyles.Number, new CultureInfo("uk-UA"), out decimal maxFee) && maxFee >= 0)
        {
            var filtered = manager.FilterByMaxFee(maxFee);
            UpdateGridItems(filtered);
        }
        else
        {
            await DisplayAlert("Увага", "Введіть коректне невід'ємне число для ціни!", "OK");
        }
    }

    private void OnResetFilterClicked(object sender, EventArgs e)
    {
        txtFilterField.Text = string.Empty;
        dtpFilterDeadline.Date = DateTime.Today;
        txtFilterMaxFee.Text = "5000";
        RefreshGrid();
    }

    public async void OnAboutClicked(object? sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AboutPage());
    }

    public void RefreshGrid()
    {
        UpdateGridItems(manager.GetAll());
    }

    private void UpdateGridItems(List<Conference> list)
    {
        displayedConferences.Clear();
        foreach (var item in list)
        {
            displayedConferences.Add(item);
        }
    }
}