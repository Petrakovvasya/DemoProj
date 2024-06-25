using JetBrains.Annotations;
using UnityEngine;

namespace Helpers
{
    public class UiOverlayPositionHelper : MonoBehaviour
    {
        private const int Offset = 500;
        private const float SmoothModifier = 0.5f;

        [SerializeField] private Transform _uiRoot;

        [SerializeField] private Vector3 _screenOffset;

        private Camera _mainCamera;

        public Transform Target { get; set; }

        [UsedImplicitly]
        private void Start() => _uiRoot.gameObject.SetActive(false);

        [UsedImplicitly]
        private void FixedUpdate()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            var t = Target == null ? transform.parent : Target;
            var screenPosition = _mainCamera!.WorldToScreenPoint(t.position);

            if (screenPosition.x < -Offset
                || screenPosition.x > Screen.width + Offset
                || screenPosition.y < -Offset
                || screenPosition.y > Screen.height + Offset)
            {
                _uiRoot.gameObject.SetActive(false);
                return;
            }

            _uiRoot.gameObject.SetActive(true);

            var smoothPosition = Vector3.Lerp(_uiRoot.transform.position,
                screenPosition - _screenOffset,
                SmoothModifier);

            _uiRoot.transform.position = smoothPosition;
        }
    }
}