using System;

namespace Game.Gameplay
{
    public class GameplayData
    {
        //todo: use UniRx?
        //

        public event Action<GameplayState> OnStateChanged;

        GameplayState _state;
        public GameplayState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged?.Invoke(value);
                }
            }
        }
    }
}
