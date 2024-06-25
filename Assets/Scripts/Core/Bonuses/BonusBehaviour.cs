using Extensions;
using UnityEngine;

namespace Core.Bonuses
{
    public class BonusBehaviour : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private float _appearDelay = 0.25f;

        private async void Start()
        {
            await gameObject.SetActiveWithBounceAsync(true, _appearDelay);
        }

        public void DisableCollisions() =>
            _collider.enabled = false;
    }
}