using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Project3;

public partial class MainPage : ContentPage
{
    private readonly ConferenceManager manager;
    private ObservableCollection<Conference> displayedConferences;
    private string currentFilePath = string.Empty;

    public MainPage()
    {
        InitializeComponent();
        manager = new ConferenceManager();
        displayedConferences = new ObservableCollection<Conference>();
        conferencesCollection.ItemsSource = displayedConferences;

        lblFilePath.Text = "Файл не обрано";
    }

    private void EnsureInitialDataCreated()
    {
        if (!File.Exists(currentFilePath))
        {
            var defaultList = new List<Conference>
        {
            new Conference(
                "Інформаційні технології та комп'ютерна інженерія",
                "Комп'ютерні науки",
                "Асоціація IT України",
                "Київ",
                "20.10.2026 - 22.10.2026",
                new DateTime(2026, 9, 15),
                450.0m
            ),
            new Conference(
                "Сучасні проблеми кібербезпеки",
                "Кібербезпека",
                "Держспецзв'язку",
                "Львів",
                "12.11.2026 - 14.11.2026",
                new DateTime(2026, 10, 1),
                300.0m
            ),
            new Conference(
                "Міжнародний форум хмарних технологій",
                "Хмарні технології",
                "Cloud Ukraine Alliance",
                "Одеса",
                "05.12.2026 - 07.12.2026",
                new DateTime(2026, 11, 10),
                800.0m
            )
        };

            var service = new JsonConferenceService();
            service.SaveToFile(currentFilePath, defaultList);
        }

        try
        {
            manager.LoadData(currentFilePath);
            lblFilePath.Text = Path.GetFileName(currentFilePath);
        }
        catch { }
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
        var editPage = new ConferenceEditPage((newConf) =>
        {
            manager.Add(newConf);
            manager.SaveData(currentFilePath); 
            RefreshGrid();
        });

        await Navigation.PushModalAsync(editPage);
    }

    public async void OnEditClicked(object? sender, EventArgs e)
    {
        if (conferencesCollection.SelectedItem is not Conference selected)
        {
            await DisplayAlert("Попередження", "Оберіть рядок для редагування у таблиці!", "OK");
            return;
        }

        int index = manager.GetAll().FindIndex(c => c.Title == selected.Title && c.Deadline == selected.Deadline);

        var editPage = new ConferenceEditPage((updatedConf) =>
        {
            manager.Update(index, updatedConf);
            manager.SaveData(currentFilePath);
            RefreshGrid();
        }, selected);

        await Navigation.PushModalAsync(editPage);
    }

    public async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (conferencesCollection.SelectedItem is not Conference selected)
        {
            await DisplayAlert("Попередження", "Оберіть рядок для видалення у таблиці!", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Підтвердження", $"Видалити конференцію \"{selected.Title}\"?", "Так", "Ні");
        if (confirm)
        {
            int index = manager.GetAll().FindIndex(c => c.Title == selected.Title && c.Deadline == selected.Deadline);
            manager.Delete(index);
            manager.SaveData(currentFilePath);
            RefreshGrid();
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

 
    private void OnFilterByFeeClicked(object sender, EventArgs e)
    {
        if (decimal.TryParse(txtFilterMaxFee.Text, out decimal maxFee))
        {
            var filtered = manager.FilterByMaxFee(maxFee);
            UpdateGridItems(filtered);
        }
        else
        {
            DisplayAlert("Увага", "Введіть коректне число для ціни!", "OK");
        }
    }

    private void OnResetFilterClicked(object sender, EventArgs e)
    {
        txtFilterField.Text = string.Empty;
        RefreshGrid();
    }

    public async void OnAboutClicked(object sender, EventArgs e)
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