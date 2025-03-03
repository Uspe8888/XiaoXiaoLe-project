using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public abstract class CharacterController : MonoBehaviour
{
    public float moveSpeed = 1f; // 移动速度
    public float attackRange = 2f; // 攻击范围
    public Transform target; // 当前目标
    protected MonsterAttack attackComponent; // 攻击组件
    private CharacterController targetCharacter;
    private Health targetHealth;
  
    
    protected virtual void Start()
    {
        attackComponent = GetComponent<MonsterAttack>();
        InitializeTarget();
        targetHealth=target.GetComponent<Health>();
     }

    private void Update()
    {
       
       if (!target)
       {
           InitializeTarget();
       }

       float distanceToTarget = Vector3.Distance(transform.position, target.position);

       if (distanceToTarget > attackRange)
       {
           MoveTowardsTarget();
       }
       else
       {
           PerformAttack();
       }

       UpdateFacingDirection();
    }

    protected abstract void InitializeTarget(); // 初始化目标

    protected virtual void MoveTowardsTarget() // 移动到目标
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    protected virtual void UpdateFacingDirection() // 更新朝向
    {
        float targetScaleX = Mathf.Sign(target.position.x - transform.position.x);
        transform.localScale = new Vector3(targetScaleX, 1, 1);
    }

    protected virtual void PerformAttack() // 攻击
    {
        attackComponent?.PerformAttack(target);
    }
}