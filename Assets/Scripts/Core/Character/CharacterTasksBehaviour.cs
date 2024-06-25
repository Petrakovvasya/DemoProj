using System;
using Models.Bonuses;
using UniRx;
using UnityEngine;

namespace Demo.Core
{
    public class CharacterTasksBehaviour : MonoBehaviour
    {
        [SerializeField] private CharacterPhysicsBehaviour _characterPhysicsBehaviour;
        [SerializeField] private int _finalCount = 10;

        private int _currentCount;

        private readonly ISubject<Unit> _gameEnded = new Subject<Unit>();

        public IObservable<Unit> GameEndedListener => _gameEnded;

        private void Start()
        {
            _characterPhysicsBehaviour.BonusReceivedListener
                .Subscribe(OnBonusReceived)
                .AddTo(this);
        }

        private void OnBonusReceived(Bonus bonus)
        {
            _currentCount++;

            if (_currentCount >= _finalCount)
            {
                _gameEnded.OnNext(Unit.Default);
            }
        }
    }
}