using System;
using DG.Tweening;
using JetBrains.Annotations;
using Demo.Models;
using Demo.Services;
using Moon.Core;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Neptune.Core
{
    internal class TaskContext
    {
        [CanBeNull] public ItemType? ItemType { get; set; }

        public bool Empty() => ItemType is null;
    }

    public class TaskView : MonoBehaviour
    {
        private static readonly TimeSpan LoopDelay = TimeSpan.FromSeconds(0.5d);
        private const float AnimateDuration = 0.25f;

        [CanBeNull] [SerializeField] private Collider _triggerCollider;

        [Space] [SerializeField] private TaskProgressView _progressView;

        [SerializeField] private Sprite _iconSprite;

        [SerializeField] private TaskTimerView _timerView;

        [Space] [SerializeField] private Button _skipButton;

        [SerializeField] private Image _adsIcon;

        [SerializeField] private Image _ticketIcon;

        [SerializeField] private TMP_Text _skipText;

        [Space] [SerializeField] private Button _completeButton;

        [SerializeField] private TMP_Text _completeButtonText;

        [SerializeField] private VirtualClockService _timeService;


        private CompositeDisposable _worldDisposables;
        private TaskContext _taskType;
        private bool _hasOfferLock;

        private readonly ISubject<ItemType?> _onSkipSubject = new Subject<ItemType?>();
        private readonly ISubject<ItemType?> _onCompleteSubject = new Subject<ItemType?>();
        private readonly ISubject<Unit> _onHideSubject = new Subject<Unit>();

        private readonly IReactiveProperty<bool> _hasCharacterProperty = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> _hasTicketsProperty = new ReactiveProperty<bool>(false);

        private readonly CompositeDisposable _renderDisposables = new CompositeDisposable();

        public IObservable<ItemType?> SkipListener => _onSkipSubject;
        public IObservable<ItemType?> CompleteListener => _onCompleteSubject;
        public IObservable<Unit> HideListener => _onHideSubject;

        [UsedImplicitly]
        private void OnDestroy()
        {
            _renderDisposables?.Clear();
        }

        public void OnEnable()
        {
            if (null != _triggerCollider)
            {
                _triggerCollider.OnTriggerEnterAsObservable()
                    .Subscribe(OnTriggerEnterImpl)
                    .AddTo(this);

                _triggerCollider.OnTriggerExitAsObservable()
                    .Subscribe(OnTriggerExitImpl)
                    .AddTo(this);
            }

            _skipButton
                .OnClickAsObservable()
                .Subscribe(OnSkipClick)
                .AddTo(this);

            _completeButton
                .OnClickAsObservable()
                .Subscribe(_ => _onCompleteSubject.OnNext(_taskType.ItemType))
                .AddTo(this);

            Hide();
        }

        public void Render(ItemType itemType, TimeSpan duration, DateTime endsAt)
        {
            Render(new TaskContext { ItemType = itemType }, duration, endsAt);
        }

        private void Render(TaskContext taskType, TimeSpan duration, DateTime endsAt)
        {
            _taskType = taskType;
            _hasOfferLock = false;
            _renderDisposables?.Clear();

            if (_taskType.Empty())
            {
                _progressView.gameObject.SetActive(false);
                _timerView.gameObject.SetActive(false);
                return;
            }

            var amount = 1;

            _progressView.Render(_iconSprite);
            _timerView.Render(_iconSprite, amount);

            _hasCharacterProperty
                .Subscribe(OnViewsChanged)
                .AddTo(_renderDisposables);

            Observable.Interval(LoopDelay)
                .Subscribe(_ => OnIntervalLoop(duration, endsAt))
                .AddTo(_renderDisposables);

            OnIntervalLoop(duration, endsAt);
        }

        public void Hide()
        {
            _progressView.gameObject.SetActive(false);
            _timerView.gameObject.SetActive(false);

            _renderDisposables?.Clear();
            _onHideSubject.OnNext(Unit.Default);
        }

        private void OnHasTicketsChanged(bool hasTickets)
        {
            if (hasTickets)
            {
                _ticketIcon.gameObject.SetActive(true);
                _adsIcon.gameObject.SetActive(false);
                _skipText.gameObject.SetActive(false);
            }
            else
            {
                _ticketIcon.gameObject.SetActive(false);
                _adsIcon.gameObject.SetActive(true);
                _skipText.gameObject.SetActive(true);
            }
        }

        private void OnViewsChanged(bool hasCharacter)
        {
            if (hasCharacter)
            {
                _worldDisposables?.Dispose();
                _worldDisposables = new CompositeDisposable();

                _hasTicketsProperty
                    .Subscribe(OnHasTicketsChanged)
                    .AddTo(_worldDisposables);

                var wasActive = _timerView.gameObject.activeSelf;

                _progressView.gameObject.SetActive(false);
                _timerView.gameObject.SetActive(true);

                if (!wasActive)
                {
                    _timerView.transform
                        .DOScale(1.0f, AnimateDuration)
                        .From()
                        .SetEase(Ease.OutBack);
                }
            }
            else
            {
                var wasActive = _progressView.gameObject.activeSelf;

                _progressView.gameObject.SetActive(true);
                _timerView.gameObject.SetActive(false);

                if (!wasActive)
                {
                    _progressView.transform
                        .DOScale(1.0f, AnimateDuration)
                        .From()
                        .SetEase(Ease.OutBack);

                    _worldDisposables?.Dispose();
                    _worldDisposables = null;
                }
            }
        }

        private void OnIntervalLoop(TimeSpan duration, DateTime endsAt)
        {
            if (_timeService.Now > endsAt)
            {
                _renderDisposables?.Clear();

                _progressView.SetFull();
                _timerView.SetFull();

                _skipButton.gameObject.SetActive(false);
                _progressView.gameObject.SetActive(false);

                if (_taskType.ItemType != null)
                {
                    _completeButton.gameObject.SetActive(true);
                    _timerView.gameObject.SetActive(true);
                }
                else
                {
                    _completeButton.gameObject.SetActive(false);
                    _timerView.gameObject.SetActive(false);
                }

                return;
            }

            var timeLeft = _timeService.TimeRemaining(endsAt);

            _progressView.SetValue(timeLeft, duration, LoopDelay);
            _timerView.SetValue(timeLeft, duration, LoopDelay);

            _completeButton.gameObject.SetActive(false);

            _skipButton.gameObject.SetActive(false);
        }

        private void OnTriggerEnterImpl(Collider other)
        {
            if (other.CompareTag("Character"))
            {
                _hasCharacterProperty.Value = true;
            }
        }

        private void OnTriggerExitImpl(Collider other)
        {
            if (other.CompareTag("Character"))
            {
                _hasCharacterProperty.Value = false;
            }
        }

        private void OnSkipClick(Unit _)
        {
            if (_hasTicketsProperty.Value)
            {
                _onSkipSubject.OnNext(_taskType.ItemType);

                _progressView.SetFull();
                _timerView.SetFull();
            }
        }
    }
}