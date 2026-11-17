using Unity.Netcode;
using UnityEngine;

namespace Game.Interactions
{
    [DisallowMultipleComponent]
    class InteractableNetwork : NetworkBehaviour
    {
        [SerializeField] Rigidbody rb;

        bool _isHidden;

        public override void OnNetworkSpawn()
        {
            if (IsServer && !gameObject.TryGetComponent<InteractableServerData>(out _))
                gameObject.AddComponent<InteractableServerData>();
        }

        protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
        {
            if (NetworkObject.InScenePlaced && _isHidden)
                Show();
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkObject.InScenePlaced)
                Hide();
        }

        private void Show()
        {
            if (TryGetComponent<Collider>(out var collider))
                collider.enabled = true;

            foreach (Transform child in transform)
                child.gameObject.SetActive(true);

            _isHidden = false;
        }

        private void Hide()
        {
            rb.isKinematic = true;

            if (TryGetComponent<Collider>(out var collider))
                collider.enabled = false;

            foreach (Transform child in transform)
                child.gameObject.SetActive(false);

            _isHidden = true;
        }
    }
}
