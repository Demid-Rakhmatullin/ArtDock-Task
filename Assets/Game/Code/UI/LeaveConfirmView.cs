using Game.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [DisallowMultipleComponent]
    class LeaveConfirmView : CanvasGroupView
    {
        [field: SerializeField]
        public Button YesBtn { get; private set; }

        [field: SerializeField]
        public Button NoBtn { get; private set; }
    }
}
