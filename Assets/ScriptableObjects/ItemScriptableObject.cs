
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
    public class ItemScriptableObject : ScriptableObject
    {
        public string itemName;
        public ItemType itemType;
        public ItemRarity rarity; 
        public BaseState baseState;
        public int amountRecover;
    
        public Buff buff;
        public int amountBuff;
    
        public enum ItemType
        {
            All,//全部
            Weapon,//武器
            Consumable,//消耗品
            Material,//材料
            Other//其他
        }
        public enum BaseState
        {
            None,
            Health
        }

        public enum Buff
        {
            None,
            MaxHealth,
            Speed,
            Damage,
        }

        public bool UseItem()
        {
            if (baseState == BaseState.Health)
            {
                Debug.Log($"血量加{amountRecover}");
                return true;
            }
            return false;
        }
    }
}
