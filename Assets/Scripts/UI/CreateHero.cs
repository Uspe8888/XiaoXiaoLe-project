using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class CreateHero : MonoBehaviour
{
    public Image image;


    public void UpdateHeroImage(Image image)
    {
       
        if (image != null)
        {
            this.image.sprite = image.sprite;
            this.image.enabled = true;
        }
        else
        {
            this.image.sprite = null;
            this.image.enabled = false;
        }
    }
}