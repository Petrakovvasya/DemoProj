using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Demo.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.Core
{
    public class BoxItemsView : MonoBehaviour
    {
        [Serializable]
        private class ScaleAnimationVariant
        {
            [SerializeField] private Vector3 _finalScale;

            [SerializeField] private float _duration;

            [SerializeField] private Ease _easeType;

            [SerializeField] private bool _isParticleAppear;

            [SerializeField] private ParticleSystem _particle;

            [SerializeField] private bool _shouldStartItemsAnim;

            public bool IsParticleAppear => _isParticleAppear;

            public bool ShouldStartItemsAnim => _shouldStartItemsAnim;

            public float Duration => _duration;

            public Vector3 FinalScale => _finalScale;

            public Ease EaseType => _easeType;

            public ParticleSystem Particle => _particle;
        }

        private const int ItemJumpAppearCount = 1;

        [SerializeField] private ItemType _type;

        [SerializeField] private GameObject[] _itemsVisual;

        [SerializeField] [Min(0f)] private float _itemFlyHeightMin = 0.1f;

        [SerializeField] [Min(0f)] private float _itemFlyHeightMax = 0.3f;

        [SerializeField] private float _itemFlyDuration = 0.2f;

        [SerializeField] private ScaleAnimationVariant[] _showAnimationSequence;

        [SerializeField] private ScaleAnimationVariant[] _hideAnimationSequence;

        public ItemType Type => _type;

        public void ShowBoxImmediately()
        {
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
        }

        public void HideBoxImmediately() => transform.localScale = Vector3.zero;

        public async void ShowBox()
        {
            HideBoxImmediately();

            await PlayAnimationSequence(_showAnimationSequence);
        }

        public async Task HideBox() => await PlayAnimationSequence(_hideAnimationSequence);

        private Task PlayAnimationSequence(IEnumerable<ScaleAnimationVariant> animationSequence)
        {
            foreach (var animVariant in animationSequence)
            {
                if (animVariant.IsParticleAppear)
                {
                    animVariant.Particle.Play();
                }

                transform
                    .DOScale(animVariant.FinalScale, animVariant.Duration)
                    .SetEase(animVariant.EaseType);

                if (animVariant.ShouldStartItemsAnim)
                {
                    AnimateItems();
                }
            }

            return Task.CompletedTask;
        }

        private void AnimateItems()
        {
            foreach (var item in _itemsVisual)
            {
                item.transform.DOLocalJump(item.transform.localPosition,
                    Random.Range(_itemFlyHeightMin, _itemFlyHeightMax),
                    ItemJumpAppearCount, _itemFlyDuration);
            }
        }
    }
}