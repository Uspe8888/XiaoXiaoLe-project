using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MonsterAttack : MonoBehaviour
{
    public float dashDistance = 0.3f; // 突进的距离
    public float dashSpeed = 5f; // 突进速度
    public float dashInterval = 0.5f; // 突进间隔
    public int attackDamage = 2; // 攻击伤害

    private bool isAttacking; // 防止重复攻击


    


    public void PerformAttack(Transform target)
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackCoroutine(target));
        }
    }

   


    private IEnumerator AttackCoroutine(Transform target)
    {
        isAttacking = true;

        // 记录起始位置和目标位置
        Vector2 startPosition = transform.position;
        Vector2 direction = (target.position - transform.position).normalized;
        Vector2 dashTarget = (Vector2)transform.position + direction * dashDistance;

        // 1. 突进到目标点
        yield return StartCoroutine(Dash(startPosition, dashTarget)); 

        // 2. 返回起始位置
        yield return StartCoroutine(Dash(dashTarget, startPosition));

        // 3. 造成伤害
        Health health = target.GetComponent<Health>();
        if (health)
        {
            health.TakeDamage(attackDamage); // 调用目标受伤方法
        }

        // 4. 等待攻击间隔
        yield return new WaitForSeconds(dashInterval);

        isAttacking = false;
    }

    private IEnumerator Dash(Vector2 from, Vector2 to)// 实现突进效果
    {
        float elapsedTime = 0f;
        while (elapsedTime < dashDistance / dashSpeed)
        {
            transform.position = Vector2.Lerp(from, to, elapsedTime * dashSpeed / dashDistance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = to; // 确保到达目标点
    }
}