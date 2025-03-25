using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class HeroTest : CharacterController
{
  
    private Transform monsterHome;
    
    protected override void InitializeTarget()
    {
        monsterHome = GameObject.FindGameObjectWithTag("Monster")?.transform;
        ChooseRandomTarget();
    }

    private void ChooseRandomTarget()
    {
        if (monsterHome.childCount > 0)
        {
            int randomIndex = Random.Range(0, monsterHome.childCount);
            target = monsterHome.GetChild(randomIndex);
        }
        else
        {
            target = null;
        }
    }
    
}
