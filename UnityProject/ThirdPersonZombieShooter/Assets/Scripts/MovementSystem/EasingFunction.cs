using System;
using UnityEngine;

public static class EasingFunction
{
    public static Func<float, float> GetFunction(EasingFunctionID function)
    {
        switch (function)
        {
            case EasingFunctionID.EaseInSine:
                return EaseInSine;
            case EasingFunctionID.EaseOutSine:
                return EaseOutSine;
            case EasingFunctionID.EaseInCubic:
                return EaseInCubic;
            case EasingFunctionID.EaseOutCubic:
                return EaseOutCubic;
            case EasingFunctionID.EaseInQuint:
                return EaseInQuint;
            case EasingFunctionID.EaseOutQuint:
                return EaseOutQuint;
            case EasingFunctionID.EaseInCirc:
                return EaseInCirc;
            case EasingFunctionID.EaseOutCirc:
                return EaseOutCirc;
            case EasingFunctionID.EaseInQuad:
                return EaseInQuad;
            case EasingFunctionID.EaseOutQuad:
                return EaseOutQuad;
            case EasingFunctionID.EaseInQuart:
                return EaseInQuart;
            case EasingFunctionID.EaseOutQuart:
                return EaseOutQuart;
            case EasingFunctionID.EaseInExpo:
                return EaseInExpo;
            case EasingFunctionID.EaseOutExpo:
                return EaseOutExpo;
            default:
                return EaseInSine;
        }
    }

    public static Func<float, float> GetInverseFunction(EasingFunctionID function)
    {
        switch (function)
        {
            case EasingFunctionID.EaseInSine:
                return InverseEaseInSine;
            case EasingFunctionID.EaseOutSine:
                return InverseEaseOutSine;
            case EasingFunctionID.EaseInCubic:
                return InverseEaseInCubic;
            case EasingFunctionID.EaseOutCubic:
                return InverseEaseOutCubic;
            case EasingFunctionID.EaseInQuint:
                return InverseEaseInQuint;
            case EasingFunctionID.EaseOutQuint:
                return InverseEaseOutQuint;
            case EasingFunctionID.EaseInCirc:
                return InverseEaseInCirc;
            case EasingFunctionID.EaseOutCirc:
                return InverseEaseOutCirc;
            case EasingFunctionID.EaseInQuad:
                return InverseEaseInQuad;
            case EasingFunctionID.EaseOutQuad:
                return InverseEaseOutQuad;
            case EasingFunctionID.EaseInQuart:
                return InverseEaseInQuart;
            case EasingFunctionID.EaseOutQuart:
                return InverseEaseOutQuart;
            case EasingFunctionID.EaseInExpo:
                return InverseEaseInExpo;
            case EasingFunctionID.EaseOutExpo:
                return InverseEaseOutExpo;
            default:
                return InverseEaseInSine;
        }
    }

    public static Func<float, float> GetIntegralFunction(EasingFunctionID function) {
        switch (function)
        {
            case EasingFunctionID.EaseInSine:
                return IntegralEaseInSine;
            case EasingFunctionID.EaseOutSine:
                return IntegralEaseOutSine;
            case EasingFunctionID.EaseInCubic:
                return IntegralEaseInCubic;
            case EasingFunctionID.EaseOutCubic:
                return IntegralEaseOutCubic;
            case EasingFunctionID.EaseInQuint:
                return IntegralEaseInQuint;
            case EasingFunctionID.EaseOutQuint:
                return IntegralEaseOutQuint;
            case EasingFunctionID.EaseInCirc:
                return IntegralEaseInCirc;
            case EasingFunctionID.EaseOutCirc:
                return IntegralEaseOutCirc;
            case EasingFunctionID.EaseInQuad:
                return IntegralEaseInQuad;
            case EasingFunctionID.EaseOutQuad:
                return IntegralEaseOutQuad;
            case EasingFunctionID.EaseInQuart:
                return IntegralEaseInQuart;
            case EasingFunctionID.EaseOutQuart:
                return IntegralEaseOutQuart;
            case EasingFunctionID.EaseInExpo:
                return IntegralEaseInExpo;
            case EasingFunctionID.EaseOutExpo:
                return IntegralEaseOutExpo;
            default:
                return IntegralEaseInSine;
        }
    }

    public static float EaseInSine(float x) => 1 - Mathf.Cos((x * Mathf.PI) / 2);

    public static float InverseEaseInSine(float y) => (2 * Mathf.Acos(1 - y)) / Mathf.PI;

    public static float IntegralEaseInSine(float x) => x + (2 / Mathf.PI) * Mathf.Sin((Mathf.PI * x) / 2);

    public static float EaseOutSine(float x) => Mathf.Sin((x * Mathf.PI) / 2);

    public static float InverseEaseOutSine(float y) => Mathf.Asin(y) * 2 / Mathf.PI;

    public static float IntegralEaseOutSine(float x) => -(2 / Mathf.PI) * Mathf.Cos((Mathf.PI * x) / 2);



    public static float EaseInCubic(float x) => x * x * x;

    public static float InverseEaseInCubic(float y) => Mathf.Pow(y, 1f / 3f);

    public static float IntegralEaseInCubic(float x) => 0.25f * Mathf.Pow(x, 4);

    public static float EaseOutCubic(float x) => 1 - Mathf.Pow(1 - x, 3);

    public static float InverseEaseOutCubic(float x) => 1 - Mathf.Pow(1 - x, 1f / 3f);

    public static float IntegralEaseOutCubic(float x) => x + (Mathf.Pow(1 - x, 4) / 4) - 0.25f;



    public static float EaseInQuint(float x) => x * x * x * x * x;

    public static float InverseEaseInQuint(float y) => Mathf.Pow(y, 1f / 5f);

    public static float IntegralEaseInQuint(float x) => (1f / 6f) * Mathf.Pow(x, 6);

    public static float EaseOutQuint(float x) => 1 - Mathf.Pow(1 - x, 5);

    public static float InverseEaseOutQuint(float y) => 1 - Mathf.Pow(1 - y, 1f / 5f);

    public static float IntegralEaseOutQuint(float x) => x + (Mathf.Pow(1 - x, 6) / 6) - (1f / 6f);



    public static float EaseInCirc(float x) => 1 - Mathf.Sqrt(1 - x * x);

    public static float InverseEaseInCirc(float y) => Mathf.Sqrt(1 - (1 - y) * (1 - y));

    public static float IntegralEaseInCirc(float x) => x - 0.5f * (x * Mathf.Sqrt(1 - x * x) + Mathf.Asin(x));

    public static float EaseOutCirc(float x) => Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));

    public static float InverseEaseOutCirc(float y) => 1 - Mathf.Sqrt(1 - y * y);

    public static float IntegralEaseOutCirc(float x) => (x - 1) * Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2)) + Mathf.Asin(x - 1) + Mathf.PI / 2;



    public static float EaseInQuad(float x) => x * x;

    public static float InverseEaseInQuad(float y) => Mathf.Sqrt(y);

    public static float IntegralEaseInQuad(float x) => (1f / 3f) * Mathf.Pow(x, 3);

    public static float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);

    public static float InverseEaseOutQuad(float y) => 1 - Mathf.Sqrt(1 - y);

    public static float IntegralEaseOutQuad(float x) => x - (1f / 3f) * Mathf.Pow(1 - x, 3);



    public static float EaseInQuart(float x) => x * x * x * x;

    public static float InverseEaseInQuart(float y) => Mathf.Pow(y, 1f / 4f);

    public static float IntegralEaseInQuart(float x) => (1f / 5f) * Mathf.Pow(x, 5);

    public static float EaseOutQuart(float x) => 1 - Mathf.Pow(1 - x, 4);

    public static float InverseEaseOutQuart(float y) => 1 - Mathf.Pow(1 - y, 1f / 4f);

    public static float IntegralEaseOutQuart(float x) => x + (Mathf.Pow(1 - x, 5) / 5) - (1f / 5f);



    public static float EaseInExpo(float x) => x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);

    public static float InverseEaseInExpo(float y) => y == 0 ? 0 : (Mathf.Log(y) / Mathf.Log(2) + 10) / 10;

    public static float IntegralEaseInExpo(float x)
    {
        if (x == 0)
            return 0;

        return Mathf.Pow(2, 10 * x - 10) / (10 * Mathf.Log(2));
    }

    public static float EaseOutExpo(float x) => x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);

    public static float InverseEaseOutExpo(float y) => y == 1 ? 1 : -Mathf.Log(1 - y) / (10 * Mathf.Log(2));

    public static float IntegralEaseOutExpo(float x)
    {
        if (x == 1)
            return 1;

        return x + Mathf.Pow(2, -10 * x) / (10 * Mathf.Log(2));
    }



    public static float EaseBezier(float x)
    {
        return x * x * (3.0f - 2.0f * x);
    }
}
