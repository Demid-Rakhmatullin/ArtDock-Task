using Game.Network;
using VContainer.Unity;

namespace Game.UI
{
    class StartScreenPresenter : IStartable
    {
        readonly StartScreenView _view;
        readonly GameNetworkManager _networkManager;

        public StartScreenPresenter(StartScreenView view, GameNetworkManager networkManager)
        {
            _view = view;
            _networkManager = networkManager;
        }

        void IStartable.Start()
        {
            //consider that view and presenter have the same lifetime
            _view.CreateBtn.onClick.AddListener(OnCreate);
            _view.JoinBtn.onClick.AddListener(OnJoin);
            _view.ExitBtn.onClick.AddListener(OnExit);
        }

        private void OnCreate()
        {
            _networkManager.StartHost();
        }

        private void OnJoin()
        {
            _networkManager.StartClient();
        }

        private void OnExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}
