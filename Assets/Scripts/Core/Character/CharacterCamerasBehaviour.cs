using UniRx;
using UnityEngine;

namespace Demo.Core
{
    public class CharacterCamerasBehaviour : MonoBehaviour
    {
        [SerializeField] private CharacterTasksBehaviour _characterTasks;
        [SerializeField] private GameObject _movingCamera;
        [SerializeField] private GameObject _dancingCamera;

        private void Start()
        {
            _characterTasks.GameEndedListener
                .Subscribe(OnGameEnded)
                .AddTo(this);
        }

        private void OnGameEnded(Unit unit)
        {
            _movingCamera.SetActive(false);
            _dancingCamera.SetActive(true);
        }
    }
}