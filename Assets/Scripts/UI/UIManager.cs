using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class UIManager : MonoBehaviour
{
    public GameObject inventory;

    public void CloseInventory()
    {
        inventory.SetActive(false);
    }
}
