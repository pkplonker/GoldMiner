using System;
using UnityEngine;

public static class FalloffGeneration
{
    public static float[,] GenerateFalloffMap(int size, float inset, float exponent =1f)
    {
        var map = new float[size, size];

        // The maximum possible distance from the edge of the map.
        float maxDistance = size / 2f;

        // Calculate the ratio between the size and the inset to use as a multiplier for the exponent.
        float ratioMultiplier = size / inset;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // The distance from the center of the map.
                float distanceX = maxDistance - Math.Abs(x - maxDistance);
                float distanceY = maxDistance - Math.Abs(y - maxDistance);

                // The distance from the edge of the map (taking the inset into account).
                float distanceFromEdge = Math.Min(distanceX, distanceY) - inset;

                // Only apply the Evaluate function if we're within the inset.
                if (distanceFromEdge < 0)
                {
                    // We normalize the distance (make it between 0 and 1) for easier handling in the Evaluate function.
                    float normalizedDistanceFromEdge = Mathf.Clamp01(-distanceFromEdge / inset);

                    // Use the Evaluate function to get a value for this point.
                    map[x, y] = Evaluate(normalizedDistanceFromEdge, ratioMultiplier*exponent);
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
        // Use a smootherstep function to smoothly increase the height and power it by the exponent to control the rate of increase.
        float smootherStep = x * x * x * (x * (x * 6 - 15) + 10);
        float value = Mathf.Pow(smootherStep, exponent);
        return value;
    }
}
