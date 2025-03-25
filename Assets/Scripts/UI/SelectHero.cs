using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// </summary>
public class SelectHero : MonoBehaviour
{
    public Transform alternativeArea;
    public GameObject heroPrefab;


    public void selectHero(Image heroImage)
    {
        GameObject hero = Instantiate(heroPrefab, alternativeArea);
        CreateHero createHero = hero.GetComponent<CreateHero>();
        createHero.UpdateHeroImage(heroImage);
    }

   
}
