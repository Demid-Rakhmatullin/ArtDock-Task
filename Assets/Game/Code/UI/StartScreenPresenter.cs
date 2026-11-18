using Game.Network;
using VContainer.Unity;

namespace Game.UI
{
    class StartScreenPresenter : IStartable
    {
        readonly StartScreenView _view;
        readonly IConnectionService _connection;

        public StartScreenPresenter(StartScreenView view, IConnectionService connection)
        {
            _view = view;
            _connection = connection;
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
            _connection.StartHost();
        }

        private void OnJoin()
        {
            _connection.StartClient();
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
