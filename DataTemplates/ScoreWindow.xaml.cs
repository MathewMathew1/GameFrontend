using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using BoardGameFrontend.Helpers;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Windows
{
    public partial class ScoreWindow : Window
    {
        public EndOfGameWithPlayers PlayerScores { get; set; }
        public ScoreWindow(EndOfGameWithPlayers playerScores)
        {
            PlayerScores = playerScores;
            DataContext = this;
            InitializeComponent();
            UpdateScoreTable(playerScores);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void UpdateScoreTable(EndOfGameWithPlayers playerScores)
        {
            TableRowGroup rowGroup = new TableRowGroup();

            foreach (var playerScore in playerScores.PlayerScores)
            {
                TableRow newRow = new TableRow();

                var playerColorBrush = new SolidColorBrush(playerScore.PlayerColor);
                TextBlock textBlock = new TextBlock
                {
                    Text = playerScore.Player.Name,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Wrap TextBlock inside Paragraph
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(textBlock);

                // Create TableCell and apply style
                TableCell playerCell = new TableCell(paragraph)
                {
                    Style = (Style)FindResource("TableCellStyle")
                };

                // Optionally, set a background color based on player color
                playerCell.Background = new SolidColorBrush(playerScore.PlayerColor);

                TableCell moraleCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.MoralePoints.Points} ({playerScore.MoralePoints.Power})"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                // Mercenary Points Cell
                TableCell mercenaryCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.MercenaryPoints}"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                // Oracle Points Cell
                TableCell oracleCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.OraclePoints}"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                // Hero Points Cell
                TableCell heroCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.HeroPoints}"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };


                TableCell artifactCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.ArtefactPoints}"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                TableCell royalCardCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.RoyalCardPoints}"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                TableCell tokenCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.TokenPoints}"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                // Army Points Cell
                TableCell armyPointsCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.ArmyPoints.Points} ({playerScore.ArmyPoints.Power})"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                // Siege Points Cell
                TableCell siegePointsCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.SiegePoints.Points} ({playerScore.SiegePoints.Power})"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                // Magic Points Cell
                TableCell magicPointsCell = new TableCell(new Paragraph(new Run(
                    $"{playerScore.MagicPoints.Points} ({playerScore.MagicPoints.Power})"
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                // Total Points Cell
                TableCell totalPointsCell = new TableCell(new Paragraph(new Run(
                    playerScore.PointsOverall.ToString()
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                var playerTime = playerScores.PlayerTimeSpan[playerScore.Player.Id];
                TableCell timeCell = new TableCell(new Paragraph(new Run(
                    TimeFormatHelper.FormatTimeSpan(playerTime)
                )))
                {
                    Style = (Style)FindResource("TableCellStyle") // Apply the style
                };

                newRow.Cells.Add(playerCell);
                newRow.Cells.Add(totalPointsCell);
                newRow.Cells.Add(moraleCell);
                newRow.Cells.Add(armyPointsCell);
                newRow.Cells.Add(siegePointsCell);
                newRow.Cells.Add(magicPointsCell);
                newRow.Cells.Add(mercenaryCell);
                newRow.Cells.Add(oracleCell);
                newRow.Cells.Add(heroCell);
                newRow.Cells.Add(artifactCell);
                newRow.Cells.Add(tokenCell);
                newRow.Cells.Add(royalCardCell);
                newRow.Cells.Add(timeCell);
                // Add the new row to the TableRowGroup
                rowGroup.Rows.Add(newRow);
            }

            // Add the TableRowGroup to the Table
            PlayerScoreTableRows.RowGroups.Add(rowGroup);
        }


    }
}