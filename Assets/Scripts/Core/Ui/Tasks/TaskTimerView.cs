using System;
using Demo.Services;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Moon.Core
{
    public class TaskTimerView : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;

        [SerializeField] private Image _fillImage;

        [SerializeField] private TMP_Text _timerText;

        [SerializeField] private TMP_Text _amountText;

        [SerializeField] private VirtualClockService _timeService;

        private Tweener _tweener;

        [UsedImplicitly]
        private void OnDisable()
        {
            _tweener?.Kill();
        }

        public void Render(Sprite sprite, int amount)
        {
            _iconImage.sprite = sprite;

            _amountText.gameObject.SetActive(amount > 0);
            _amountText.text = $"{amount}";
        }

        public void SetValue(TimeSpan timeLeft, TimeSpan duration, TimeSpan loop)
        {
            var amount = 1.0f - (float)(timeLeft.TotalSeconds / duration.TotalSeconds);

            _timerText.text = _timeService.FormatTimeSpanDefault(timeLeft);

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
            _timerText.text = "Ready";
        }
    }
}