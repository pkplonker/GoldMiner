 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //
using UnityEngine;

    /// <summary>
    ///Item full description
    /// </summary>
    	[CreateAssetMenu(fileName = "New Item",menuName = "Items/Base Item")]

public class Item : ScriptableObject
    {
	    public string _itemName = "New Item";
	    [SerializeField] protected float _value = 10f;
	    public Sprite _sprite;
	    public virtual float GetValue() => _value;
    }
