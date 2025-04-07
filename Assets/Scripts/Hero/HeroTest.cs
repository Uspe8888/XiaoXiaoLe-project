using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HeroTest 类继承自 RoleController，主要功能是初始化目标并选择一个随机目标。
/// </summary>
public class HeroTest : RoleController
{
    /// <summary>
    /// 用于存储怪物的家位置的 Transform 组件。
    /// </summary>
    private Transform monsterHome;
    
    /// <summary>
    /// 重写的保护方法，用于初始化目标。
    /// 查找带有 "Monster" 标签的游戏对象，并获取其 Transform 组件。
    /// 如果找到了怪物家位置，则调用 ChooseRandomTarget 方法选择一个随机目标。
    /// </summary>
    protected override void InitializeTarget()
    {
        monsterHome = GameObject.FindGameObjectWithTag("Monster")?.transform;
        ChooseRandomTarget();
    }

    /// <summary>
    /// 从怪物家位置的子对象中随机选择一个目标。
    /// 如果 monsterHome 有子对象，则生成一个随机索引，并将目标设置为该索引对应的子对象；
    /// 如果没有子对象，则将目标设置为 null。
    /// </summary>
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