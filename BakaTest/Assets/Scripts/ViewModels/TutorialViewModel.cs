#nullable enable
using System;
using BakaTest.Core.Commands;
using BakaTest.Core.MVVM;
using BakaTest.Data.Tutorial;
using BakaTest.Services.Tutorial;
using BakaTest.Services.Localization;
using UnityEngine;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// チュートリアル画面のViewModel
    /// </summary>
    /// <remarks>
    /// ITutorialServiceを使用してチュートリアルの進行を管理します。
    /// </remarks>
    public class TutorialViewModel : ViewModelBase
    {
        private readonly ITutorialService _tutorialService;
        private readonly ILocalizationService _localizationService;

        // Current Step Data
        private string _stepTitle = string.Empty;
        private string _stepDescription = string.Empty;
        private string _progressText = string.Empty;
        private bool _canSkip = true;
        private bool _showNextButton = true;
        private bool _showSkipButton = true;

        /// <summary>現在のステップのタイトル</summary>
        public string StepTitle
        {
            get => _stepTitle;
            set => SetProperty(ref _stepTitle, value);
        }

        /// <summary>現在のステップの説明</summary>
        public string StepDescription
        {
            get => _stepDescription;
            set => SetProperty(ref _stepDescription, value);
        }

        /// <summary>進行状況テキスト (例: "ステップ 1/10")</summary>
        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }

        /// <summary>スキップ可能かどうか</summary>
        public bool CanSkip
        {
            get => _canSkip;
            set => SetProperty(ref _canSkip, value);
        }

        /// <summary>次へボタンを表示するか</summary>
        public bool ShowNextButton
        {
            get => _showNextButton;
            set => SetProperty(ref _showNextButton, value);
        }

        /// <summary>スキップボタンを表示するか</summary>
        public bool ShowSkipButton
        {
            get => _showSkipButton;
            set => SetProperty(ref _showSkipButton, value);
        }

        // Commands
        public RelayCommand<object?> NextCommand { get; }
        public RelayCommand<object?> SkipCommand { get; }
        public RelayCommand<object?> BackCommand { get; }

        /// <summary>チュートリアルが完了した時に発火</summary>
        public event EventHandler? TutorialCompleted;

        /// <summary>チュートリアルがスキップされた時に発火</summary>
        public event EventHandler? TutorialSkipped;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TutorialViewModel(ITutorialService tutorialService, ILocalizationService localizationService)
        {
            _tutorialService = tutorialService ?? throw new ArgumentNullException(nameof(tutorialService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

            // Commands初期化
            NextCommand = new RelayCommand<object?>(ExecuteNext, CanExecuteNext);
            SkipCommand = new RelayCommand<object?>(ExecuteSkip, CanExecuteSkip);
            BackCommand = new RelayCommand<object?>(ExecuteBack);

            // サービスイベントを購読
            _tutorialService.StepChanged += OnStepChanged;
            _tutorialService.TutorialCompleted += OnTutorialCompleted;
            _tutorialService.TutorialSkipped += OnTutorialServiceSkipped;

            // 現在のステップを表示
            UpdateCurrentStep();

            Debug.Log("[TutorialViewModel] Initialized");
        }

        /// <summary>
        /// 現在のステップ情報を更新します
        /// </summary>
        private void UpdateCurrentStep()
        {
            var currentStep = _tutorialService.CurrentStep;

            if (currentStep == null)
            {
                // チュートリアルが進行中でない場合
                StepTitle = _localizationService.GetText("tutorial.welcome.title", Data.Localization.LocalizationCategory.Tutorial);
                StepDescription = _localizationService.GetText("tutorial.welcome.description", Data.Localization.LocalizationCategory.Tutorial);
                ProgressText = string.Empty;
                CanSkip = true;
                ShowNextButton = true;
                ShowSkipButton = true;

                Debug.Log("[TutorialViewModel] No current step - showing welcome screen");
                return;
            }

            // ローカライズされたタイトルと説明を取得
            StepTitle = _localizationService.GetText(currentStep.titleKey, Data.Localization.LocalizationCategory.Tutorial);
            StepDescription = _localizationService.GetText(currentStep.descriptionKey, Data.Localization.LocalizationCategory.Tutorial);

            // 進行状況を計算
            var progress = _tutorialService.Progress;
            int currentIndex = progress.completedSteps.Count + 1;
            int totalSteps = 10; // TODO: TutorialDataから取得
            ProgressText = $"ステップ {currentIndex}/{totalSteps}";

            CanSkip = currentStep.canSkip;
            ShowNextButton = true;
            ShowSkipButton = currentStep.canSkip;

            Debug.Log($"[TutorialViewModel] Updated to step: {currentStep.stepId}");
        }

        /// <summary>
        /// ステップ変更イベントハンドラ
        /// </summary>
        private void OnStepChanged(TutorialStep step)
        {
            UpdateCurrentStep();
            NextCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// チュートリアル完了イベントハンドラ
        /// </summary>
        private void OnTutorialCompleted()
        {
            Debug.Log("[TutorialViewModel] Tutorial completed!");
            TutorialCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// チュートリアルスキップイベントハンドラ
        /// </summary>
        private void OnTutorialServiceSkipped()
        {
            Debug.Log("[TutorialViewModel] Tutorial skipped!");
            TutorialSkipped?.Invoke(this, EventArgs.Empty);
        }

        // ===== Command Handlers =====

        private void ExecuteNext(object? parameter)
        {
            Debug.Log("[TutorialViewModel] Next button clicked");

            if (!_tutorialService.IsInTutorial)
            {
                // チュートリアルを開始
                _tutorialService.StartTutorial();
            }
            else
            {
                // 次のステップに進む
                _tutorialService.NextStep();
            }
        }

        private bool CanExecuteNext(object? parameter)
        {
            return true; // 常に次へ進めるようにする
        }

        private void ExecuteSkip(object? parameter)
        {
            Debug.Log("[TutorialViewModel] Skip button clicked");
            _tutorialService.SkipTutorial();
        }

        private bool CanExecuteSkip(object? parameter)
        {
            return CanSkip && _tutorialService.IsInTutorial;
        }

        private void ExecuteBack(object? parameter)
        {
            Debug.Log("[TutorialViewModel] Back button clicked (not implemented)");
            // TODO: 前のステップに戻る機能が必要な場合は実装
        }

        /// <summary>
        /// 特定のステップに移動します
        /// </summary>
        /// <param name="stepId">ステップID</param>
        public void GoToStep(string stepId)
        {
            _tutorialService.GoToStep(stepId);
        }

        /// <summary>
        /// チュートリアルを再開始します
        /// </summary>
        public void RestartTutorial()
        {
            Debug.Log("[TutorialViewModel] Restarting tutorial");
            _tutorialService.ResetTutorial();
            _tutorialService.StartTutorial();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tutorialService.StepChanged -= OnStepChanged;
                _tutorialService.TutorialCompleted -= OnTutorialCompleted;
                _tutorialService.TutorialSkipped -= OnTutorialServiceSkipped;
            }

            base.Dispose(disposing);
        }
    }
}
