#nullable enable
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace BakaTest.UI.LoadingScreen
{
    /// <summary>
    /// ローディング画面管理サービスの実装
    /// </summary>
    public class LoadingScreenService : ILoadingScreenService
    {
        private GameObject? _loadingScreenRoot;
        private UIDocument? _uiDocument;
        private VisualElement? _rootElement;
        private VisualElement? _container;
        private Label? _messageLabel;
        private VisualElement? _progressBar;
        private VisualElement? _progressFill;

        private bool _isShowing;
        private MonoBehaviour? _coroutineRunner;

        public bool IsShowing => _isShowing;

        public event Action? LoadingScreenShown;
        public event Action? LoadingScreenHidden;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoadingScreenService()
        {
            Initialize();
            Debug.Log("[LoadingScreenService] Initialized.");
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            // LoadingScreenのルートオブジェクトを作成
            _loadingScreenRoot = new GameObject("LoadingScreen");
            UnityEngine.Object.DontDestroyOnLoad(_loadingScreenRoot);

            // コルーチンランナー
            _coroutineRunner = _loadingScreenRoot.AddComponent<CoroutineRunnerBehaviour>();

            // UIDocumentを追加
            _uiDocument = _loadingScreenRoot.AddComponent<UIDocument>();

            // プログラム的にUI構造を作成
            CreateLoadingScreenUI();

            // 初期状態は非表示
            if (_rootElement != null)
            {
                _rootElement.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// ローディング画面のUIを作成します
        /// </summary>
        private void CreateLoadingScreenUI()
        {
            if (_uiDocument == null) return;

            // ルート要素を作成
            _rootElement = new VisualElement();
            _rootElement.name = "LoadingScreenRoot";
            _rootElement.style.position = Position.Absolute;
            _rootElement.style.left = 0;
            _rootElement.style.top = 0;
            _rootElement.style.right = 0;
            _rootElement.style.bottom = 0;
            _rootElement.style.backgroundColor = new Color(0, 0, 0, 0.9f);
            _rootElement.style.alignItems = Align.Center;
            _rootElement.style.justifyContent = Justify.Center;

            // コンテナ
            _container = new VisualElement();
            _container.name = "LoadingContainer";
            _container.style.alignItems = Align.Center;
            _container.style.width = new Length(80, LengthUnit.Percent);
            _container.style.maxWidth = 600;

            // スピナー（簡易的なローディングアニメーション）
            var spinner = new VisualElement();
            spinner.name = "LoadingSpinner";
            spinner.style.width = 80;
            spinner.style.height = 80;
            spinner.style.borderTopLeftRadius = 40;
            spinner.style.borderTopRightRadius = 40;
            spinner.style.borderBottomLeftRadius = 40;
            spinner.style.borderBottomRightRadius = 40;
            spinner.style.borderLeftWidth = 6;
            spinner.style.borderRightWidth = 6;
            spinner.style.borderTopWidth = 6;
            spinner.style.borderBottomWidth = 6;
            spinner.style.borderLeftColor = new Color(1, 1, 1, 0.3f);
            spinner.style.borderRightColor = new Color(1, 1, 1, 0.3f);
            spinner.style.borderTopColor = Color.white;
            spinner.style.borderBottomColor = new Color(1, 1, 1, 0.3f);
            spinner.style.marginBottom = 30;

            // メッセージラベル
            _messageLabel = new Label("Loading...");
            _messageLabel.name = "LoadingMessage";
            _messageLabel.style.fontSize = 24;
            _messageLabel.style.color = Color.white;
            _messageLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _messageLabel.style.marginBottom = 30;

            // プログレスバー
            _progressBar = new VisualElement();
            _progressBar.name = "ProgressBar";
            _progressBar.style.width = new Length(100, LengthUnit.Percent);
            _progressBar.style.height = 8;
            _progressBar.style.backgroundColor = new Color(1, 1, 1, 0.2f);
            _progressBar.style.borderTopLeftRadius = 4;
            _progressBar.style.borderTopRightRadius = 4;
            _progressBar.style.borderBottomLeftRadius = 4;
            _progressBar.style.borderBottomRightRadius = 4;
            _progressBar.style.overflow = Overflow.Hidden;

            _progressFill = new VisualElement();
            _progressFill.name = "ProgressFill";
            _progressFill.style.width = new Length(0, LengthUnit.Percent);
            _progressFill.style.height = new Length(100, LengthUnit.Percent);
            _progressFill.style.backgroundColor = Color.white;

            // 階層構造を構築
            _progressBar.Add(_progressFill);
            _container.Add(spinner);
            _container.Add(_messageLabel);
            _container.Add(_progressBar);
            _rootElement.Add(_container);

            // UIDocumentに設定
            _uiDocument.rootVisualElement.Add(_rootElement);

            // スピナーアニメーションを開始
            StartSpinnerAnimation(spinner);
        }

        /// <summary>
        /// スピナーアニメーションを開始します
        /// </summary>
        private void StartSpinnerAnimation(VisualElement spinner)
        {
            if (_coroutineRunner == null) return;
            _coroutineRunner.StartCoroutine(SpinnerAnimationCoroutine(spinner));
        }

        private IEnumerator SpinnerAnimationCoroutine(VisualElement spinner)
        {
            float rotation = 0f;
            while (true)
            {
                rotation += Time.deltaTime * 360f; // 1秒で1回転
                if (rotation >= 360f) rotation -= 360f;
                spinner.style.rotate = new Rotate(new Angle(rotation, AngleUnit.Degree));
                yield return null;
            }
        }

        public void Show(string? message = null)
        {
            if (_rootElement == null) return;

            _isShowing = true;
            _rootElement.style.display = DisplayStyle.Flex;
            _rootElement.style.opacity = 1f;

            if (message != null && _messageLabel != null)
            {
                _messageLabel.text = message;
            }

            SetProgress(0f);

            LoadingScreenShown?.Invoke();
            Debug.Log($"[LoadingScreenService] Loading screen shown: {message ?? "Loading..."}");
        }

        public void Hide(bool fadeOut = true)
        {
            if (_rootElement == null || !_isShowing) return;

            if (fadeOut && _coroutineRunner != null)
            {
                _coroutineRunner.StartCoroutine(FadeOutCoroutine());
            }
            else
            {
                _rootElement.style.display = DisplayStyle.None;
                _isShowing = false;
                LoadingScreenHidden?.Invoke();
                Debug.Log("[LoadingScreenService] Loading screen hidden.");
            }
        }

        private IEnumerator FadeOutCoroutine()
        {
            if (_rootElement == null) yield break;

            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _rootElement.style.opacity = 1f - t;
                yield return null;
            }

            _rootElement.style.display = DisplayStyle.None;
            _rootElement.style.opacity = 1f; // Reset for next time
            _isShowing = false;
            LoadingScreenHidden?.Invoke();

            Debug.Log("[LoadingScreenService] Loading screen hidden (fade out).");
        }

        public void SetProgress(float progress)
        {
            if (_progressFill == null) return;

            progress = Mathf.Clamp01(progress);
            _progressFill.style.width = new Length(progress * 100f, LengthUnit.Percent);
        }

        public void SetMessage(string message)
        {
            if (_messageLabel == null) return;

            _messageLabel.text = message;
            Debug.Log($"[LoadingScreenService] Message updated: {message}");
        }

        private class CoroutineRunnerBehaviour : MonoBehaviour { }
    }
}
