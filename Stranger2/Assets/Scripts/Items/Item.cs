using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    // [SerializeField] private int stackSize;
    [SerializeField] private Sprite icon;
    public Sprite Icon { get => icon;}
}
