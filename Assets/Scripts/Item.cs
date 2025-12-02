using ScriptableObjects;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public int quantity;
    public Sprite sprite;
    public ItemScriptableObject itemSo;
    [TextArea]
    public string description;
    
    private InventoryManager inventoryManager;
    void Start()
    {
        inventoryManager =  GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            int extraItems = inventoryManager.AddItem(itemName, itemSo, quantity, sprite, description);
            
            if(extraItems > 0)
            {
                quantity = extraItems;
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
    }
}
