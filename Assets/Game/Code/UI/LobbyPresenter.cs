using Game.Common;
using Game.Network;
using VContainer.Unity;

namespace Game.UI
{
    class LobbyPresenter : IStartable
    {
        readonly LobbyView _view;
        readonly IConnectionService _connection;
        readonly LoadingScreenService _loadingScreen;

        public LobbyPresenter(LobbyView view, IConnectionService connection, 
            LoadingScreenService loadingScreen)
        {
            _view = view;
            _connection = connection;
            _loadingScreen = loadingScreen;
        }

        void IStartable.Start()
        {
            if (_connection.IsHost)
            {
                _view.StartBtn.gameObject.SetActive(true);
                //consider that view and presenter have the same lifetime
                _view.StartBtn.onClick.AddListener(OnStart);
            }
            else
                _view.StartBtn.gameObject.SetActive(false);

            _loadingScreen.SubscribeNextSceneLoad();
        }

        public void OnStart()
        {
            _connection.StartGameplay();
        }
    }
}
