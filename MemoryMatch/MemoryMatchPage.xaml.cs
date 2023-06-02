using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Drawing;

namespace MemoryMatch
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MemoryMatchPage : ContentPage
    {
        private readonly ResultDatabase _database;
        private readonly int _size;
        private readonly List<char> _cardValues;
        private readonly List<Button> _cardButtons;
        private string _firstCard;
        private string _secondCard;
        private int _numCardsRevealed = 0;
        private string playerName;

        public MemoryMatchPage(int size, string playerName)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db");
            _database = new ResultDatabase(dbPath);

            this.playerName = playerName;
            _size = size;
            _cardValues = GenerateCardValues();
            _cardButtons = GenerateCardButtons();

            var grid = new Grid
            {
                BackgroundColor = Xamarin.Forms.Color.FromHex("#232F34"),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            for (var row = 0; row < _size; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (var col = 0; col < _size; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            var random = new Random();

            foreach (var button in _cardButtons)
            {
                var row = random.Next(0, _size);
                var col = random.Next(0, _size);
                while (grid.Children.Any(x => Grid.GetRow(x) == row && Grid.GetColumn(x) == col))
                {
                    row = random.Next(0, _size);
                    col = random.Next(0, _size);
                }
                grid.Children.Add(button, col, row);
            }

            Content = grid;
        }

        private List<char> GenerateCardValues()
        {
            var symbols = new List<char> { '☀', '☁', '☂', '☃', '☄', '★', '☆', '☎', '☮', '☯', '♠', '♣', '♥', '♦', '♪', '♫', '♛', '♞' };

            var cardValues = new List<char>();

            for (var i = 0; i < _size * _size / 2; i++)
            {
                var symbol = symbols[i % symbols.Count];
                cardValues.Add(symbol);
                cardValues.Add(symbol);
            }

            var random = new Random();
            for (var i = 0; i < cardValues.Count; i++)
            {
                var temp = cardValues[i];
                var randomIndex = random.Next(i, cardValues.Count);
                cardValues[i] = cardValues[randomIndex];
                cardValues[randomIndex] = temp;
            }

            return cardValues;
        }

        private List<Button> GenerateCardButtons()
        {
            var cardButtons = new List<Button>();

            var availablePositions = new List<int>();
            for (var i = 0; i < _size * _size; i++)
            {
                availablePositions.Add(i);
            }

            var cardValues = GenerateCardValues();

            foreach (var cardValue in cardValues)
            {
                var positionIndex = availablePositions.Count > 1 ? new Random().Next(0, availablePositions.Count - 1) : 0;
                var position = availablePositions[positionIndex];
                availablePositions.RemoveAt(positionIndex);

                var button = new Button
                {
                    Text = "",
                    FontSize = 120 / (_size * 0.6),
                    BackgroundColor = System.Drawing.Color.Gray,
                    TextColor = System.Drawing.Color.Black,
                    CornerRadius = 10,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                var index = position;
                button.Clicked += (sender, args) =>
                {
                    if (button.Text != "" || (_firstCard != null && _secondCard != null))
                        return;

                    var clickedIndex = index % _cardValues.Count; // Use modulo operator to get a valid index
                    button.Text = _cardValues[clickedIndex].ToString();
                    button.BackgroundColor = System.Drawing.Color.White;

                    if (_firstCard == null)
                    {
                        _firstCard = _cardValues[clickedIndex].ToString();
                    }
                    else
                    {
                        _secondCard = _cardValues[clickedIndex].ToString();
                        CheckForMatch();
                    }
                };

                button.AutomationId = index.ToString();
                cardButtons.Add(button);
            }

            return cardButtons;
        }


        private int score = 0; // Переменная для отслеживания счета
        private int _numIncorrectMatches = 0; // Переменная для отслеживания количества неверных попыток

        private async void CheckForMatch()
        {
            if (_firstCard == _secondCard)
            {
                score += 5;
                _firstCard = null;
                _secondCard = null;
                _numCardsRevealed += 2;

                int pairsNeeded;

                if (_size == 2)
                    pairsNeeded = 4;
                else if (_size == 3)
                    pairsNeeded = 8;
                else if (_size == 4)
                    pairsNeeded = 16;
                else if (_size == 5)
                    pairsNeeded = 24;
                else if (_size == 6)
                    pairsNeeded = 36;
                else
                    pairsNeeded = _size * _size;

                if (_numCardsRevealed >= pairsNeeded)
                {
                    await ShowWinMessage();
                }
            }
            else
            {
                score -= 1;
                _numIncorrectMatches++;
                await Task.Delay(1000);
                foreach (var button in _cardButtons)
                {
                    if (button.Text == _firstCard || button.Text == _secondCard)
                    {
                        button.Text = "";
                        button.BackgroundColor = Xamarin.Forms.Color.Gray;
                    }
                }

                _firstCard = null;
                _secondCard = null;
            }
        }

        private async Task ShowWinMessage()
        {
            await Task.Delay(500); // Добавляем небольшую задержку перед отображением окна

            var alertResult = await DisplayAlert("Поздравляю!", $"Вы выиграли, {playerName}!\nВаши очки: {score}\nНеверные попытки: {_numIncorrectMatches}", "Начать заново", "Закончить игру");

            if (alertResult)
            {
                // Начать новую игру
                ResetGame();
            }
            else
            {
                await SaveResultAndExit();
            }
        }

        private void ResetGame()
        {
            _cardValues.Clear();
            _cardButtons.Clear();
            _firstCard = null;
            _secondCard = null;
            _numCardsRevealed = 0;
            score = 0;
            _numIncorrectMatches = 0;

            _cardValues.AddRange(GenerateCardValues());
            _cardButtons.AddRange(GenerateCardButtons());

            var grid = (Grid)Content;
            grid.Children.Clear();

            var random = new Random();
            foreach (var button in _cardButtons)
            {
                var row = random.Next(0, _size);
                var col = random.Next(0, _size);
                while (grid.Children.Any(x => Grid.GetRow(x) == row && Grid.GetColumn(x) == col))
                {
                    row = random.Next(0, _size);
                    col = random.Next(0, _size);
                }
                grid.Children.Add(button, col, row);
            }
        }

        private async Task SaveResultAndExit()
        {
            var result = new Result
            {
                PlayerName = playerName,
                Score = score,
                WrongAttempts = _numIncorrectMatches,
            };
            await _database.SaveResultAsync(result);

            await Navigation.PopAsync();
        }
    }
}