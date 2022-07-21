// Easing functions released on easings.net under GPL v3

using System.Collections.Generic;
using UnityEngine;

namespace BlastonCameraBehaviour
{
    public class Easing
    {
        public enum Function { 
            linear,
            easeInSine,
            easeOutSine,
            easeInOutSine,
            easeInQuad,
            easeOutQuad,
            easeInOutQuad,
            easeInCubic,
            easeOutCubic,
            easeInOutCubic,
            easeInBounce,
            easeOutBounce,
            easeInOutBounce
        };

        private delegate float EasingFn(float x);

        private static Dictionary<Function, EasingFn> easingFns = new Dictionary<Function, EasingFn> {
            { Function.linear, x => x },
            { Function.easeInSine, x =>  1 - Mathf.Cos((x * Mathf.PI) / 2) },
            { Function.easeOutSine, x => Mathf.Sin((x * Mathf.PI) / 2) },
            { Function.easeInOutSine, x => -(Mathf.Cos(Mathf.PI * x) - 1) / 2 },
            { Function.easeInQuad, x => x * x },
            { Function.easeOutQuad, x => 1 - (1 - x) * (1 - x) },
            { Function.easeInOutQuad, x => x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2 },
            { Function.easeInCubic, x => Mathf.Pow(x, 3) },
            { Function.easeOutCubic, x => 1 - Mathf.Pow(1 - x, 3) },
            { Function.easeInOutCubic, x => (x < 0.5f ? 4.0f * Mathf.Pow(x, 3) : 1.0f - Mathf.Pow(-2 * x + 2, 3) / 2.0f) },
            { Function.easeInBounce, x => 1 - Ease(Function.easeOutBounce, x) },
            { Function.easeOutBounce, x =>
                {
                    float n1 = 7.5625f;
                    float d1 = 2.75f;

                    if (x < 1 / d1) {
                        return n1 * x * x;
                    } else if (x < 2 / d1) {
                        return n1 * (x -= 1.5f / d1) * x + 0.75f;
                    } else if (x < 2.5 / d1) {
                        return n1 * (x -= 2.25f / d1) * x + 0.9375f;
                    } else {
                        return n1 * (x -= 2.625f / d1) * x + 0.984375f;
                    }
                }
            },
            { Function.easeInOutBounce, x =>
                {
                    return x < 0.5
                      ? (1 - Ease(Function.easeOutBounce, 1 - 2 * x)) / 2
                      : (1 + Ease(Function.easeOutBounce, 2 * x - 1)) / 2;
                }
            }
        };

        public static float Ease(Easing.Function function, float x)
        {
            x = Mathf.Max(0, Mathf.Min(1, x));
            return easingFns[function](x);
        }
    }
}
