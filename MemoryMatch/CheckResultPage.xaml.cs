using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MemoryMatch
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckResultPage : ContentPage
    {
        private ListView _resultListView;
        private ResultDatabase _database;

        public CheckResultPage()
        {
            InitializeComponent();

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db");
            _database = new ResultDatabase(dbPath);

            _resultListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(ResultCell)),
                SeparatorColor = Color.FromHex("#CCCCCC"),
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            _resultListView.ItemSelected += ResultListView_ItemSelected;

            var layout = new StackLayout();
            layout.Children.Add(_resultListView);

            Content = layout;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Получение результатов из базы данных
            List<Result> results = await _database.GetResultsAsync();

            // Отсортируйте результаты по убыванию баллов и возрастанию неудачных попыток
            results = results.OrderByDescending(r => r.Score).ThenBy(r => r.WrongAttempts).ToList();

            // Определение места и установка кубков
            for (int i = 0; i < results.Count; i++)
            {
                if (i == 0)
                    results[i].Place = 1;
                else if (results[i].Score == results[i - 1].Score && results[i].WrongAttempts == results[i - 1].WrongAttempts)
                    results[i].Place = results[i - 1].Place;
                else
                    results[i].Place = i + 1;
            }

            // Установка списка результатов в ListView
            _resultListView.ItemsSource = results;

            // Установка BindingContext
            BindingContext = results;
        }

        private async void ResultListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var result = e.SelectedItem as Result;

            // Отображение диалогового окна с вопросом об удалении
            bool answer = await DisplayAlert("Удаление результата", "Вы уверены, что хотите удалить этот результат?", "Да", "Нет");

            if (answer)
            {
                await _database.DeleteResultAsync(result);

                List<Result> updatedResults = await _database.GetResultsAsync();

                updatedResults = updatedResults.OrderByDescending(r => r.Score).ThenBy(r => r.WrongAttempts).ToList();

                for (int i = 0; i < updatedResults.Count; i++)
                {
                    if (i == 0)
                        updatedResults[i].Place = 1;
                    else if (updatedResults[i].Score == updatedResults[i - 1].Score && updatedResults[i].WrongAttempts == updatedResults[i - 1].WrongAttempts)
                        updatedResults[i].Place = updatedResults[i - 1].Place;
                    else
                        updatedResults[i].Place = i + 1;
                }

                _resultListView.ItemsSource = updatedResults;

                BindingContext = updatedResults;
            }

            _resultListView.SelectedItem = null;
        }
        private void SortPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSortCriteria = sortPicker.SelectedItem as string;

            switch (selectedSortCriteria)
            {
                case "Player Name":
                    SortResults("PlayerName");
                    break;
                case "Score":
                    SortResults("Score");
                    break;
                case "Wrong Attempts":
                    SortResults("WrongAttempts");
                    break;
            }
        }

        private void SortResults(string sortBy)
        {
            if (sortBy == "PlayerName")
            {
                _resultListView.ItemsSource = (_resultListView.ItemsSource as List<Result>).OrderBy(r => r.PlayerName).ToList();
            }
            else if (sortBy == "Score")
            {
                _resultListView.ItemsSource = (_resultListView.ItemsSource as List<Result>).OrderByDescending(r => r.Score).ToList();
            }
            else if (sortBy == "WrongAttempts")
            {
                _resultListView.ItemsSource = (_resultListView.ItemsSource as List<Result>).OrderBy(r => r.WrongAttempts).ToList();
            }
        }
    }
}
