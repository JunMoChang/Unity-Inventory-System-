using System;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [NonSerialized] public string itemName;
    [NonSerialized] public int itemQuantity;
    [NonSerialized] public bool isFull;
    [NonSerialized] public ItemScriptableObject itemSo;
    public Sprite emptySprite;
    public Sprite defaultItemSprite;
    [NonSerialized] public Sprite itemSprite;
    [NonSerialized] public string itemDescription;
    
    [SerializeField] public int itemMaxQuantity;
    
    [SerializeField] private TMP_Text itemSlotQuantityText;
    [SerializeField] private Image itemSlotImage;
    
    [SerializeField] private TMP_Text itemDescriptionNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private Image itemDescriptionImage;

    public GameObject selectedPanel;
    [NonSerialized]public bool isSelected;
    
    private InventoryManager inventoryManager;
    
    [NonSerialized] private bool isReferenceSlot;
    [NonSerialized] private ItemSlot sourceSlot;
    [NonSerialized] private readonly System.Collections.Generic.List<ItemSlot> referenceSlots = new ();
    //[NonSerialized] private ItemSlot referenceSlot;
    void Awake()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }
    void Start()
    {
        itemDescriptionImage.sprite = emptySprite;
    }

    public int AddItem(string targetItemName, ItemScriptableObject targetItemSo, int targetQuantity, Sprite targetSprite, string targetDescription)
    {
        if (isFull) return targetQuantity;
        
        bool wasEmpty = itemQuantity == 0;
            
        itemName = targetItemName;
        itemSo = targetItemSo;
        itemDescription = targetDescription;
        itemSprite = targetSprite;
        itemSlotImage.sprite = itemSprite;
        
        itemQuantity += targetQuantity;
        int extraItems = 0;
        if (itemQuantity >= itemMaxQuantity)
        {
            itemSlotQuantityText.text = itemMaxQuantity.ToString();
            itemSlotQuantityText.enabled = true;
            isFull = true;
            
            extraItems = itemQuantity - itemMaxQuantity;
            itemQuantity =  itemMaxQuantity;
        }
        
        itemSlotQuantityText.text = itemQuantity.ToString();
        itemSlotQuantityText.enabled = true;

        if (wasEmpty) inventoryManager.CreateReferenceInAllContainer(this);
        
        SyncToReferenceSlots();
        
        return extraItems;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }
    
    private void OnLeftClick()
    {
        if (isReferenceSlot && sourceSlot != null)
        {
            sourceSlot.HandelLeftClick();
            /*sourceSlot.OnPointerClick(new PointerEventData(EventSystem.current) 
            { 
                button = PointerEventData.InputButton.Left 
            });*/
            return;
        }
        HandelLeftClick();
    }
    private void OnRightClick()
    {
        if (!isSelected || itemQuantity == 0) return;
        
        if (isReferenceSlot && sourceSlot != null)
        {
            sourceSlot.HandelRightClick();
            /*sourceSlot.OnPointerClick(new PointerEventData(EventSystem.current)
            { 
                button = PointerEventData.InputButton.Right 
            });*/
            
            return;
        }
        HandelRightClick();
    }

    private void HandelLeftClick()
    {
        if (isSelected)
        {
            if(itemSo == null) return;
            
            bool usable = inventoryManager.UseItem(itemSo);
            if (!usable) return;
            
            itemQuantity -= 1;
            if (itemQuantity <= 0)
            {
                EmptySlot();
                return;
            }
            
            itemSlotQuantityText.text = itemQuantity.ToString();
            itemSlotQuantityText.enabled = true;
            isFull = false;

            SyncToReferenceSlots();
            return;
        }
        
        if(itemSo == null) return;
        
        inventoryManager.SetCurrentSelectedSlot(this);
        isSelected = true;
        selectedPanel.SetActive(isSelected);
        
        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
        if(itemDescriptionImage.sprite == null) itemDescriptionImage.sprite = emptySprite;
        
        SyncToReferenceSlots();
    }
    private void HandelRightClick()
    {
        GameObject item = new GameObject(itemName);
        Item newItemScr = item.AddComponent<Item>();
        newItemScr.itemSo = itemSo;
        newItemScr.itemName = itemName;
        newItemScr.quantity = 1;
        newItemScr.sprite = itemSprite;
        newItemScr.description =  itemDescription;
        
        SpriteRenderer spriteRenderer = item.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemSprite;

        item.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = item.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        
        item.transform.position = GameObject.Find("Player").transform.position + new Vector3(1f, 0, 0);
        
        itemQuantity -= 1;
        if (itemQuantity <= 0)
        {
            EmptySlot();
            return;
        }
        itemSlotQuantityText.text = itemQuantity.ToString();
        itemSlotQuantityText.enabled = true;
        isFull = false;
        
        SyncToReferenceSlots();
    }
    public void EmptySlot()
    {
        ClearDisplay();
        ClearReferenceRelationships();
    }
    private void ClearDisplay()
    {
        isFull = false;
        itemQuantity = 0;
        itemName = null;
        itemSo = null;
        itemDescription = null;
        itemSprite = null;
        
        itemSlotImage.sprite = defaultItemSprite;
        itemSlotQuantityText.enabled = false;
        
        itemDescriptionImage.sprite = emptySprite;
        itemDescriptionText.text = "";
        itemDescriptionNameText.text = "";
    }
    
    private void ClearReferenceRelationships()
    {
        if (referenceSlots.Count > 0)
        {
            var copy = new System.Collections.Generic.List<ItemSlot>(referenceSlots);
            referenceSlots.Clear();
            
            int num = copy.Count;
            for (int i = 0; i < num; i++)
            {
                if (copy[i] != null)
                {
                    copy[i].sourceSlot =  null;
                    copy[i].isReferenceSlot = false;
                    copy[i].ClearDisplay();
                }
            }
        }
        
        if (isReferenceSlot && sourceSlot != null)
        {
            sourceSlot.referenceSlots.Remove(this);
            sourceSlot = null;
            isReferenceSlot = false;
        }
    }
    
    private void SyncToReferenceSlots()
    {
        var referenceSlotsCopy = new System.Collections.Generic.List<ItemSlot>(referenceSlots);

        foreach (ItemSlot refSlot in referenceSlotsCopy)
        {
            if (refSlot == null) continue;
            
            refSlot.SyncFromSourceSlot(this);
        }
    }
    
    private void SyncFromSourceSlot(ItemSlot targetSlot)
    {
        if (targetSlot.itemQuantity <= 0)
        {
            EmptySlot();
        }
        else
        {
            itemName = targetSlot.itemName;
            itemSo = targetSlot.itemSo;
            itemDescription = targetSlot.itemDescription;
            itemSprite = targetSlot.itemSprite;
            itemQuantity = targetSlot.itemQuantity;
            isFull = targetSlot.isFull;
            isSelected =  targetSlot.isSelected;
            
            selectedPanel.SetActive(isSelected);
            itemSlotImage.sprite = itemSprite;
            itemSlotQuantityText.text = itemQuantity.ToString();
            itemSlotQuantityText.enabled = true;
        }
    }
    
    public void SetAsReferenceSlot(ItemSlot targetSlot)
    {
        isReferenceSlot = true;
        sourceSlot = targetSlot;
        
        if (!targetSlot.referenceSlots.Contains(this))
        {
            targetSlot.referenceSlots.Add(this);
        }
        
        SyncFromSourceSlot(targetSlot);
    }
    
    public void SetDeselected()
    {
        isSelected = false;
        selectedPanel.SetActive(false);
        
        SyncToReferenceSlots();
    }
}
