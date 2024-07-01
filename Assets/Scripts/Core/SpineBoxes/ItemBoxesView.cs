using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Demo.Models;
using JigglePhysics;
using UnityEngine;

namespace Demo.Core
{
    public class ItemBoxesView : MonoBehaviour
    {
        private const string BoxBoneName = "BoxBone";
        private const float BoxSpawnOffset = 0.25f;
        private const int DefaultBoxesCount = 1;

        [SerializeField] private BoxItemsView _boxPrefab;

        [SerializeField] private JiggleRigBuilder _jiggleRigBuilder;

        [SerializeField] private JiggleSettings _jiggleSettings;

        [SerializeField] [Range(0f, 1f)] private float _ignoreBoxesJigglePercent = 0.5f;

        [SerializeField] private Transform _boxPivot;

        [SerializeField] private Vector3 _startSpawnOffset;

        [SerializeField] private Vector3 _boxesRotation;

        [SerializeField] private int _maxBoxesCount = 15;

        [SerializeField] private int _itemsPerBox = 10;

        [SerializeField] private float _moveToHandsFromCounterDuration = 0.1f;

        [SerializeField] private float _moveToHandsBoxDelay = 0.1f;

        [SerializeField] private float _boxDisappearMoveOthersDuration = 0.1f;

        private readonly List<BoxItemsView> _boxBehaviours = new List<BoxItemsView>();
        private BoxItemsView _currentBox;
        private Transform[] _boxBones;
        private Transform _previousBoxBone;

        public int BoxesCount => _boxBehaviours.Count;

        public Vector3 BoxesPosition =>
            _boxBehaviours.Count != 0
                ? _boxBehaviours.Last().transform.position + _startSpawnOffset
                : transform.position + _startSpawnOffset;

        private void Start()
        {
            transform.eulerAngles = _boxesRotation;
        }

        public async Task PlayBoxesMoveAnimation(Vector3 finalMoveOffset)
        {
            foreach (var boxBehaviour in _boxBehaviours.ToList()
                         .Where(boxBehaviour => boxBehaviour != null))
            {
                boxBehaviour.transform.DOLocalMove(finalMoveOffset, _moveToHandsFromCounterDuration);

                await Task.Delay(TimeSpan.FromSeconds(_moveToHandsBoxDelay));
            }
        }

        public void Render(IEnumerable<Item> items)
        {
            _jiggleRigBuilder.gameObject.SetActive(true);
            if (_boxBones == null)
            {
                SpawnBoxesBones();
            }

            if (items == null)
            {
                ClearBoxes();
            }
            else
            {
                UpdateBoxes(items);
            }
        }

        private void SpawnBoxesBones()
        {
            _boxBones = new Transform[_maxBoxesCount];

            for (var i = 0; i < _maxBoxesCount; i++)
            {
                var bone = new GameObject { name = BoxBoneName + i }.transform;
                bone.SetParent(_previousBoxBone == null ? _boxPivot : _previousBoxBone);
                bone.localEulerAngles = Vector3.zero;
                bone.localPosition = i == 0 ? Vector3.zero : Vector3.up * BoxSpawnOffset;
                _previousBoxBone = bone;
                _boxBones[i] = bone;
            }

            InitializeJiggleRig();
        }

        private void InitializeJiggleRig()
        {
            var startJiggleIndex = Mathf.Max((int)(_boxBones.Length * _ignoreBoxesJigglePercent) - 1, 0);
            _jiggleRigBuilder.jiggleRigs[0] = new JiggleRigBuilder.JiggleRig(_boxBones[startJiggleIndex],
                _jiggleSettings, new List<Transform>(), new List<Collider>());

            _jiggleRigBuilder.Initialize();
        }

        private void ClearBoxes()
        {
            foreach (var boxBehaviour in _boxBehaviours)
            {
                Destroy(boxBehaviour.gameObject);
            }

            _boxBehaviours.Clear();
        }

        private void UpdateBoxes(IEnumerable<Item> items)
        {
            var existingBoxes = CountExistingBoxes();

            foreach (var item in items)
            {
                var defaultBoxesCount = item.Quantity == 0 ? 0 : DefaultBoxesCount;
                var requiredBoxCount = Mathf.Max(defaultBoxesCount, item.Quantity / _itemsPerBox);

                if (existingBoxes.TryGetValue(item.Type, out var existingCount))
                {
                    UpdateExistingBoxes(existingCount, requiredBoxCount, item.Type);
                }
                else
                {
                    AddBoxes(requiredBoxCount, item.Type);
                }

                existingBoxes[item.Type] = requiredBoxCount;
            }
        }

        private Dictionary<ItemType, int> CountExistingBoxes()
        {
            var existingBoxes = new Dictionary<ItemType, int>();

            foreach (var box in _boxBehaviours)
            {
                if (!existingBoxes.ContainsKey(box.Type))
                {
                    existingBoxes[box.Type] = 0;
                }

                existingBoxes[box.Type]++;
            }

            return existingBoxes;
        }

        private void UpdateExistingBoxes(int existingCount, int requiredBoxCount,
            ItemType type)
        {
            if (requiredBoxCount < existingCount)
            {
                RemoveExcessBoxes(existingCount - requiredBoxCount, type);
            }
            else if (requiredBoxCount > existingCount)
            {
                AddBoxes(requiredBoxCount - existingCount, type);
            }
        }

        private void RemoveExcessBoxes(int countToRemove, ItemType type)
        {
            for (var i = 0; i < countToRemove; i++)
            {
                var boxToRemove = _boxBehaviours.FindLast(b => b.Type == type);
                _boxBehaviours.Remove(boxToRemove);
                AnimateBoxRemoval(boxToRemove);
            }
        }

        private async void AnimateBoxRemoval(BoxItemsView boxToRemove)
        {
            await boxToRemove.HideBox();
            Destroy(boxToRemove.gameObject);
            RearrangeBoxes();
        }

        private void RearrangeBoxes()
        {
            for (var i = 0; i < _boxBehaviours.Count; i++)
            {
                var box = _boxBehaviours[i];
                box.transform.SetParent(_boxBones[i]);
                box.transform.DOLocalMove(_startSpawnOffset, _boxDisappearMoveOthersDuration);
            }
        }

        private void AddBoxes(int countToAdd, ItemType type)
        {
            for (var i = 0; i < countToAdd; i++)
            {
                InstantiateBox(type, i == countToAdd - 1);
            }
        }

        private void InstantiateBox(ItemType type, bool showBox)
        {
            if (_boxPrefab == null || _boxBehaviours.Count >= _maxBoxesCount)
            {
                return;
            }

            var boxInstance = Instantiate(_boxPrefab, _boxBones[_boxBehaviours.Count]);
            boxInstance.transform.localPosition = _startSpawnOffset;

            var startJiggleIndex = (int)(_boxBehaviours.Count * _ignoreBoxesJigglePercent);
            var newParent = _boxBehaviours.Count < startJiggleIndex
                ? transform
                : boxInstance.transform.parent;

            boxInstance.transform.parent = newParent;

            boxInstance.HideBoxImmediately();
            _boxBehaviours.Add(boxInstance);

            if (showBox)
            {
                boxInstance.ShowBox();
            }
            else
            {
                boxInstance.ShowBoxImmediately();
            }
        }
    }
}