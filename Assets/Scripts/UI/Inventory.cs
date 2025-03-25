using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Inventory : MonoBehaviour
{
    public ItemData[] items; // 数组存储所有的ScriptableObject

    public Transform gridParent; // 背包UI的Grid容器
    public GameObject itemSlotPrefab; // 用于显示图标的格子Prefab

    void Start()
    {
        DisplayItems(); // 打开背包时显示物品
    }

    public void DisplayItems()
    {
        foreach (ItemData item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, gridParent);
            InventorySlot itemSlot = slot.GetComponent<InventorySlot>();
            itemSlot.UpdateSlot(item);
        }
    }

}
