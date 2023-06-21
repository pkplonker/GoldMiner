 //
 // Copyright (C) 2023 Stuart Heath. All rights reserved.
 //
using UnityEngine;

    /// <summary>
    ///Extension full description
    /// </summary>
    
public static class Extension
{
	public static bool IsInfinity(this Vector3 vector) =>
		float.IsPositiveInfinity(vector.x) ||
		float.IsPositiveInfinity(vector.y) ||
		float.IsPositiveInfinity(vector.z) ||
		float.IsNegativeInfinity(vector.x) ||
		float.IsNegativeInfinity(vector.y) ||
		float.IsNegativeInfinity(vector.z);
}
