using System;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Neptune.Core
{
    public class TaskProgressView : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;

        [SerializeField] private Image _fillImage;

        private Tweener _tweener;

        [UsedImplicitly]
        private void OnDisable() => _tweener?.Kill();

        public void Render(Sprite sprite) => _iconImage.sprite = sprite;

        public void SetValue(TimeSpan timeLeft, TimeSpan duration, TimeSpan loop)
        {
            var amount = 1.0f - (float)(timeLeft.TotalSeconds / duration.TotalSeconds);

            if (amount < _fillImage.fillAmount)
            {
                _fillImage.fillAmount = amount;
                return;
            }


            _tweener?.Kill();
            _tweener = _fillImage
                .DOFillAmount(amount, (float)loop.TotalSeconds)
                .SetEase(Ease.Linear);
        }

        public void SetFull()
        {
            _tweener?.Kill();
            _fillImage.fillAmount = 1.0f;
        }
    }
}