using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Demo.Core
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimatorBehaviour : MonoBehaviour
    {
        private const float AnimationCrossFade = 0.01f;
        private static readonly int IdleState = Animator.StringToHash("Idle");
        private static readonly int WalkingState = Animator.StringToHash("Walking");
        private static readonly int DancingState = Animator.StringToHash("Dancing");

        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterMovementBehaviour _characterMovement;
        [SerializeField] private CharacterTasksBehaviour _characterTasks;
        [SerializeField] private ParticleSystem _stepLeftVfx;
        [SerializeField] private ParticleSystem _stepRightVfx;

        private bool _isGameEnded;

        private void Awake()
        {
            _characterMovement.WalkingListener
                .Subscribe(OnWalkingChanged)
                .AddTo(this);

            _characterTasks.GameEndedListener
                .Subscribe(OnGameEnded)
                .AddTo(this);
        }

        private void OnWalkingChanged(bool isWalking)
        {
            if (_isGameEnded)
            {
                return;
            }

            if (!isWalking)
            {
                _animator.CrossFade(IdleState, AnimationCrossFade);
                return;
            }

            _animator.CrossFade(WalkingState, AnimationCrossFade);
        }

        private void OnGameEnded(Unit unit)
        {
            _isGameEnded = true;
            _animator.CrossFade(DancingState, AnimationCrossFade);
        }

        [UsedImplicitly]
        public void OnStepLeftHandler() =>
            _stepLeftVfx.Play();

        [UsedImplicitly]
        public void OnStepRightHandler() =>
            _stepRightVfx.Play();
    }
}