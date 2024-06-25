using System;
using System.Threading.Tasks;
using Extensions;
using Models.Bonuses;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Demo.Core
{
    public class CoreUiBehaviour : MonoBehaviour
    {
        [SerializeField] private CharacterPhysicsBehaviour _characterPhysicsBehaviour;
        [SerializeField] private CharacterTasksBehaviour _characterTasksBehaviour;
        [SerializeField] private GameObject _joystickCanvas;
        [SerializeField] private GameObject _tutorPanel;
        [SerializeField] private GameObject _gameEndedPanel;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _exitGameButton;
        [SerializeField] private TMP_Text _cheeseAmountText;
        [SerializeField] private float _hideDuration = 0.25f;
        [SerializeField] private float _appearDuration = 0.5f;
        [SerializeField] private float _tutorAppearDelay = 0.25f;
        [SerializeField] private float _gameEndedAppearDelay = 2f;

        private async void Start()
        {
            await Task.Delay(TimeSpan.FromSeconds(_tutorAppearDelay));

            await _tutorPanel.SetActiveWithBounceAsync(true, _appearDuration);

            _characterPhysicsBehaviour.BonusReceivedListener
                .Subscribe(AddNewBonus)
                .AddTo(this);

            _characterTasksBehaviour.GameEndedListener
                .Subscribe(OnGameEnded)
                .AddTo(this);

            _startGameButton.OnClickAsObservable()
                .Subscribe(OnGameStarted)
                .AddTo(this);

            _exitGameButton.OnClickAsObservable()
                .Subscribe(_ => Application.Quit())
                .AddTo(this);
        }

        private async void OnGameStarted(Unit unit)
        {
            await _tutorPanel.gameObject.SetActiveWithBounceAsync(false, _hideDuration);

            _joystickCanvas.SetActive(true);
        }

        private async void OnGameEnded(Unit unit)
        {
            await Task.Delay(TimeSpan.FromSeconds(_gameEndedAppearDelay));

            await _gameEndedPanel.SetActiveWithBounceAsync(true, _appearDuration);
        }

        private async void AddNewBonus(Bonus bonus) //TODO: handle different bonuses
        {
            _cheeseAmountText.text = $"{bonus.Amount}";

            await _cheeseAmountText.gameObject.SetActiveWithBounceAsync(true, 0.25f);
        }
    }
}