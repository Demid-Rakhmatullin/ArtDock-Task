using Game.Common;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    [DisallowMultipleComponent]
    class PlayersListViewRow : CanvasGroupView
    {
        [field: SerializeField]
        public TMP_Text Number { get; private set; }

        [field: SerializeField]
        public TMP_Text Name { get; private set; }
    }
}
