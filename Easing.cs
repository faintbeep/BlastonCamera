using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlastonCameraBehaviour
{
    public class Easing
    {
        public enum Function { linear, easeInOutCubic };

        public static float Ease(Easing.Function function, float x)
        {
            x = (float)Math.Max(0, Math.Min(1, x));
            switch(function)
            {
                default:
                case Easing.Function.linear:
                    return x;
                case Easing.Function.easeInOutCubic:
                    return EaseInOutCubic(x);
            }
        }

        private static float EaseInOutCubic(float x)
        {
            return (float)(x < 0.5 ? 4.0 * Math.Pow(x, 3) : 1.0 - Math.Pow(-2 * x + 2, 3) / 2.0);
        }
    }
}
