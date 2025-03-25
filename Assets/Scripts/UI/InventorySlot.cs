using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// </summary>
public class InventorySlot : MonoBehaviour
{
    public Image iconImage; // 绑定格子中的Image组件
    public Image emptySlotIcon;
    public SelectHero selectHero;

    // 更新格子UI
    private void Start()
    {
        selectHero = GameObject.FindGameObjectWithTag("select").GetComponent<SelectHero>();
    }

    public void UpdateSlot(ItemData item)
    {
        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true; // 显示道具图标
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false; // 隐藏道具图标
        }

        // 空格子图标始终显示
        emptySlotIcon.enabled = true;
    }
    
    public void ChoiceHero()
    {
        selectHero.selectHero(iconImage);
        Destroy(gameObject);
        
    }
    
    
}