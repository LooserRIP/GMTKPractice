using System.Collections.Generic;
using UnityEngine;


public class gameManager : MonoBehaviour
{
    public GameObject baseItem;
    public Item[] gameItems;

    public List<GameObject> droppedItems;
    public List<int> droppedItemsID;

    public void DropItem(int itemId, Transform location)
    {
        GameObject item = Instantiate(GetPickupableItem(itemId), location.position, location.rotation);
        item.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        droppedItems.Add(item);
        droppedItemsID.Add(itemId);
    }
    public GameObject GetPickupableItem(int itemId)
    {
        Item item = gameItems[itemId];
        GameObject itemObject = baseItem;
        itemObject.GetComponent<SpriteRenderer>().sprite = item.Sprite;
        return itemObject;
    }
}