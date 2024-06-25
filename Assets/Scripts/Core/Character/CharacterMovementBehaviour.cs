using System;
using UniRx;
using UnityEngine;

namespace Demo.Core
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovementBehaviour : MonoBehaviour
    {
        private const float LookAtMagnitudeMinThreshold = 0.05f;
        private const float LookAtMagnitudeMaxThreshold = 2f;

        [SerializeField] private Joystick _joystick;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _rotationSpeed = 30f;
        [SerializeField] private float _movementSpeed = 3f;

        private readonly IReactiveProperty<bool> _walkingProperty =
            new ReactiveProperty<bool>();

        public IObservable<bool> WalkingListener => _walkingProperty;

        private void FixedUpdate()
        {
            var lookAt = Vector3.right * _joystick.Horizontal + Vector3.forward * _joystick.Vertical;

            lookAt.Normalize();

            var isMoving = lookAt.magnitude
                is > LookAtMagnitudeMinThreshold
                and <= LookAtMagnitudeMaxThreshold;

            _walkingProperty.Value = isMoving;

            if (!isMoving)
            {
                return;
            }

            var t = Time.fixedDeltaTime * _rotationSpeed;
            var r = Vector3.Lerp(_rb.transform.forward, lookAt, t);

            var tr = transform;
            tr.forward = r;

            _rb.MovePosition(tr.position + r * (_movementSpeed * Time.fixedDeltaTime));
        }
    }
}