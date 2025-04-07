using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class HeroHome : MonoBehaviour
{
    ColorPiece colorPiece;

    [System.Serializable]
    public struct Heros
    {
        public ColorPiece.ColorType heroColor;
        public GameObject heroPrefab;
    }
    public List<Heros> heros;
    public float spawnRange = 1; //生成范围
    
    public GameObject waitingArea;
    public GameObject heroPrefab;
    

    public void SpawnSixHero()
    {
        //todo：这时列表里还没有数据，childImages.Count是0，所以英雄生成不出来，因为是在Awake里，生命周期有问题
        List<Image> childImages = new List<Image>();
        foreach (Transform child in waitingArea.transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null && img.sprite != null)
            {
                childImages.Add(img);
            }
        }
        
        
        for (int i = 0; i < childImages.Count; i++)
        {
            spawnHero(i,childImages[i].sprite);
           
        }
    }

   
    public void spawnHero(ColorPiece.ColorType color)
    {
        for (int i = 0; i < heros.Count; i++)
        {
            if (heros[i].heroColor == color)
            {
                GameObject hero = Instantiate(heros[i].heroPrefab,
                    transform.position + Random.onUnitSphere * spawnRange, Quaternion.identity);
                hero.transform.parent = transform;
                hero.transform.position = new Vector3(hero.transform.position.x, hero.transform.position.y,
                    transform.position.z);
            }
        }
    }

    // public void spawnHero(int heroIndex)
    // {
    //     GameObject hero = Instantiate(heros[heroIndex].heroPrefab,transform.position+Random.onUnitSphere*spawnRange,Quaternion.identity);
    //     hero.transform.parent = transform;
    //     hero.transform.position = new Vector3(hero.transform.position.x, hero.transform.position.y, transform.position.z);
    // }

    public void spawnHero(int heroIndex)
    {
        // 计算生成位置的偏移量
        float offset = (heroIndex - 2.5f) * spawnRange; // 2.5f 是为了将第一个英雄放在左侧中间位置
        Vector3 spawnPosition = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);

        // 实例化英雄对象
        GameObject hero = Instantiate(heros[heroIndex].heroPrefab, spawnPosition, Quaternion.identity);

        // 设置生成的英雄对象为子对象
        hero.transform.parent = transform;

        // 调整生成英雄对象的位置（这里已经通过spawnPosition设置，可以省略）
        // hero.transform.position = new Vector3(hero.transform.position.x, hero.transform.position.y, transform.position.z);
    }
    public void spawnHero(int heroIndex,Sprite sprite)
    {
        // 计算生成位置的偏移量
        float offset = (heroIndex - 2.5f) * spawnRange; // 2.5f 是为了将第一个英雄放在左侧中间位置
        Vector3 spawnPosition = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);

        // 实例化英雄对象
        GameObject hero = Instantiate(heroPrefab, spawnPosition, Quaternion.identity);
        SpriteRenderer heroSpriteRenderer = hero.GetComponentInChildren<SpriteRenderer>();
        if (heroSpriteRenderer != null)
        {
            heroSpriteRenderer.sprite = sprite; // 设置精灵
        }
        else
        {
            Debug.LogError("没有找到SpriteRenderer组件");
        }
        
        // 设置生成的英雄对象为子对象
        hero.transform.parent = transform;
        
    }
    
    
}