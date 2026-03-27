using System;
using UnityEngine;
using UnityEngine.Android;

namespace Utility
{
    public static class Curves
    {
        // TODO: DIURNAL, NOCTURNAL, CyclicPeak, IdealValuePeak, ;
        public static float Linear(float x)
        {
            return x;
        }

        public static float InverseLinear(float x)
        {
            return 1 - x;
        }

        public static Func<float, float> Exponential(float intensity = 2f)
        {
            // this is not a true exponential. It's just a "polynomial" approximation
            // (not even truly polynomial since a polynomial is a sum of monomials)
            // EXPONENTIAL is the common name in industry.
            return (x) => Mathf.Pow(x, intensity);
        }

        public static Func<float, float> Logarithmic(float intensity = 2f)
        {
            // similar comments apply. This is but a nth-root-based approximation of a logarithmic function.
            return Exponential(1 / intensity);
        }
        
        public static Func<float, float> InvertedExponential(float intensity = 2f)
        {
            return (x) => 1 - Mathf.Pow(x, intensity);
        }
        
        public static Func<float, float> InvertedLogarithmic(float intensity = 2f)
        {
            return (x)=> 1-Mathf.Pow(x, 1 / intensity);
        }

        public static Func<float, float> Sigmoid(float steepness = 12f, float midpoint = 0.5f)
        {
            return (x) =>
            {
                float result = 1f / (1f + Mathf.Exp(-steepness * (x - midpoint)));

                // could be outside bounds. 
                return Mathf.Clamp01(result);
            };
        }

        public static Func<float, float> InvertedSigmoid(float steepness =12f, float midpoint = 0.5f)
        {
            Func<float, float> sigmoid = Sigmoid(steepness, midpoint);
            return (x) =>
            {
                return 1 - sigmoid(x);
            };
        }
        
        // =========================================================
        //  SEMANTIC PRESETS (Ready-to-use constants)
        // =========================================================

        // --- EXPONENTIAL PRESETS (Delay the utility growth) ---
        
        // Exponent 1.5: The curve is noticeable, but forgiving.
        public static readonly Func<float, float> MILD_EXPONENTIAL = Exponential(1.5f); 
        // Exponent 2: The standard quadratic curve.
        public static readonly Func<float, float> MEDIUM_EXPONENTIAL = Exponential(2f); 
        // Exponent 4: Ignores the value until the very end, then shoots up.
        public static readonly Func<float, float> AGGRESSIVE_EXPONENTIAL = Exponential(4f);

        // --- INVERTED EXPONENTIAL PRESETS (Utility drops slowly at first, then crashes) ---
        
        public static readonly Func<float, float> INVERTED_MILD_EXPONENTIAL = InvertedExponential(1.5f); 
        public static readonly Func<float, float> INVERTED_MEDIUM_EXPONENTIAL = InvertedExponential(2f); 
        public static readonly Func<float, float> INVERTED_AGGRESSIVE_EXPONENTIAL = InvertedExponential(4f);


        // --- LOGARITHMIC PRESETS (Trigger the utility growth early) ---
        
        // Reacts quickly, but not excessively.
        public static readonly Func<float, float> MILD_LOGARITHMIC = Logarithmic(1.5f);
        // The classic square root. High utility right away.
        public static readonly Func<float, float> MEDIUM_LOGARITHMIC = Logarithmic(2f);
        // Even a tiny amount of input yields almost 90% utility.
        public static readonly Func<float, float> AGGRESSIVE_LOGARITHMIC = Logarithmic(4f);

        // --- INVERTED LOGARITHMIC PRESETS (Utility drops sharply at first, then stabilizes) ---
        
        public static readonly Func<float, float> INVERTED_MILD_LOGARITHMIC = InvertedLogarithmic(1.5f);
        public static readonly Func<float, float> INVERTED_MEDIUM_LOGARITHMIC = InvertedLogarithmic(2f);
        public static readonly Func<float, float> INVERTED_AGGRESSIVE_LOGARITHMIC = InvertedLogarithmic(4f);


        // --- SIGMOID PRESETS ('S' shaped transitions) ---
        
        // Very smooth transition.
        public static readonly Func<float, float> MILD_SIGMOID = Sigmoid(5f);
        // The standard, balanced transition.
        public static readonly Func<float, float> MEDIUM_SIGMOID = Sigmoid(10f);
        // Almost a step function. A sharp 0-to-1 change in the middle of the graph.
        public static readonly Func<float, float> AGGRESSIVE_SIGMOID = Sigmoid(20f);

        // --- INVERTED SIGMOID PRESETS (Inverted 'S' shaped transitions) ---
        
        public static readonly Func<float, float> INVERTED_MILD_SIGMOID = InvertedSigmoid(5f);
        public static readonly Func<float, float> INVERTED_MEDIUM_SIGMOID = InvertedSigmoid(10f);
        public static readonly Func<float, float> INVERTED_AGGRESSIVE_SIGMOID = InvertedSigmoid(20f);
        
    }
}