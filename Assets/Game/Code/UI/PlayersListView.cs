using Game.Common;
using UnityEngine;

namespace Game.UI
{
    [DisallowMultipleComponent]
    class PlayersListView : CanvasGroupView
    {
        [field: SerializeField]
        public RectTransform ListContainer { get; private set; }
    }
}
