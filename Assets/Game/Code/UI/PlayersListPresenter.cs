using Game.Network;
using System;
using UnityEngine.InputSystem;
using VContainer.Unity;
using static Game.Common.InputGlobalActions;

namespace Game.UI
{
    class PlayersListPresenter : IStartable, ITickable, IDisposable
    {
        readonly PlayersListView _view;
        readonly InputActionAsset _input;
        readonly INetworkClientsService _players;
        readonly bool _showByKey; //todo: more graceful approach to reuse code in Lobby and Gameplay list

        PlayersListViewRow[] _rowViews;
        InputAction _tabInput;

        public PlayersListPresenter(PlayersListView view, InputActionAsset input,
            INetworkClientsService players, bool showByKey)
        {
            _view = view;
            _input = input;
            _players = players;
            _showByKey = showByKey;
        }

        void IStartable.Start()
        {
            //todo: row prefabs pooling/spawn?
            _rowViews = _view
                .ListContainer
                .GetComponentsInChildren<PlayersListViewRow>();

            if (_showByKey)
                _tabInput = _input.FindAction(TAB);
            else
            {
                RefreshList();
                _view.Show();
            }

            _players.OnClientsUpdated += OnPlayersUpdated;
        }

        void ITickable.Tick()
        {
            if (!_showByKey)
                return;

            if (_tabInput.IsPressed() && !_view.IsShown)
            {
                RefreshList();
                _view.Show();
            }
            else if (!_tabInput.IsPressed() && _view.IsShown)
                _view.Hide();
        }

        private void OnPlayersUpdated()
        {
            if (_view.IsShown)
                RefreshList();
        }

        private void RefreshList()
        {
            for (int i = 0; i < _rowViews.Length; i++)
            {
                var row = _rowViews[i];

                if (i < _players.List.Count)
                {
                    var player = _players.List[i];
                    row.Number.text = (i + 1).ToString();
                    row.Name.text = player.Name;
                    row.SteamId.text = player?.SteamId.ToString();

                    row.Show();
                }
                else
                    row.Hide();
            }
        }

        void IDisposable.Dispose()
        {
            _players.OnClientsUpdated -= OnPlayersUpdated;
        }
    }
}
