using ScriptableObjects;
using UnityEngine;

public class InventorySlotsContainer : MonoBehaviour
{
    public ItemScriptableObject.ItemType itemType;
    
    [SerializeField] private ItemSlot[] itemSlots;
    [SerializeField] private InventoryManager inventoryManager;
    
    void Start()
    {
        if(itemType != ItemScriptableObject.ItemType.All) gameObject.SetActive(false);
    }
    public int AddItem(string itemName, ItemScriptableObject itemSo, int quantity, Sprite sprite, string description)
    {
        int num = itemSlots.Length;
        for (int i = 0; i < num; i++)
        {
            if (!itemSlots[i].isFull && itemSlots[i].itemName == itemName || itemSlots[i].itemQuantity == 0)
            {
                int extraItems = itemSlots[i].AddItem(itemName, itemSo, quantity, sprite, description);
                if(extraItems > 0) extraItems = AddItem(itemName, itemSo, extraItems, sprite, description);
                
                return extraItems;
            }
        }
        return quantity;
    }
    
    public void DeSelectedAllItems()
    {
        int num = itemSlots.Length;
        for (int i = 0; i < num; i++)
        {
            itemSlots[i].selectedPanel.SetActive(false);
            itemSlots[i].isSelected = false;
        }
    }

    public ItemSlot[] GetItemSlots()
    {
        return itemSlots;
    }
    
    
}
