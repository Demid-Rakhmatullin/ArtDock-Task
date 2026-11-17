using Game.Gameplay;
using System;
using Unity.Netcode;
using UnityEngine.InputSystem;
using VContainer.Unity;
using static Game.Global.InputGlobalActions;
using static UnityEngine.InputSystem.InputAction;

namespace Game.UI
{
    class LeaveConfirmPresenter : IStartable, IDisposable
    {
        readonly LeaveConfirmView _view;
        readonly NetworkManager _networkManager;
        readonly InputActionAsset _input;
        readonly GameplayData _gpData;

        public LeaveConfirmPresenter(LeaveConfirmView view, NetworkManager networkManager,
            InputActionAsset input, GameplayData gpData)
        {
            _view = view;
            _networkManager = networkManager;
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
            _networkManager.Shutdown();
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
