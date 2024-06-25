using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Ui;
using Models.Bonuses;
using UnityEngine;

namespace Demo.Core
{
    public class CharacterUiBehaviour : MonoBehaviour
    {
        [SerializeField] private NewBonusReceivedView _newBonusReceivedView;
        [SerializeField] private Transform _newBonusAddedParent;
        [SerializeField] private float _newCollectableAppearDelay = 0.1f;

        private readonly Queue<Bonus> _bonusQueue = new Queue<Bonus>();
        private bool _isRendering;

        public void EnqueueBonus(Bonus bonus)
        {
            _bonusQueue.Enqueue(bonus);
            if (!_isRendering)
            {
                RenderBonuses();
            }
        }

        private async void RenderBonuses()
        {
            _isRendering = true;

            while (_bonusQueue.Count > 0)
            {
                var view = Instantiate(_newBonusReceivedView, _newBonusAddedParent);
                view.Render(_bonusQueue.Dequeue());

                await Task.Delay(TimeSpan.FromSeconds(_newCollectableAppearDelay));
            }

            _isRendering = false;
        }
    }
}