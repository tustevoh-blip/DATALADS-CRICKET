using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DATALADS_CRICKET
{
    public partial class TeamSetup : Window
    {
        public TeamSetup()
        {
            InitializeComponent();
        }

        private void PlayersA_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var lines = TxtPlayersA.Text
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            LblCountA.Text = lines.Count.ToString();
            UpdateNextButton();
        }

        private void PlayersB_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var lines = TxtPlayersB.Text
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            LblCountB.Text = lines.Count.ToString();
            UpdateNextButton();
        }

        private void UpdateNextButton()
        {
            bool teamAok = !string.IsNullOrWhiteSpace(TxtTeamAName.Text)
                           && TxtPlayersA.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length >= 2;
            bool teamBok = !string.IsNullOrWhiteSpace(TxtTeamBName.Text)
                           && TxtPlayersB.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length >= 2;

            BtnNext.IsEnabled = teamAok && teamBok;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            // Save data to pass to the next page later
            var teamA = new TeamInfo
            {
                Name = TxtTeamAName.Text.Trim(),
                Players = TxtPlayersA.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            };

            var teamB = new TeamInfo
            {
                Name = TxtTeamBName.Text.Trim(),
                Players = TxtPlayersB.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            };

            MessageBox.Show($"Teams registered successfully:\n\n{teamA.Name} ({teamA.Players.Count} players)\n{teamB.Name} ({teamB.Players.Count} players)",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // TODO: navigate to MatchSetup window next
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Simple team data model
    public class TeamInfo
    {
        public string Name { get; set; }
        public List<string> Players { get; set; } = new List<string>();
    }
}


