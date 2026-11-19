using Game.Gameplay;
using Game.Network;
using System;
using UnityEngine.InputSystem;
using VContainer.Unity;
using static Game.Common.InputGlobalActions;
using static UnityEngine.InputSystem.InputAction;

namespace Game.UI
{
    class LeaveConfirmPresenter : IStartable, IDisposable
    {
        readonly LeaveConfirmView _view;
        readonly IConnectionService _connection;
        readonly InputActionAsset _input;
        readonly GameplayData _gpData;

        public LeaveConfirmPresenter(LeaveConfirmView view, IConnectionService connection,
            InputActionAsset input, GameplayData gpData)
        {
            _view = view;
            _connection = connection;
            _input = input;
            _gpData = gpData;
        }

        void IStartable.Start()
        {
            _input.FindAction(CANCEL).started += OnPressedCancel;

            //consider that view and presenter have the same lifetime
            _view.YesBtn.onClick.AddListener(Leave);
            _view.NoBtn.onClick.AddListener(Hide);
        }

        private void OnPressedCancel(CallbackContext obj)
        {
            if (!_view.IsShown)
            {
                _gpData.State = GameplayState.Pause;
                _view.Show();
            }
            else
                Hide();
        }

        private void Leave()
        {
            _connection.Shutdown();
        }

        private void Hide()
        {
            _gpData.State = GameplayState.Active;
            _view.Hide();
        }

        void IDisposable.Dispose()
        {
            if (_input)
                _input.FindAction(CANCEL).started -= OnPressedCancel;
        }
    }
}
