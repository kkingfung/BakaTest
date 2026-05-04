#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using BakaTest.Data.Tutorial;
using BakaTest.Services.Save;
using BakaTest.Services.SceneManagement;
using BakaTest.Services.Localization;

namespace BakaTest.Services.Tutorial
{
    /// <summary>
    /// チュートリアルサービス実装
    /// </summary>
    public class TutorialService : ITutorialService
    {
        private readonly TutorialData _tutorialData;
        private readonly ISaveService _saveService;
        private readonly ISceneManagementService _sceneService;
        private readonly ILocalizationService _localizationService;

        private TutorialProgress _progress;
        private TutorialStep? _currentStep;

        // 機能アンロック定義（ステップID → 機能名のマッピング）
        private readonly Dictionary<string, List<string>> _featureUnlocks = new Dictionary<string, List<string>>
        {
            { "welcome", new List<string> { "main_menu" } },
            { "test_selection", new List<string> { "test_selection_screen" } },
            { "test_complete", new List<string> { "test_system" } },
            { "champion_intro", new List<string> { "champion_screen" } },
            { "champion_select", new List<string> { "champion_selection" } },
            { "point_allocation", new List<string> { "point_allocation_screen" } },
            { "battle_intro", new List<string> { "battle_screen" } },
            { "battle_complete", new List<string> { "battle_system" } },
            { "items_intro", new List<string> { "inventory_screen", "item_shop" } },
            { "settings_intro", new List<string> { "settings_screen" } }
        };

        /// <summary>チュートリアル中かどうか</summary>
        public bool IsInTutorial => _progress != null && !_progress.isCompleted && !_progress.wasSkipped;

        /// <summary>現在のチュートリアル進行状況</summary>
        public TutorialProgress Progress => _progress;

        /// <summary>現在のステップ</summary>
        public TutorialStep? CurrentStep => _currentStep;

        /// <summary>ステップが変更された時に発火</summary>
        public event Action<TutorialStep>? StepChanged;

        /// <summary>チュートリアルが完了した時に発火</summary>
        public event Action? TutorialCompleted;

        /// <summary>チュートリアルがスキップされた時に発火</summary>
        public event Action? TutorialSkipped;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TutorialService(
            TutorialData tutorialData,
            ISaveService saveService,
            ISceneManagementService sceneService,
            ILocalizationService localizationService)
        {
            _tutorialData = tutorialData ?? throw new ArgumentNullException(nameof(tutorialData));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            _sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

            // セーブデータから進行状況を読み込み
            _progress = _saveService.CurrentSaveData.tutorialProgress ?? new TutorialProgress();

            // 現在のステップを復元
            if (!string.IsNullOrEmpty(_progress.currentStepId))
            {
                _currentStep = _tutorialData.GetStepById(_progress.currentStepId);
            }

            Debug.Log($"[TutorialService] Initialized. IsInTutorial={IsInTutorial}, CurrentStep={_currentStep?.stepId ?? "null"}");
        }

        /// <summary>
        /// チュートリアルを開始します
        /// </summary>
        public void StartTutorial()
        {
            Debug.Log("[TutorialService] Starting tutorial...");

            // 新しい進行状況を作成
            _progress = new TutorialProgress
            {
                isCompleted = false,
                wasSkipped = false,
                currentStepId = string.Empty,
                completedSteps = new List<string>(),
                lastUpdated = DateTime.Now
            };

            // 最初のステップに移動
            var firstStep = _tutorialData.GetFirstStep();
            if (firstStep != null)
            {
                _currentStep = firstStep;
                _progress.currentStepId = firstStep.stepId;
                SaveProgress();
                StepChanged?.Invoke(firstStep);
            }
            else
            {
                Debug.LogError("[TutorialService] No tutorial steps found!");
            }
        }

        /// <summary>
        /// チュートリアルをスキップします
        /// </summary>
        public void SkipTutorial()
        {
            Debug.Log("[TutorialService] Skipping tutorial...");

            _progress.isCompleted = true;
            _progress.wasSkipped = true;
            _progress.currentStepId = string.Empty;
            _progress.lastUpdated = DateTime.Now;
            _currentStep = null;

            SaveProgress();
            TutorialSkipped?.Invoke();
        }

        /// <summary>
        /// 次のステップに進みます
        /// </summary>
        public void NextStep()
        {
            if (_currentStep == null)
            {
                Debug.LogWarning("[TutorialService] NextStep called but no current step!");
                return;
            }

            // 現在のステップを完了済みに追加
            if (!_progress.completedSteps.Contains(_currentStep.stepId))
            {
                _progress.completedSteps.Add(_currentStep.stepId);
            }

            // 次のステップを取得
            var nextStep = _tutorialData.GetNextStep(_currentStep.stepId);
            if (nextStep != null)
            {
                _currentStep = nextStep;
                _progress.currentStepId = nextStep.stepId;
                _progress.lastUpdated = DateTime.Now;

                SaveProgress();
                StepChanged?.Invoke(nextStep);

                Debug.Log($"[TutorialService] Moved to next step: {nextStep.stepId}");

                // シーン遷移が必要な場合は遷移
                if (nextStep.stepType == TutorialStepType.NavigateToScene && !string.IsNullOrEmpty(nextStep.targetScene))
                {
                    NavigateToScene(nextStep.targetScene);
                }
            }
            else
            {
                // チュートリアル完了
                CompleteTutorial();
            }
        }

        /// <summary>
        /// 特定のステップに移動します
        /// </summary>
        public void GoToStep(string stepId)
        {
            var step = _tutorialData.GetStepById(stepId);
            if (step == null)
            {
                Debug.LogWarning($"[TutorialService] Step not found: {stepId}");
                return;
            }

            _currentStep = step;
            _progress.currentStepId = stepId;
            _progress.lastUpdated = DateTime.Now;

            SaveProgress();
            StepChanged?.Invoke(step);

            Debug.Log($"[TutorialService] Jumped to step: {stepId}");

            // シーン遷移が必要な場合は遷移
            if (step.stepType == TutorialStepType.NavigateToScene && !string.IsNullOrEmpty(step.targetScene))
            {
                NavigateToScene(step.targetScene);
            }
        }

        /// <summary>
        /// 現在のステップを取得します
        /// </summary>
        public TutorialStep? GetCurrentStep()
        {
            return _currentStep;
        }

        /// <summary>
        /// 現在のステップを完了します
        /// </summary>
        public void CompleteCurrentStep()
        {
            if (_currentStep == null)
            {
                Debug.LogWarning("[TutorialService] CompleteCurrentStep called but no current step!");
                return;
            }

            Debug.Log($"[TutorialService] Completing step: {_currentStep.stepId}");
            NextStep();
        }

        /// <summary>
        /// 特定の機能がアンロックされているかチェックします
        /// </summary>
        public bool IsFeatureUnlocked(string featureName)
        {
            // チュートリアルスキップ済みまたは完了済みの場合は全機能アンロック
            if (_progress.wasSkipped || _progress.isCompleted)
            {
                return true;
            }

            // 完了済みステップから機能アンロック状況をチェック
            foreach (var completedStepId in _progress.completedSteps)
            {
                if (_featureUnlocks.TryGetValue(completedStepId, out var features))
                {
                    if (features.Contains(featureName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// チュートリアルの進行状況をリセットします
        /// </summary>
        public void ResetTutorial()
        {
            Debug.Log("[TutorialService] Resetting tutorial...");

            _progress = new TutorialProgress();
            _currentStep = null;
            SaveProgress();
        }

        /// <summary>
        /// チュートリアルを完了します
        /// </summary>
        private void CompleteTutorial()
        {
            Debug.Log("[TutorialService] Tutorial completed!");

            _progress.isCompleted = true;
            _progress.currentStepId = string.Empty;
            _progress.lastUpdated = DateTime.Now;
            _currentStep = null;

            SaveProgress();
            TutorialCompleted?.Invoke();
        }

        /// <summary>
        /// 進行状況を保存します
        /// </summary>
        private void SaveProgress()
        {
            _saveService.CurrentSaveData.tutorialProgress = _progress;
            _saveService.Save();
            Debug.Log($"[TutorialService] Progress saved. IsCompleted={_progress.isCompleted}, CurrentStep={_progress.currentStepId}");
        }

        /// <summary>
        /// シーンに遷移します
        /// </summary>
        private void NavigateToScene(string sceneName)
        {
            Debug.Log($"[TutorialService] Navigating to scene: {sceneName}");

            // シーン名をメソッド名にマッピング
            switch (sceneName.ToLower())
            {
                case "mainmenu":
                    _sceneService.LoadMainMenu();
                    break;
                case "testselection":
                    _sceneService.LoadTestSelection();
                    break;
                case "championselection":
                    _sceneService.LoadChampionSelection();
                    break;
                case "pointallocation":
                    _sceneService.LoadPointAllocation();
                    break;
                case "battle":
                    _sceneService.LoadBattle();
                    break;
                case "inventory":
                    _sceneService.LoadInventory();
                    break;
                case "championshop":
                    _sceneService.LoadChampionShop();
                    break;
                case "settings":
                    _sceneService.LoadSettings();
                    break;
                default:
                    Debug.LogWarning($"[TutorialService] Unknown scene name: {sceneName}");
                    break;
            }
        }
    }
}
