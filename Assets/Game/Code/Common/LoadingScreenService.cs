using Game.Network;
using UnityEngine;
using VContainer.Unity;

namespace Game.Common
{
    public class LoadingScreenService : IInitializable
    {
        readonly IConnectionService _connection;

        LoadingScreenView _view;

        public LoadingScreenService(IConnectionService connection)
        {
            _connection = connection;
        }

        void IInitializable.Initialize()
        {
            _view = Object.FindAnyObjectByType<LoadingScreenView>();
            Object.DontDestroyOnLoad(_view);            
        }

        public void Show() => _view.Show();

        public void Hide() => _view.Hide();

        public void SubscribeNextSceneLoad()
        {
            _connection.OnSceneChangeStarted += Show;
            _connection.OnSceneChangeFinished += OnSceneChangeFinished;
        }

        private void OnSceneChangeFinished()
        {
            Hide();
            _connection.OnSceneChangeStarted -= Show;
            _connection.OnSceneChangeFinished -= OnSceneChangeFinished;
        }
    }
}
