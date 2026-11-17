using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    class CanvasGroupView : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] bool hideOnStart;

        public bool IsShown { get; private set; }

        private void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (hideOnStart)
                Hide();
            else
                Show();
        }

        public void Show()
        {
            if (!IsShown)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
                IsShown = true;
            }
        }

        public void Hide()
        {
            if (IsShown)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                IsShown = false;
            }
        }
    }
}
