using UniRx;
using UnityEngine;

namespace Demo.Core
{
    public class JoystickBehaviour : MonoBehaviour
    {
        [SerializeField] private CharacterTasksBehaviour _characterTasksBehaviour;
        [SerializeField] private Joystick _joystick;
        
        private void Start()
        {
            _characterTasksBehaviour.GameEndedListener
                .Subscribe(OnGameEnded)
                .AddTo(this);
        }

        private void OnGameEnded(Unit unit)
        {
            _joystick.OnPointerUp(default);
            
            gameObject.SetActive(false);
        }
    }
}