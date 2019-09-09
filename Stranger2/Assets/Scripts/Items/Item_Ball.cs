using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Ball", menuName = "Items/Item_Ball", order = 1)]
public class Item_Ball : Item
{
    [SerializeField] private string description = "공이다";
    public string Description { get => description; }

    public void Balling()
    {
        Debug.Log("Ball을 사용함");
    }

}