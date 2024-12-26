using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;
using BoardGameFrontend.ChatLogManager;
using System;
using System.Windows.Threading;

namespace BoardGameFrontend.SignalHub
{
    public class ArtifactsListener : IDisposable
    {
        private readonly HubConnection _connection;
        private bool _disposed;

        public ArtifactsListener(HubConnection connection, Game game, MainChatLogManager mainChatLogManager, Dispatcher dispatcher)
        {
            _connection = connection;
            _disposed = false;

            _connection.On<ArtifactPlayed>("ArtifactPlayed", (artifactPlayed) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.PlayArtifact(artifactPlayed);
                    mainChatLogManager.ArtifactChatLogManager.ArtifactPlayed(artifactPlayed);
                });
            });

            _connection.On<ArtifactPlayed>("ArtifactRePlayed", (artifactPlayed) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.RePlayArtifact(artifactPlayed);
                    mainChatLogManager.ArtifactChatLogManager.ArtifactRePlayed(artifactPlayed);
                });
            });

            _connection.On<ArtifactToPickFromData>("ArtifactsToPickFrom", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.AddPossibleArtifacts(data);
                    mainChatLogManager.ArtifactChatLogManager.ArtifactsToPickFrom(data);
                });
            });

            _connection.On<ArtifactToPickFromData>("ArtifactsGiven", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.AddArtifacts(data);
                    mainChatLogManager.ArtifactChatLogManager.ArtifactsGiven(data);
                });
            });

            _connection.On<ArtifactToPickFromDataForOtherUsers>("ArtifactsGivenToOtherPlayer", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.AddArtifacts(data);
                    mainChatLogManager.ArtifactChatLogManager.ArtifactToPickFromDataForOtherUsers(data);
                });
            });

            _connection.On<ArtifactTakenData>("ArtifactsTaken", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.AddTakenArtifacts(data);
                    mainChatLogManager.ArtifactChatLogManager.ArtifactTakenData(data);
                });
            });

            _connection.On<ArtifactTakenData>("ArtifactsTakenOtherData", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.AddTakenArtifacts(data);              
                });
            });

            _connection.On<ArtifactToPickFromDataForOtherUsers>("ArtifactSummary", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    mainChatLogManager.ArtifactChatLogManager.ArtifactSummary(data);   
                });
            });

            _connection.On<ArtifactRerolledData>("ArtifactRerolled", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.RerollArtifact(data);
                    mainChatLogManager.ArtifactChatLogManager.ArtifactRerolled(data);
                });
            });

            _connection.On<ArtifactRerolledDataForOtherUsers>("ArtifactRerolledOtherData", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    mainChatLogManager.ArtifactChatLogManager.ArtifactRerolledOtherData(data);
                });
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _connection.Remove("ArtifactPlayed");
                _connection.Remove("ArtifactRePlayed");
                _connection.Remove("ArtifactsToPickFrom");
                _connection.Remove("ArtifactsGiven");
                _connection.Remove("ArtifactsGivenToOtherPlayer");
                _connection.Remove("ArtifactsTaken");
                _connection.Remove("ArtifactsTakenOtherData");
                _connection.Remove("ArtifactSummary");
                _connection.Remove("ArtifactRerolled");
                _connection.Remove("ArtifactRerolledOtherData");
            }

            _disposed = true;
        }
    }
}
