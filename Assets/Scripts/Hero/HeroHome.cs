using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// 
/// </summary>
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

    public float spawnRange=1;//生成范围


    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            spawnHero(i);
        }
    }


    public void spawnHero(ColorPiece.ColorType color)
    {
        for(int i = 0; i < heros.Count; i++)
        {
            if (heros[i].heroColor == color)
            {
                GameObject hero = Instantiate(heros[i].heroPrefab,transform.position+Random.onUnitSphere*spawnRange,Quaternion.identity);
                hero.transform.parent = transform;
                hero.transform.position = new Vector3(hero.transform.position.x, hero.transform.position.y, transform.position.z);
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



}
