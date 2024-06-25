using Core.Bonuses;
using DG.Tweening;
using UnityEngine;

namespace Demo.Core
{
    public class CharacterBonusesBehaviour : MonoBehaviour
    {
        private const float FlyDuration = 0.4f;
        private const float ScaleAmount = 0.1f;
        private const float RotationAmount = 360;

        //[Inject]
        //private IBonusService _defaultBonusesService; TODO: Inject

        public void AnimateReceiveBonus(BonusBehaviour bonus)
        {
            bonus.DisableCollisions();
            
            DOTween.Sequence()
                .Join(bonus.transform.DOJump(transform.position, 4, 1, FlyDuration))
                .Join(bonus.transform.DOScale(Vector3.one * ScaleAmount, FlyDuration))
                .Join(bonus.transform.DORotate(new Vector3(0, RotationAmount, 0), FlyDuration,
                    RotateMode.FastBeyond360))
                .OnComplete(() => { Destroy(bonus.gameObject); });
        }
    }
}