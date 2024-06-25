using System;
using System.Linq;
using JetBrains.Annotations;
using Models;
using UnityEngine;

namespace Services.Bonuses
{
    [Serializable]
    internal class InternalBonus
    {
        public BonusType Type;

        [CanBeNull] public Sprite Sprite;

        [CanBeNull] public GameObject Object;
    }

    public class DefaultBonusesService : MonoBehaviour, IBonusService
    {
        [SerializeField] private InternalBonus[] _bonuses;

        public static DefaultBonusesService Instance { get; private set; }

        [UsedImplicitly]
        private void Awake()
        {
            if (Instance != null && this != Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
        }

        public Sprite FindSprite(BonusType type) =>
            _bonuses.FirstOrDefault(d => d.Type == type)?.Sprite;

        public GameObject FindObject(BonusType type) =>
            _bonuses.FirstOrDefault(d => d.Type == type)?.Object;
    }
}