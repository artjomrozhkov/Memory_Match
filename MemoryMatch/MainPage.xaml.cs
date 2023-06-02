using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MemoryMatch
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            // Создаем контейнер для страницы
            StackLayout pageContainer = new StackLayout
            {
                BackgroundColor = Color.FromHex("#232F34"),
                Padding = new Thickness(20),
                Spacing = 20,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            // Создаем заголовок
            Label titleLabel = new Label
            {
                Text = "Игра Memory Match",
                TextColor = Color.FromHex("#FFC107"),
                FontSize = 36,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "Arial",
                HorizontalTextAlignment = TextAlignment.Center
            };
            pageContainer.Children.Add(titleLabel);

            // Создаем описание игры
            Label descriptionLabel = new Label
            {
                Text = "Попробуйте найти все пары одинаковых карточек!",
                TextColor = Color.White,
                FontSize = 24,
                FontFamily = "Arial",
                HorizontalTextAlignment = TextAlignment.Center
            };
            pageContainer.Children.Add(descriptionLabel);


            // Создаем выбор размера поля
            Label sizeLabel = new Label
            {
                Text = "Выберите размер поля:",
                TextColor = Color.White,
                FontSize = 20,
                FontFamily = "Arial",
                HorizontalTextAlignment = TextAlignment.Center
            };
            pageContainer.Children.Add(sizeLabel);

            Picker sizePicker = new Picker
            {
                Title = "Размер поля",
                FontFamily = "Arial",
                FontSize = 20,
                BackgroundColor = Color.White,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            sizePicker.Items.Add("2x2");
            sizePicker.Items.Add("3x3");
            sizePicker.Items.Add("4x4");
            sizePicker.Items.Add("5x5");
            sizePicker.Items.Add("6x6");
            sizePicker.SelectedIndex = 0;
            pageContainer.Children.Add(sizePicker);

            // Создаем поле ввода имени игрока
            Entry playerNameEntry = new Entry
            {
                Placeholder = "Введите имя игрока",
                FontFamily = "Arial",
                FontSize = 20,
                BackgroundColor = Color.White,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                MaxLength = 12

            };
            pageContainer.Children.Add(playerNameEntry);

            // Создаем кнопку "Начать игру"
            Button startButton = new Button
            {
                Text = "Начать игру",
                FontFamily = "Arial",
                FontSize = 24,
                BackgroundColor = Color.FromHex("#FFC107"),
                TextColor = Color.White,
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            playerNameEntry.TextChanged += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(playerNameEntry.Text))
                {
                    startButton.IsEnabled = false; // Блокируем кнопку, если поле пустое
                }
                else
                {
                    startButton.IsEnabled = true; // Разблокируем кнопку, если поле заполнено
                }
            };


            // Устанавливаем исходное состояние кнопки
            startButton.IsEnabled = false;

            Button checkResult = new Button
            {
                Text = "Результаты",
                FontFamily = "Arial",
                FontSize = 16,
                BackgroundColor = Color.FromHex("#FFC107"),
                TextColor = Color.White,
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            startButton.Clicked += async (sender, args) =>
            {
                // Запускаем игру с выбранным размером поля и именем игрока
                string selectedSize = sizePicker.SelectedItem.ToString();
                int size = int.Parse(selectedSize.Split('x')[0]);
                string playerName = playerNameEntry.Text;
                await Navigation.PushAsync(new MemoryMatchPage(size, playerName));
            };
            pageContainer.Children.Add(startButton);

            checkResult.Clicked += async (sender, args) =>
            {
                await Navigation.PushAsync(new CheckResultPage());
            };
            pageContainer.Children.Add(checkResult);


            // Добавляем контейнер на страницу
            Content = pageContainer;
        }
    }
}
