using System;
using Core.Bonuses;
using Models;
using Models.Bonuses;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Demo.Core
{
    public class CharacterPhysicsBehaviour : MonoBehaviour
    {
        [SerializeField] private Collider _bonusesCollider;
        [SerializeField] private CharacterUiBehaviour _characterUiBehaviour;
        [SerializeField] private CharacterBonusesBehaviour _characterBonusesBehaviour;

        private int _cheeseAmount;

        private readonly ISubject<Bonus> _newBonusReceived = new Subject<Bonus>();

        public IObservable<Bonus> BonusReceivedListener => _newBonusReceived;

        private void Start()
        {
            _bonusesCollider.OnTriggerEnterAsObservable()
                .Subscribe(OnBonusTriggered)
                .AddTo(this);
        }

        private void OnBonusTriggered(Collider other)
        {
            if (!other.CompareTag("Bonus"))
            {
                return; //TODO: handle different bonuses
            }

            var b = other.GetComponent<BonusBehaviour>();

            _characterUiBehaviour.EnqueueBonus(new Bonus(BonusType.Cheese, 1));
            _characterBonusesBehaviour.AnimateReceiveBonus(b);

            _newBonusReceived?.OnNext(new Bonus(BonusType.Cheese, ++_cheeseAmount));
        }
    }
}