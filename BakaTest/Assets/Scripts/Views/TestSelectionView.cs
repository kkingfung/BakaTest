#nullable enable
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.ViewModels;
using BakaTest.Services.Tests;
using BakaTest.Services.Player;
using BakaTest.Services.SceneManagement;

namespace BakaTest.Views
{
    /// <summary>
    /// テスト選択画面のView
    /// </summary>
    /// <remarks>
    /// TestSelectionViewModelとUI Toolkitを接続します。
    /// ボタンベースのUI（教科と難易度選択）を実装します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class TestSelectionView : ViewBase<TestSelectionViewModel>
    {
        // 教科選択ボタン
        private Button? _mathButton;
        private Button? _scienceButton;
        private Button? _englishButton;
        private Button? _historyButton;

        // 難易度選択ボタン
        private Button? _elementaryButton;
        private Button? _middleSchoolButton;
        private Button? _highSchoolButton;
        private Button? _universityButton;

        // アクションボタン
        private Button? _startTestButton;
        private Button? _backButton;

        // 情報パネルラベル
        private Label? _questionsCountLabel;
        private Label? _timeLimitLabel;
        private Label? _pointsPerQuestionLabel;
        private Label? _dailyProgressLabel;
        private Label? _totalPointsLabel;

        // 警告パネル
        private VisualElement? _dailyLimitWarning;

        protected override void Awake()
        {
            base.Awake();

            // サービス取得
            var testService = ServiceLocator.Instance.Get<ITestService>();
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();

            if (testService == null || playerDataService == null)
            {
                Debug.LogError("[TestSelectionView] Required services not found!");
                return;
            }

            // ViewModelを作成して設定
            SetViewModel(new TestSelectionViewModel(testService, playerDataService));
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            // UI要素を取得
            GetUIElements();

            // ViewModelの初期値でUIを更新
            UpdateUIFromViewModel();

            // イベントハンドラを登録
            RegisterEventHandlers();
        }

        protected override void BindViewModel(TestSelectionViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
                viewModel.TestStartRequested += OnTestStartRequested;
                viewModel.BackToMenuRequested += OnBackToMenuRequested;
            }
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.TestStartRequested -= OnTestStartRequested;
                ViewModel.BackToMenuRequested -= OnBackToMenuRequested;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();
        }

        protected void OnEnable()
        {
            // 画面が有効になったらデータを更新
            ViewModel?.RefreshData();
        }

        private void GetUIElements()
        {
            if (Root == null) return;

            // 教科選択ボタン
            _mathButton = Q<Button>("MathButton");
            _scienceButton = Q<Button>("ScienceButton");
            _englishButton = Q<Button>("EnglishButton");
            _historyButton = Q<Button>("HistoryButton");

            // 難易度選択ボタン
            _elementaryButton = Q<Button>("ElementaryButton");
            _middleSchoolButton = Q<Button>("MiddleSchoolButton");
            _highSchoolButton = Q<Button>("HighSchoolButton");
            _universityButton = Q<Button>("UniversityButton");

            // アクションボタン
            _startTestButton = Q<Button>("StartTestButton");
            _backButton = Q<Button>("BackButton");

            // 情報パネルラベル
            _questionsCountLabel = Q<Label>("QuestionsCountLabel");
            _timeLimitLabel = Q<Label>("TimeLimitLabel");
            _pointsPerQuestionLabel = Q<Label>("PointsPerQuestionLabel");
            _dailyProgressLabel = Q<Label>("DailyProgressLabel");
            _totalPointsLabel = Q<Label>("TotalPointsLabel");

            // 警告パネル
            _dailyLimitWarning = Q<VisualElement>("DailyLimitWarning");
        }

        private void RegisterEventHandlers()
        {
            // 教科選択ボタン
            if (_mathButton != null)
                _mathButton.clicked += () => ViewModel?.SelectMathCommand.Execute(null);
            if (_scienceButton != null)
                _scienceButton.clicked += () => ViewModel?.SelectScienceCommand.Execute(null);
            if (_englishButton != null)
                _englishButton.clicked += () => ViewModel?.SelectEnglishCommand.Execute(null);
            if (_historyButton != null)
                _historyButton.clicked += () => ViewModel?.SelectHistoryCommand.Execute(null);

            // 難易度選択ボタン
            if (_elementaryButton != null)
                _elementaryButton.clicked += () => ViewModel?.SelectElementaryCommand.Execute(null);
            if (_middleSchoolButton != null)
                _middleSchoolButton.clicked += () => ViewModel?.SelectMiddleSchoolCommand.Execute(null);
            if (_highSchoolButton != null)
                _highSchoolButton.clicked += () => ViewModel?.SelectHighSchoolCommand.Execute(null);
            if (_universityButton != null)
                _universityButton.clicked += () => ViewModel?.SelectUniversityCommand.Execute(null);

            // アクションボタン
            if (_startTestButton != null)
                _startTestButton.clicked += () => ViewModel?.StartTestCommand.Execute(null);
            if (_backButton != null)
                _backButton.clicked += () => ViewModel?.BackToMenuCommand.Execute(null);
        }

        private void UnregisterEventHandlers()
        {
            // 教科選択ボタン
            if (_mathButton != null)
                _mathButton.clicked -= () => ViewModel?.SelectMathCommand.Execute(null);
            if (_scienceButton != null)
                _scienceButton.clicked -= () => ViewModel?.SelectScienceCommand.Execute(null);
            if (_englishButton != null)
                _englishButton.clicked -= () => ViewModel?.SelectEnglishCommand.Execute(null);
            if (_historyButton != null)
                _historyButton.clicked -= () => ViewModel?.SelectHistoryCommand.Execute(null);

            // 難易度選択ボタン
            if (_elementaryButton != null)
                _elementaryButton.clicked -= () => ViewModel?.SelectElementaryCommand.Execute(null);
            if (_middleSchoolButton != null)
                _middleSchoolButton.clicked -= () => ViewModel?.SelectMiddleSchoolCommand.Execute(null);
            if (_highSchoolButton != null)
                _highSchoolButton.clicked -= () => ViewModel?.SelectHighSchoolCommand.Execute(null);
            if (_universityButton != null)
                _universityButton.clicked -= () => ViewModel?.SelectUniversityCommand.Execute(null);

            // アクションボタン
            if (_startTestButton != null)
                _startTestButton.clicked -= () => ViewModel?.StartTestCommand.Execute(null);
            if (_backButton != null)
                _backButton.clicked -= () => ViewModel?.BackToMenuCommand.Execute(null);
        }

        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            UpdateSubjectButtonStates();
            UpdateDifficultyButtonStates();
            UpdateInfoPanel();
            UpdateWarningPanel();
            UpdateStartButtonState();
        }

        private void UpdateSubjectButtonStates()
        {
            if (ViewModel == null) return;

            // すべてのボタンからselectedクラスを削除
            _mathButton?.RemoveFromClassList("selected");
            _scienceButton?.RemoveFromClassList("selected");
            _englishButton?.RemoveFromClassList("selected");
            _historyButton?.RemoveFromClassList("selected");

            // 選択されているボタンにselectedクラスを追加
            if (ViewModel.IsMathSelected) _mathButton?.AddToClassList("selected");
            if (ViewModel.IsScienceSelected) _scienceButton?.AddToClassList("selected");
            if (ViewModel.IsEnglishSelected) _englishButton?.AddToClassList("selected");
            if (ViewModel.IsHistorySelected) _historyButton?.AddToClassList("selected");
        }

        private void UpdateDifficultyButtonStates()
        {
            if (ViewModel == null) return;

            // すべてのボタンからselectedクラスを削除
            _elementaryButton?.RemoveFromClassList("selected");
            _middleSchoolButton?.RemoveFromClassList("selected");
            _highSchoolButton?.RemoveFromClassList("selected");
            _universityButton?.RemoveFromClassList("selected");

            // 選択されているボタンにselectedクラスを追加
            if (ViewModel.IsElementarySelected) _elementaryButton?.AddToClassList("selected");
            if (ViewModel.IsMiddleSchoolSelected) _middleSchoolButton?.AddToClassList("selected");
            if (ViewModel.IsHighSchoolSelected) _highSchoolButton?.AddToClassList("selected");
            if (ViewModel.IsUniversitySelected) _universityButton?.AddToClassList("selected");
        }

        private void UpdateInfoPanel()
        {
            if (ViewModel == null) return;

            if (_questionsCountLabel != null)
                _questionsCountLabel.text = ViewModel.QuestionsCount.ToString();

            if (_timeLimitLabel != null)
                _timeLimitLabel.text = ViewModel.TimeLimit;

            if (_pointsPerQuestionLabel != null)
                _pointsPerQuestionLabel.text = ViewModel.PointsPerQuestion.ToString();

            if (_dailyProgressLabel != null)
                _dailyProgressLabel.text = ViewModel.DailyProgressText;

            if (_totalPointsLabel != null)
                _totalPointsLabel.text = ViewModel.TotalPotentialPoints.ToString();
        }

        private void UpdateWarningPanel()
        {
            if (ViewModel == null || _dailyLimitWarning == null) return;

            // デイリー上限に達している場合は警告パネルを表示
            _dailyLimitWarning.style.display = ViewModel.IsDailyLimitReached 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }

        private void UpdateStartButtonState()
        {
            if (ViewModel == null || _startTestButton == null) return;

            bool canStart = ViewModel.StartTestCommand.CanExecute(null);
            _startTestButton.SetEnabled(canStart);
        }

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // プロパティ変更時にUIを更新
            UpdateUIFromViewModel();
        }

        private void OnTestStartRequested(object? sender, System.EventArgs e)
        {
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService == null)
            {
                Debug.LogError("[TestSelectionView] SceneManagementService not found!");
                return;
            }

            Debug.Log("[TestSelectionView] Loading Test Taking scene...");
            sceneService.LoadTestTaking();
        }

        private void OnBackToMenuRequested(object? sender, System.EventArgs e)
        {
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService == null)
            {
                Debug.LogError("[TestSelectionView] SceneManagementService not found!");
                return;
            }

            Debug.Log("[TestSelectionView] Loading Main Menu scene...");
            sceneService.LoadMainMenu();
        }
    }
}
