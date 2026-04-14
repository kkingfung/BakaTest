#nullable enable
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace BakaTest.UI.Animation
{
    /// <summary>
    /// UI Toolkit用のアニメーションユーティリティ
    /// </summary>
    public static class UIAnimator
    {
        private static MonoBehaviour? _coroutineRunner;

        /// <summary>
        /// コルーチンランナーを初期化します
        /// </summary>
        private static void EnsureCoroutineRunner()
        {
            if (_coroutineRunner == null)
            {
                var go = new GameObject("UIAnimator_CoroutineRunner");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _coroutineRunner = go.AddComponent<CoroutineRunnerBehaviour>();
            }
        }

        #region Fade Animations

        /// <summary>
        /// 要素をフェードインします
        /// </summary>
        public static Coroutine FadeIn(VisualElement element, float duration, EasingFunction easing = EasingFunction.Linear, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            return _coroutineRunner!.StartCoroutine(FadeCoroutine(element, 0f, 1f, duration, easing, onComplete));
        }

        /// <summary>
        /// 要素をフェードアウトします
        /// </summary>
        public static Coroutine FadeOut(VisualElement element, float duration, EasingFunction easing = EasingFunction.Linear, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            return _coroutineRunner!.StartCoroutine(FadeCoroutine(element, 1f, 0f, duration, easing, onComplete));
        }

        /// <summary>
        /// 要素を指定の透明度にフェードします
        /// </summary>
        public static Coroutine FadeTo(VisualElement element, float targetOpacity, float duration, EasingFunction easing = EasingFunction.Linear, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            float currentOpacity = element.resolvedStyle.opacity;
            return _coroutineRunner!.StartCoroutine(FadeCoroutine(element, currentOpacity, targetOpacity, duration, easing, onComplete));
        }

        private static IEnumerator FadeCoroutine(VisualElement element, float startOpacity, float endOpacity, float duration, EasingFunction easing, Action? onComplete)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = Easing.Evaluate(easing, t);
                element.style.opacity = Mathf.Lerp(startOpacity, endOpacity, easedT);
                yield return null;
            }

            element.style.opacity = endOpacity;
            onComplete?.Invoke();
        }

        #endregion

        #region Scale Animations

        /// <summary>
        /// 要素をスケールアップします（0から1）
        /// </summary>
        public static Coroutine ScaleIn(VisualElement element, float duration, EasingFunction easing = EasingFunction.EaseOutBack, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            return _coroutineRunner!.StartCoroutine(ScaleCoroutine(element, Vector2.zero, Vector2.one, duration, easing, onComplete));
        }

        /// <summary>
        /// 要素をスケールダウンします（1から0）
        /// </summary>
        public static Coroutine ScaleOut(VisualElement element, float duration, EasingFunction easing = EasingFunction.EaseInBack, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            return _coroutineRunner!.StartCoroutine(ScaleCoroutine(element, Vector2.one, Vector2.zero, duration, easing, onComplete));
        }

        /// <summary>
        /// 要素をパルス（拡大→縮小）します
        /// </summary>
        public static Coroutine Pulse(VisualElement element, float scaleAmount = 1.2f, float duration = 0.3f, EasingFunction easing = EasingFunction.EaseInOutCubic, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            return _coroutineRunner!.StartCoroutine(PulseCoroutine(element, scaleAmount, duration, easing, onComplete));
        }

        private static IEnumerator ScaleCoroutine(VisualElement element, Vector2 startScale, Vector2 endScale, float duration, EasingFunction easing, Action? onComplete)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = Easing.Evaluate(easing, t);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedT);
                element.style.scale = new Scale(new Vector3(scale.x, scale.y, 1f));
                yield return null;
            }

            element.style.scale = new Scale(new Vector3(endScale.x, endScale.y, 1f));
            onComplete?.Invoke();
        }

        private static IEnumerator PulseCoroutine(VisualElement element, float scaleAmount, float duration, EasingFunction easing, Action? onComplete)
        {
            Vector2 originalScale = new Vector2(element.resolvedStyle.scale.value.x, element.resolvedStyle.scale.value.y);
            Vector2 targetScale = originalScale * scaleAmount;

            // Scale up
            float elapsed = 0f;
            float halfDuration = duration * 0.5f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                float easedT = Easing.Evaluate(easing, t);
                Vector2 scale = Vector2.Lerp(originalScale, targetScale, easedT);
                element.style.scale = new Scale(new Vector3(scale.x, scale.y, 1f));
                yield return null;
            }

            // Scale down
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                float easedT = Easing.Evaluate(easing, t);
                Vector2 scale = Vector2.Lerp(targetScale, originalScale, easedT);
                element.style.scale = new Scale(new Vector3(scale.x, scale.y, 1f));
                yield return null;
            }

            element.style.scale = new Scale(new Vector3(originalScale.x, originalScale.y, 1f));
            onComplete?.Invoke();
        }

        #endregion

        #region Slide Animations

        /// <summary>
        /// 要素を左からスライドインします
        /// </summary>
        public static Coroutine SlideInFromLeft(VisualElement element, float distance, float duration, EasingFunction easing = EasingFunction.EaseOutCubic, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            return _coroutineRunner!.StartCoroutine(SlideCoroutine(element, new Vector3(-distance, 0, 0), Vector3.zero, duration, easing, onComplete));
        }

        /// <summary>
        /// 要素を右からスライドインします
        /// </summary>
        public static Coroutine SlideInFromRight(VisualElement element, float distance, float duration, EasingFunction easing = EasingFunction.EaseOutCubic, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            return _coroutineRunner!.StartCoroutine(SlideCoroutine(element, new Vector3(distance, 0, 0), Vector3.zero, duration, easing, onComplete));
        }

        /// <summary>
        /// 要素を上からスライドインします
        /// </summary>
        public static Coroutine SlideInFromTop(VisualElement element, float distance, float duration, EasingFunction easing = EasingFunction.EaseOutCubic, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            return _coroutineRunner!.StartCoroutine(SlideCoroutine(element, new Vector3(0, -distance, 0), Vector3.zero, duration, easing, onComplete));
        }

        /// <summary>
        /// 要素を下からスライドインします
        /// </summary>
        public static Coroutine SlideInFromBottom(VisualElement element, float distance, float duration, EasingFunction easing = EasingFunction.EaseOutCubic, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            return _coroutineRunner!.StartCoroutine(SlideCoroutine(element, new Vector3(0, distance, 0), Vector3.zero, duration, easing, onComplete));
        }

        private static IEnumerator SlideCoroutine(VisualElement element, Vector3 startPosition, Vector3 endPosition, float duration, EasingFunction easing, Action? onComplete)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = Easing.Evaluate(easing, t);
                Vector3 position = Vector3.Lerp(startPosition, endPosition, easedT);
                element.style.translate = new Translate(position.x, position.y);
                yield return null;
            }

            element.style.translate = new Translate(endPosition.x, endPosition.y);
            onComplete?.Invoke();
        }

        #endregion

        #region Combined Animations

        /// <summary>
        /// フェードインとスケールアップを同時に実行します
        /// </summary>
        public static void FadeInAndScaleIn(VisualElement element, float duration, EasingFunction easing = EasingFunction.EaseOutBack, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            _coroutineRunner!.StartCoroutine(CombinedFadeAndScaleCoroutine(element, true, duration, easing, onComplete));
        }

        /// <summary>
        /// フェードアウトとスケールダウンを同時に実行します
        /// </summary>
        public static void FadeOutAndScaleOut(VisualElement element, float duration, EasingFunction easing = EasingFunction.EaseInBack, Action? onComplete = null)
        {
            EnsureCoroutineRunner();
            _coroutineRunner!.StartCoroutine(CombinedFadeAndScaleCoroutine(element, false, duration, easing, onComplete));
        }

        private static IEnumerator CombinedFadeAndScaleCoroutine(VisualElement element, bool fadeIn, float duration, EasingFunction easing, Action? onComplete)
        {
            float startOpacity = fadeIn ? 0f : 1f;
            float endOpacity = fadeIn ? 1f : 0f;
            Vector2 startScale = fadeIn ? Vector2.zero : Vector2.one;
            Vector2 endScale = fadeIn ? Vector2.one : Vector2.zero;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = Easing.Evaluate(easing, t);

                element.style.opacity = Mathf.Lerp(startOpacity, endOpacity, easedT);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedT);
                element.style.scale = new Scale(new Vector3(scale.x, scale.y, 1f));

                yield return null;
            }

            element.style.opacity = endOpacity;
            element.style.scale = new Scale(new Vector3(endScale.x, endScale.y, 1f));
            onComplete?.Invoke();
        }

        #endregion

        #region Utility

        /// <summary>
        /// アニメーションを停止します
        /// </summary>
        public static void Stop(Coroutine coroutine)
        {
            if (_coroutineRunner != null && coroutine != null)
            {
                _coroutineRunner.StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// すべてのアニメーションを停止します
        /// </summary>
        public static void StopAll()
        {
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StopAllCoroutines();
            }
        }

        #endregion

        private class CoroutineRunnerBehaviour : MonoBehaviour { }
    }
}
