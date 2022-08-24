 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //
using UnityEngine;

    /// <summary>
    ///GoldPrice full description
    /// </summary>
    
public static class GoldPrice
    {
	    public static float goldPrice { get; private set; } = 47.50f;
	    public static void SetGoldPrice(float p) => goldPrice = p;
    }
