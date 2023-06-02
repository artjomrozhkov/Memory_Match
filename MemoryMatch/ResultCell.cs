using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MemoryMatch
{
    public class ResultCell : ViewCell
    {
        public ResultCell()
        {
            var placeLabel = new Label
            {
                FontSize = 18,
                TextColor = Color.Black
            };
            placeLabel.SetBinding(Label.TextProperty, new Binding("Place", stringFormat: "{0}."));

            var nameLabel = new Label
            {
                FontSize = 18,
                TextColor = Color.Black
            };
            nameLabel.SetBinding(Label.TextProperty, new Binding("PlayerName"));

            var scoreLabel = new Label
            {
                FontSize = 16,
                TextColor = Color.FromHex("#CCCCCC")
            };
            scoreLabel.SetBinding(Label.TextProperty, new Binding("Score", stringFormat: "Score: {0}"));

            var wrongAttemptsLabel = new Label
            {
                FontSize = 16,
                TextColor = Color.FromHex("#CCCCCC")
            };
            wrongAttemptsLabel.SetBinding(Label.TextProperty, new Binding("WrongAttempts", stringFormat: "Wrong Attempts: {0}"));

            var layout = new StackLayout
            {
                Padding = new Thickness(10),
                Spacing = 5,
                Orientation = StackOrientation.Horizontal,
                Children = { placeLabel, nameLabel, scoreLabel, wrongAttemptsLabel }
            };
            View = layout;
        }
    }
}
