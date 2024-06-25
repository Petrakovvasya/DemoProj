using System;
using System.Threading.Tasks;
using DG.Tweening;
using Models.Bonuses;
using Services.Bonuses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui
{
    public class NewBonusReceivedView : MonoBehaviour
    {
        private const string NewBonusFormat = "+{0}";

        [SerializeField] private TMP_Text _newBonusText;
        [SerializeField] private Image _newBonusImage;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Vector3 _appearMoveOffset = Vector3.up;
        [SerializeField] private float _appearMoveDuration = 0.25f;
        [SerializeField] private Vector3 _appearStartScale = Vector3.zero;
        [SerializeField] private Vector3 _appearFinalScale = Vector3.one;
        [SerializeField] private float _appearSizeDuration = 0.35f;
        [SerializeField] private Ease _appearSizeEase = Ease.OutBack;
        [SerializeField] private float _appearStartAlpha;
        [SerializeField] private float _appearFinalAlpha = 1f;
        [SerializeField] private float _appearAlphaDuration = 0.2f;
        [SerializeField] private float _disappearDelay = 1.5f;
        [SerializeField] private float _disappearStartAlpha = 1f;
        [SerializeField] private float _disappearFinalAlpha;
        [SerializeField] private float _disappearAlphaDuration = 0.2f;

        //[Inject] 
        //private IBonusService _bonusesService;

        public void Render(Bonus bonus)
        {
            _newBonusText.text = string.Format(NewBonusFormat, bonus.Amount);
            _newBonusImage.sprite = DefaultBonusesService.Instance.FindSprite(bonus.Type); //TODO: inject

            PlayAnimation();
        }

        private async void PlayAnimation()
        {
            transform.DOLocalMove(_appearMoveOffset, _appearMoveDuration);
            PlayScaleAnimation(_appearStartScale, _appearFinalScale, _appearSizeDuration);
            PlayAlphaAnimation(_appearStartAlpha, _appearFinalAlpha, _appearAlphaDuration);

            await Task.Delay(TimeSpan.FromSeconds(_disappearDelay));

            PlayAlphaAnimation(_disappearStartAlpha, _disappearFinalAlpha, _disappearAlphaDuration,
                () => { Destroy(gameObject); });
        }

        private void PlayScaleAnimation(Vector3 start, Vector3 end, float duration)
        {
            transform.localScale = start;
            transform.DOScale(end, duration)
                .SetEase(_appearSizeEase);
        }

        private void PlayAlphaAnimation(float start, float end, float duration) =>
            PlayAlphaAnimation(start, end, duration, null);

        private void PlayAlphaAnimation(float start, float end, float duration,
            TweenCallback onComplete)
        {
            if (_canvasGroup == null)
            {
                return;
            }

            _canvasGroup.alpha = start;

            var tween = DOTween.To(() => _canvasGroup.alpha,
                x => _canvasGroup.alpha = x,
                end,
                duration);

            if (onComplete != null)
            {
                tween.onComplete += onComplete;
            }

            tween.Play();
        }
    }
}