using System;
using UnityEngine;

public static class FalloffGeneration
{
    public static float[,] GenerateFalloffMap(int size, float inset, float exponent = 1f, float noiseScale = 0.01f, float noiseWeight = 0.1f)
    {
        var map = new float[size, size];

        var maxDistance = size / 2f;

        var ratioMultiplier = size / inset;

        var offsetX = UnityEngine.Random.value * 1000;
        var offsetY = UnityEngine.Random.value * 1000;

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var distanceX = maxDistance - Math.Abs(x - maxDistance);
                var distanceY = maxDistance - Math.Abs(y - maxDistance);

                var distanceFromEdge = Math.Min(distanceX, distanceY) - inset;

                if (distanceFromEdge < 0)
                {
                    var normalizedDistanceFromEdge = Mathf.Clamp01(-distanceFromEdge / inset);

                    var noise = Mathf.PerlinNoise(x * noiseScale + offsetX, y * noiseScale + offsetY) * noiseWeight;

                    var distanceWithNoise = Mathf.Clamp01(normalizedDistanceFromEdge + noise);

                    map[x, y] = Evaluate(distanceWithNoise, ratioMultiplier * exponent);
                }
                else
                {
                    map[x, y] = 0;
                }
            }
        }

        return map;
    }

    private static float Evaluate(float x, float exponent)
    {
        var smootherStep = x * x * x * (x * (x * 6 - 15) + 10);
        return Mathf.Pow(smootherStep, exponent);
    }
}
