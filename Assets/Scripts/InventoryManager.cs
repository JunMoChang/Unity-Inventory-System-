using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryMenu;

    public InventorySlotContainerCategory inventorySlotContainerCategory;
    public InventorySlotsContainer[] inventorySlotsContainers; 
    [SerializeField] private ItemScriptableObject[] itemSos;
    private InventorySlotsContainer inventoryAllSlotContainer;
    public readonly Dictionary<ItemScriptableObject.ItemType, InventorySlotsContainer> itemSlotContainersDic = new ();
    private ItemSlot currentSelectedItemSlot;
    
    private bool menuActivated;

    void Awake()
    {
        //ItemScriptableObject.ItemType[] type = (ItemScriptableObject.ItemType[])Enum.GetValues(typeof(ItemScriptableObject.ItemType));
        int num = inventorySlotsContainers.Length;
        for (int i = 0; i < num; i++)
        {
            itemSlotContainersDic.Add(inventorySlotsContainers[i].itemType, inventorySlotsContainers[i]);
        }
        itemSlotContainersDic.TryGetValue(ItemScriptableObject.ItemType.All, out inventoryAllSlotContainer);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!menuActivated)
            {
                Time.timeScale = 0;
                inventoryMenu.SetActive(true);
                menuActivated = true;
            } 
            else if (menuActivated)
            {
                Time.timeScale = 1;
                inventoryMenu.SetActive(false);
                menuActivated = false;
            }
        }
    }

    public bool UseItem(ItemScriptableObject targetItemSos)
    {
        int num = itemSos.Length;
        for (int i = 0; i < num; i++)
        {
            if (itemSos[i].itemType == targetItemSos.itemType && itemSos[i].itemName == targetItemSos.itemName)
            {
                return itemSos[i].UseItem();
            }
        }
        return false;
    }
    
    public int AddItem(string itemName, ItemScriptableObject itemSo, int quantity, Sprite sprite, string description)
    {
        InventorySlotsContainer itemSlotContainer = SelectedSlotContainer(itemSo.itemType);
        if(itemSlotContainer == null) return quantity;
        
        return itemSlotContainer.AddItem(itemName, itemSo, quantity, sprite, description);
    }

    public void CreateReferenceInAllContainer(ItemSlot sourceSlot)
    {
        ItemSlot[] itemSlots = inventoryAllSlotContainer.GetItemSlots();
        
        int num = itemSlots.Length;
        for (int i = 0; i < num; i++)
        {
            if (itemSlots[i].itemQuantity != 0) continue;
            
            itemSlots[i].SetAsReferenceSlot(sourceSlot);
            return;
        }
    }
    
    public void SetCurrentSelectedSlot(ItemSlot slot)
    {
        if (currentSelectedItemSlot == slot) return;
        
        if(currentSelectedItemSlot != null) currentSelectedItemSlot.SetDeselected();
        
        currentSelectedItemSlot = slot;
    }
    
    public void DeSelectedAllItems(ItemScriptableObject.ItemType itemType)
    {
        InventorySlotsContainer inventorySlotsContainer = SelectedSlotContainer(itemType);
        if(inventorySlotsContainer == null) return;
        
        inventorySlotsContainer.DeSelectedAllItems();
    }
    
    private InventorySlotsContainer SelectedSlotContainer(ItemScriptableObject.ItemType itemType)
    {
        itemSlotContainersDic.TryGetValue(itemType, out InventorySlotsContainer slotContainer);
        return slotContainer;
    }

    public InventorySlotsContainer GetCurrentSlotsContainer( )
    {
        return inventorySlotContainerCategory.GetCurrentSlotContainer();
    }
}
