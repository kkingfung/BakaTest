#nullable enable
using UnityEngine;

namespace BakaTest.UI.Animation
{
    /// <summary>
    /// イージング関数の種類
    /// </summary>
    public enum EasingFunction
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce
    }

    /// <summary>
    /// イージング関数の実装
    /// </summary>
    public static class Easing
    {
        /// <summary>
        /// 指定されたイージング関数を評価します
        /// </summary>
        /// <param name="function">イージング関数の種類</param>
        /// <param name="t">時間（0.0 ~ 1.0）</param>
        /// <returns>イージング適用後の値</returns>
        public static float Evaluate(EasingFunction function, float t)
        {
            return function switch
            {
                EasingFunction.Linear => Linear(t),
                EasingFunction.EaseInQuad => EaseInQuad(t),
                EasingFunction.EaseOutQuad => EaseOutQuad(t),
                EasingFunction.EaseInOutQuad => EaseInOutQuad(t),
                EasingFunction.EaseInCubic => EaseInCubic(t),
                EasingFunction.EaseOutCubic => EaseOutCubic(t),
                EasingFunction.EaseInOutCubic => EaseInOutCubic(t),
                EasingFunction.EaseInQuart => EaseInQuart(t),
                EasingFunction.EaseOutQuart => EaseOutQuart(t),
                EasingFunction.EaseInOutQuart => EaseInOutQuart(t),
                EasingFunction.EaseInQuint => EaseInQuint(t),
                EasingFunction.EaseOutQuint => EaseOutQuint(t),
                EasingFunction.EaseInOutQuint => EaseInOutQuint(t),
                EasingFunction.EaseInSine => EaseInSine(t),
                EasingFunction.EaseOutSine => EaseOutSine(t),
                EasingFunction.EaseInOutSine => EaseInOutSine(t),
                EasingFunction.EaseInExpo => EaseInExpo(t),
                EasingFunction.EaseOutExpo => EaseOutExpo(t),
                EasingFunction.EaseInOutExpo => EaseInOutExpo(t),
                EasingFunction.EaseInCirc => EaseInCirc(t),
                EasingFunction.EaseOutCirc => EaseOutCirc(t),
                EasingFunction.EaseInOutCirc => EaseInOutCirc(t),
                EasingFunction.EaseInBack => EaseInBack(t),
                EasingFunction.EaseOutBack => EaseOutBack(t),
                EasingFunction.EaseInOutBack => EaseInOutBack(t),
                EasingFunction.EaseInElastic => EaseInElastic(t),
                EasingFunction.EaseOutElastic => EaseOutElastic(t),
                EasingFunction.EaseInOutElastic => EaseInOutElastic(t),
                EasingFunction.EaseInBounce => EaseInBounce(t),
                EasingFunction.EaseOutBounce => EaseOutBounce(t),
                EasingFunction.EaseInOutBounce => EaseInOutBounce(t),
                _ => Linear(t)
            };
        }

        // Linear
        public static float Linear(float t) => t;

        // Quadratic
        public static float EaseInQuad(float t) => t * t;
        public static float EaseOutQuad(float t) => t * (2 - t);
        public static float EaseInOutQuad(float t) => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;

        // Cubic
        public static float EaseInCubic(float t) => t * t * t;
        public static float EaseOutCubic(float t) => (--t) * t * t + 1;
        public static float EaseInOutCubic(float t) => t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;

        // Quartic
        public static float EaseInQuart(float t) => t * t * t * t;
        public static float EaseOutQuart(float t) => 1 - (--t) * t * t * t;
        public static float EaseInOutQuart(float t) => t < 0.5f ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;

        // Quintic
        public static float EaseInQuint(float t) => t * t * t * t * t;
        public static float EaseOutQuint(float t) => 1 + (--t) * t * t * t * t;
        public static float EaseInOutQuint(float t) => t < 0.5f ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;

        // Sine
        public static float EaseInSine(float t) => 1 - Mathf.Cos(t * Mathf.PI / 2);
        public static float EaseOutSine(float t) => Mathf.Sin(t * Mathf.PI / 2);
        public static float EaseInOutSine(float t) => -(Mathf.Cos(Mathf.PI * t) - 1) / 2;

        // Exponential
        public static float EaseInExpo(float t) => t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);
        public static float EaseOutExpo(float t) => t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
        public static float EaseInOutExpo(float t)
        {
            return t == 0 ? 0 : t == 1 ? 1 : t < 0.5f ?
                Mathf.Pow(2, 20 * t - 10) / 2 :
                (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
        }

        // Circular
        public static float EaseInCirc(float t) => 1 - Mathf.Sqrt(1 - t * t);
        public static float EaseOutCirc(float t) => Mathf.Sqrt(1 - (--t) * t);
        public static float EaseInOutCirc(float t)
        {
            return t < 0.5f ?
                (1 - Mathf.Sqrt(1 - 4 * t * t)) / 2 :
                (Mathf.Sqrt(1 - (-2 * t + 2) * (-2 * t + 2)) + 1) / 2;
        }

        // Back
        private const float c1 = 1.70158f;
        private const float c2 = c1 * 1.525f;
        private const float c3 = c1 + 1;

        public static float EaseInBack(float t) => c3 * t * t * t - c1 * t * t;
        public static float EaseOutBack(float t) => 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        public static float EaseInOutBack(float t)
        {
            return t < 0.5f ?
                (Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2 :
                (Mathf.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        }

        // Elastic
        private const float c4 = (2 * Mathf.PI) / 3;
        private const float c5 = (2 * Mathf.PI) / 4.5f;

        public static float EaseInElastic(float t)
        {
            return t == 0 ? 0 : t == 1 ? 1 :
                -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);
        }

        public static float EaseOutElastic(float t)
        {
            return t == 0 ? 0 : t == 1 ? 1 :
                Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
        }

        public static float EaseInOutElastic(float t)
        {
            return t == 0 ? 0 : t == 1 ? 1 : t < 0.5f ?
                -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2 :
                (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2 + 1;
        }

        // Bounce
        private const float n1 = 7.5625f;
        private const float d1 = 2.75f;

        public static float EaseOutBounce(float t)
        {
            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }
            else if (t < 2.5 / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        }

        public static float EaseInBounce(float t) => 1 - EaseOutBounce(1 - t);

        public static float EaseInOutBounce(float t)
        {
            return t < 0.5f ?
                (1 - EaseOutBounce(1 - 2 * t)) / 2 :
                (1 + EaseOutBounce(2 * t - 1)) / 2;
        }
    }
}
