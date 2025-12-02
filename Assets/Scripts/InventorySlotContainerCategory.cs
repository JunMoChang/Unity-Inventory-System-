using UnityEngine;
using UnityEngine.UI;

public class InventorySlotContainerCategory : MonoBehaviour
{
    private InventoryManager inventoryManager;
    
    [SerializeField]private ScrollRect scrollRect;
    private InventorySlotsContainer currentSlotsContainer;
    private InventorySlotsContainer lastSlotsContainer;
    public Button[] categoryButtons;
    
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        int num = categoryButtons.Length;
        for (int i = 0; i < num; i++)
        {
            int currentIndex = i;
           
            categoryButtons[i].onClick.AddListener(() => CategoryButtonClick(inventoryManager.inventorySlotsContainers[currentIndex]));
        }
        currentSlotsContainer = inventoryManager.inventorySlotsContainers[0];
        lastSlotsContainer = currentSlotsContainer;
        
        CategoryButtonClick(currentSlotsContainer);
    }
    
    private void CategoryButtonClick(InventorySlotsContainer inventorySlotContainer)
    {
        if(inventorySlotContainer.gameObject.activeSelf) return;
        currentSlotsContainer = inventorySlotContainer;
        scrollRect.content = inventorySlotContainer.gameObject.GetComponent<RectTransform>();
        inventorySlotContainer.gameObject.SetActive(true);
        
        lastSlotsContainer.DeSelectedAllItems();
        lastSlotsContainer = inventorySlotContainer.GetComponent<InventorySlotsContainer>();
        
        int num = inventoryManager.inventorySlotsContainers.Length;
        for (int i = 0; i < num; i++)
        {
            GameObject gObj = inventoryManager.inventorySlotsContainers[i].gameObject;
            if(inventorySlotContainer.gameObject == gObj)  continue;
            
            gObj.SetActive(false);
        }
    }

    public InventorySlotsContainer GetCurrentSlotContainer()
    {
        return currentSlotsContainer;
    }
}
