using Game.Common;
using TMPro;
using UnityEngine;

class SystemMessageView : CanvasGroupView
{
    [field: SerializeField] 
    public TMP_Text Text {  get; private set; }
}
