#nullable enable
using System;
using BakaTest.Data.Tutorial;
using BakaTest.Services.Tutorial;

namespace BakaTest.Tests.Mocks
{
    /// <summary>
    /// TutorialServiceのモック実装（テスト用）
    /// </summary>
    public class MockTutorialService : ITutorialService
    {
        private TutorialProgress _progress = new TutorialProgress();
        private TutorialStep? _currentStep;
        private int _currentStepIndex = -1;

        // Test helper events
        public event Action? TutorialStartedEvent;
        public event Action<TutorialStep>? StepChangedEvent;

        public bool IsInTutorial => !_progress.isCompleted && !_progress.wasSkipped;

        public TutorialProgress Progress => _progress;

        public TutorialStep? CurrentStep => _currentStep;

        public event Action<TutorialStep>? StepChanged;
        public event Action? TutorialCompleted;
        public event Action? TutorialSkipped;

        public void StartTutorial()
        {
            _progress = new TutorialProgress
            {
                isCompleted = false,
                wasSkipped = false,
                currentStepId = "step_1",
                completedSteps = new System.Collections.Generic.List<string>()
            };

            _currentStep = CreateTestStep("step_1", "Test Step 1");
            _currentStepIndex = 0;

            TutorialStartedEvent?.Invoke();
            StepChanged?.Invoke(_currentStep);
        }

        public void SkipTutorial()
        {
            _progress.isCompleted = true;
            _progress.wasSkipped = true;
            _currentStep = null;

            TutorialSkipped?.Invoke();
        }

        public void NextStep()
        {
            if (_currentStep == null) return;

            _currentStepIndex++;
            if (_currentStepIndex >= 5) // Simulate 5 steps total
            {
                CompleteTutorial();
                return;
            }

            _currentStep = CreateTestStep($"step_{_currentStepIndex + 1}", $"Test Step {_currentStepIndex + 1}");
            _progress.currentStepId = _currentStep.stepId;

            StepChangedEvent?.Invoke(_currentStep);
            StepChanged?.Invoke(_currentStep);
        }

        public void GoToStep(string stepId)
        {
            _currentStep = CreateTestStep(stepId, "Test Step");
            _progress.currentStepId = stepId;

            StepChanged?.Invoke(_currentStep);
        }

        public TutorialStep? GetCurrentStep()
        {
            return _currentStep;
        }

        public void CompleteCurrentStep()
        {
            if (_currentStep == null) return;

            if (!_progress.completedSteps.Contains(_currentStep.stepId))
            {
                _progress.completedSteps.Add(_currentStep.stepId);
            }

            NextStep();
        }

        public bool IsFeatureUnlocked(string featureName)
        {
            return _progress.isCompleted || _progress.wasSkipped;
        }

        public void ResetTutorial()
        {
            _progress = new TutorialProgress();
            _currentStep = null;
            _currentStepIndex = -1;
        }

        public void CompleteTutorial()
        {
            _progress.isCompleted = true;
            _currentStep = null;

            TutorialCompleted?.Invoke();
        }

        private TutorialStep CreateTestStep(string id, string title)
        {
            return new TutorialStep
            {
                stepId = id,
                stepType = TutorialStepType.Information,
                titleKey = $"tutorial.{id}.title",
                descriptionKey = $"tutorial.{id}.description",
                targetScene = "",
                nextStepId = "",
                canSkip = true,
                completionCondition = new TutorialCompletionCondition
                {
                    type = TutorialConditionType.TapAnywhere
                }
            };
        }
    }
}
