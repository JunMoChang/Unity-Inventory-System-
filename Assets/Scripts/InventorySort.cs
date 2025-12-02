using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class InventorySort : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private ItemSortType currentSortType;
    private SortOrder currentSortOrder;
    
    [SerializeField] private Dropdown sortTypeDropdown;
    [SerializeField] private Text sortOrderText;
    [SerializeField] private Button sortOrderButton;
    class ItemInfo
    {
        public string itemName;
        public ItemScriptableObject itemSo;
        public int itemQuantity;
        public Sprite itemSprite;
        public string itemDescription;
        public ItemSlot sourceSlot;
    }
    private void Awake()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void Start()
    {
        if (sortTypeDropdown != null)
        {
            sortTypeDropdown.ClearOptions();
            List<string> text = new List<string>
            {
                "默认顺序",
                "按名称",
                "按类型",
                "按数量",
                "按稀有度"
            };
            sortTypeDropdown.AddOptions(text);
            sortTypeDropdown.onValueChanged.AddListener(ChangeSortType);
        }
        sortOrderButton.onClick.AddListener(ChangeSortOrder);
        currentSortOrder = SortOrder.Ascending;
    }

    private void ChangeSortType(int index)
    {
        currentSortType = (ItemSortType)index;
        SortCurrentContainer();
    }

    private void ChangeSortOrder()
    {
        currentSortOrder = (currentSortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
        
        sortOrderText.text = currentSortOrder == SortOrder.Ascending ? "↑升序" : "↓降序";
        SortCurrentContainer();
    }
    
    private void SortCurrentContainer()
    {
        if(currentSortType == ItemSortType.None) return;
        
        InventorySlotsContainer currentSlotsContainer = inventoryManager.GetCurrentSlotsContainer();
        if(currentSlotsContainer == null) return;
        
        if (currentSlotsContainer.itemType == ItemScriptableObject.ItemType.All)
        {
            SortAllSlotsContainer();
            return;
        }

        SortOtherSlotsContainer(currentSlotsContainer);
    }

    private void SortAllSlotsContainer()
    {
        List<ItemInfo> allSortedItemSlots = new List<ItemInfo>();
        InventorySlotsContainer allSlotsContainer = null;
        
        InventorySlotsContainer[] currentSlotsContainer = inventoryManager.inventorySlotsContainers;
        int num;
        foreach (InventorySlotsContainer container in currentSlotsContainer)
        {
            if (container.itemType == ItemScriptableObject.ItemType.All)
            {
                allSlotsContainer = container;
                continue;
            }
            
            ItemSlot[] slots = container.GetItemSlots();
            num = slots.Length;
            for (int i = 0; i < num; i++)
            {
                if (slots[i].itemQuantity > 0)
                {
                    allSortedItemSlots.Add(new ItemInfo
                    {
                        itemName = slots[i].itemName,
                        itemSo = slots[i].itemSo,
                        itemQuantity = slots[i].itemQuantity,
                        itemSprite = slots[i].itemSprite,
                        itemDescription = slots[i].itemDescription,
                        sourceSlot = slots[i]
                    });
                }
            }
        }
        if(allSlotsContainer == null) return;
        
        allSortedItemSlots = ItemSortByType(allSortedItemSlots);
        
        ItemSlot[] allContainerSlots = allSlotsContainer.GetItemSlots();
        num = allContainerSlots.Length;
        for (int i = 0; i < num; i++)
        {
            allContainerSlots[i].EmptySlot();
        }
        int num2 = allSortedItemSlots.Count;
        for (int i = 0; i < num && i < num2; i++)
        {
            allContainerSlots[i].SetAsReferenceSlot(allSortedItemSlots[i].sourceSlot);
        }
    }

    private void SortOtherSlotsContainer(InventorySlotsContainer container)
    {
        ItemSlot[] slots = container.GetItemSlots();

        List<ItemInfo> sortedSlots = new List<ItemInfo>();
        int num = slots.Length;
        for (int i = 0; i < num; i++)
        {
            if(slots[i].itemQuantity <= 0) continue;
            
            sortedSlots.Add(new ItemInfo()
            {
                itemName = slots[i].itemName,
                itemSo = slots[i].itemSo,
                itemQuantity = slots[i].itemQuantity,
                itemSprite = slots[i].itemSprite,
                itemDescription = slots[i].itemDescription,
            });
        }
        if(sortedSlots.Count == 0) return;
        
        num = slots.Length;
        for (int i = 0; i < num; i++)
        {
            slots[i].EmptySlot();
        }
        
        sortedSlots = ItemSortByType(sortedSlots);
        num = sortedSlots.Count;
        for (int i = 0; i < num; i++)
        {
            ItemInfo itemSlot = sortedSlots[i];
            slots[i].AddItem(itemSlot.itemName, itemSlot.itemSo, itemSlot.itemQuantity, itemSlot.itemSprite,
                itemSlot.itemDescription);
        }
    }
    private List<ItemInfo> ItemSortByType(List<ItemInfo> sortedSlots)
    {
        List<ItemInfo> resultSortedList = new List<ItemInfo>(sortedSlots);
        
        switch (currentSortType)
        {
            case ItemSortType.Name:
                resultSortedList =  resultSortedList.OrderBy(t => t.itemName).ToList();
                break;
            case ItemSortType.Type:
                resultSortedList = resultSortedList.OrderBy(t => t.itemSo.itemType).ToList();
                break;
            case ItemSortType.Quantity:
                resultSortedList = resultSortedList.OrderBy(t => t.itemQuantity).ToList();
                break;
            case ItemSortType.Rarity:
                resultSortedList = resultSortedList.OrderBy(t => t.itemSo.rarity).ToList();
                break;
            default: return null;
        }

        if (currentSortOrder == SortOrder.Descending)
        {
            resultSortedList.Reverse();
        }
        
        return resultSortedList;
    }
}
