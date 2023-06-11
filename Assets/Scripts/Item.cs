using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public ItemType Type;
    public ItemRarity Rarity = ItemRarity.Common;
    public float size = 1f;
    public float damage = 0f;
    public float knockback = 0f;
    
}
public enum ItemType {
    Sword,
    Bow,
    Potion,
    Trap,
    Misc
}
public enum ItemRarity {
    Common,
    Uncommon,
    Rare,
    Epic,
    Awesome
}