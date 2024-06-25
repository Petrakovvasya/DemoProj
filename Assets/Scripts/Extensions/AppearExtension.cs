using System;
using System.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Extensions
{
    public static class AppearExtension
    {
        public static async Task SetActiveWithBounceAsync(this GameObject gameObject, bool isActive, float duration)
        {
            var origin = isActive ? Vector3.zero : Vector3.one;
            var target = isActive ? Vector3.one : Vector3.zero;
            var ease = isActive ? Ease.OutBack : Ease.InBack;

            gameObject.SetActive(true);
            gameObject.transform.localScale = origin;

            var tween = gameObject.transform.DOScale(target, duration)
                .SetEase(ease)
                .SetUpdate(true);

            try
            {
                await tween.AsyncWaitForCompletion();
            }
            catch (OperationCanceledException)
            {
                tween.Kill();
            }

            gameObject.OnDisableAsObservable()
                .Subscribe(_ => tween.Kill())
                .AddTo(gameObject);

            if (!isActive)
            {
                gameObject.SetActive(false);
            }
        }
    }
}