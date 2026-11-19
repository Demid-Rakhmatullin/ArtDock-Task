using TMPro;
using UnityEngine;

namespace Game.Common
{
    [DisallowMultipleComponent]
    class SystemMessageView : CanvasGroupView
    {
        [field: SerializeField]
        public TMP_Text Text { get; private set; }
    }
}
