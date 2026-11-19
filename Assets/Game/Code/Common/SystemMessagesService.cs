using System.Runtime.CompilerServices;
using UnityEngine;
using VContainer.Unity;

namespace Game.Common
{
    public class SystemMessagesService : ITickable
    {
        const float MESSAGE_SHOW_TIME = 3f;

        bool _messageShown;
        float _messageTimer;

        public void DisplayMessage(string message)
        {
            var view = FindView();
            if (view)
            {
                view.Text.text = message;
                view.Show();

                _messageShown = true;
                _messageTimer = MESSAGE_SHOW_TIME;
            }
        }

        public void Tick()
        {
            if (_messageShown)
            {
                _messageTimer -= Time.deltaTime;
                if (_messageTimer < 0f)
                {
                    _messageShown = false;

                    var view = FindView();
                    if (view)
                    {
                        view.Text.text = string.Empty;
                        view.Hide();
                    }
                }
            }
        }

        //todo: create view dynamically if it not exists on current scene? or keep it in DDOL?
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SystemMessageView FindView()
            => Object.FindAnyObjectByType<SystemMessageView>();
    }
}
