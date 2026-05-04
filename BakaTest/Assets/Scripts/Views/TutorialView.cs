#nullable enable
using System;
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.ViewModels;
using BakaTest.Services.Tutorial;
using BakaTest.Services.Localization;
using BakaTest.Services.SceneManagement;

namespace BakaTest.Views
{
    /// <summary>
    /// チュートリアル画面のView
    /// </summary>
    /// <remarks>
    /// チュートリアルのステップ情報を表示し、ユーザーの進行操作を受け付けます。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class TutorialView : ViewBase<TutorialViewModel>
    {
        // UI Elements
        private Label? _stepTitleLabel;
        private Label? _stepDescriptionLabel;
        private Label? _progressLabel;
        private Button? _nextButton;
        private Button? _skipButton;
        private VisualElement? _highlightOverlay;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービスを取得
            var tutorialService = ServiceLocator.Instance.Get<ITutorialService>();
            var localizationService = ServiceLocator.Instance.Get<ILocalizationService>();

            if (tutorialService == null || localizationService == null)
            {
                Debug.LogError("[TutorialView] Required services not found in ServiceLocator.");
                return;
            }

            // ViewModelを作成
            var viewModel = new TutorialViewModel(tutorialService, localizationService);
            SetViewModel(viewModel);
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            GetUIElements();
            RegisterEventHandlers();
            UpdateUIFromViewModel();
        }

        protected override void BindViewModel(TutorialViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
                viewModel.TutorialCompleted += OnTutorialCompleted;
                viewModel.TutorialSkipped += OnTutorialSkipped;
            }
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.TutorialCompleted -= OnTutorialCompleted;
                ViewModel.TutorialSkipped -= OnTutorialSkipped;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();
        }

        /// <summary>
        /// UI要素を取得します
        /// </summary>
        private void GetUIElements()
        {
            if (Root == null) return;

            _stepTitleLabel = Root.Q<Label>("StepTitleLabel");
            _stepDescriptionLabel = Root.Q<Label>("StepDescriptionLabel");
            _progressLabel = Root.Q<Label>("ProgressLabel");
            _nextButton = Root.Q<Button>("NextButton");
            _skipButton = Root.Q<Button>("SkipButton");
            _highlightOverlay = Root.Q<VisualElement>("HighlightOverlay");

            // 初期状態でハイライトを非表示
            if (_highlightOverlay != null)
            {
                _highlightOverlay.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            if (_nextButton != null)
                _nextButton.clicked += OnNextClicked;

            if (_skipButton != null)
                _skipButton.clicked += OnSkipClicked;
        }

        /// <summary>
        /// イベントハンドラを解除します
        /// </summary>
        private void UnregisterEventHandlers()
        {
            if (_nextButton != null)
                _nextButton.clicked -= OnNextClicked;

            if (_skipButton != null)
                _skipButton.clicked -= OnSkipClicked;
        }

        /// <summary>
        /// ViewModelからUIを更新します
        /// </summary>
        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            if (_stepTitleLabel != null)
                _stepTitleLabel.text = ViewModel.StepTitle;

            if (_stepDescriptionLabel != null)
                _stepDescriptionLabel.text = ViewModel.StepDescription;

            if (_progressLabel != null)
                _progressLabel.text = ViewModel.ProgressText;

            if (_nextButton != null)
                _nextButton.style.display = ViewModel.ShowNextButton ? DisplayStyle.Flex : DisplayStyle.None;

            if (_skipButton != null)
            {
                _skipButton.style.display = ViewModel.ShowSkipButton ? DisplayStyle.Flex : DisplayStyle.None;
                _skipButton.SetEnabled(ViewModel.CanSkip);
            }
        }

        // ===== Event Handlers =====

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(ViewModel.StepTitle):
                    if (_stepTitleLabel != null)
                        _stepTitleLabel.text = ViewModel.StepTitle;
                    break;

                case nameof(ViewModel.StepDescription):
                    if (_stepDescriptionLabel != null)
                        _stepDescriptionLabel.text = ViewModel.StepDescription;
                    break;

                case nameof(ViewModel.ProgressText):
                    if (_progressLabel != null)
                        _progressLabel.text = ViewModel.ProgressText;
                    break;

                case nameof(ViewModel.ShowNextButton):
                    if (_nextButton != null)
                        _nextButton.style.display = ViewModel.ShowNextButton ? DisplayStyle.Flex : DisplayStyle.None;
                    break;

                case nameof(ViewModel.ShowSkipButton):
                    if (_skipButton != null)
                        _skipButton.style.display = ViewModel.ShowSkipButton ? DisplayStyle.Flex : DisplayStyle.None;
                    break;

                case nameof(ViewModel.CanSkip):
                    if (_skipButton != null)
                        _skipButton.SetEnabled(ViewModel.CanSkip);
                    break;
            }
        }

        private void OnTutorialCompleted(object? sender, EventArgs e)
        {
            Debug.Log("[TutorialView] Tutorial completed - returning to main menu");

            // メインメニューに戻る
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService != null)
            {
                sceneService.LoadMainMenu();
            }
        }

        private void OnTutorialSkipped(object? sender, EventArgs e)
        {
            Debug.Log("[TutorialView] Tutorial skipped - returning to main menu");

            // メインメニューに戻る
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService != null)
            {
                sceneService.LoadMainMenu();
            }
        }

        private void OnNextClicked()
        {
            ViewModel?.NextCommand.Execute(null);
        }

        private void OnSkipClicked()
        {
            ViewModel?.SkipCommand.Execute(null);
        }

        /// <summary>
        /// UI要素をハイライトします（チュートリアル用）
        /// </summary>
        /// <param name="elementName">ハイライトする要素の名前</param>
        public void HighlightElement(string elementName)
        {
            if (Root == null || _highlightOverlay == null) return;

            var targetElement = Root.Q<VisualElement>(elementName);
            if (targetElement == null)
            {
                Debug.LogWarning($"[TutorialView] Element not found for highlight: {elementName}");
                return;
            }

            // ハイライトオーバーレイを表示
            _highlightOverlay.style.display = DisplayStyle.Flex;

            // TODO: ハイライト位置を対象要素に合わせる
            // Unity UI ToolkitではWorldBoundを使用して要素の位置を取得できますが、
            // これは実装が複雑になるため、将来的に実装することを推奨します。

            Debug.Log($"[TutorialView] Highlighting element: {elementName}");
        }

        /// <summary>
        /// ハイライトを解除します
        /// </summary>
        public void ClearHighlight()
        {
            if (_highlightOverlay != null)
            {
                _highlightOverlay.style.display = DisplayStyle.None;
            }
        }
    }
}
