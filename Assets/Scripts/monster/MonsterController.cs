using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public Transform herohome; // herohome物体
    public float moveSpeed = 1f; // 移动速度
    public float attackRange = 3f; // 攻击范围

    public MonsterAttack monsterAttack; // 引用攻击脚本
    public Transform target; // 当前目标

    private void Start()
    {
        
        
        monsterAttack = GetComponent<MonsterAttack>();
        ChooseRandomTarget();
    }

    private void Update()
    {
        if (!target) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            MoveTowardsTarget(); // 移动逻辑
        }
        else
        {
            monsterAttack.PerformAttack(target); // 触发攻击逻辑
            //ChooseRandomTarget(); // 重新选择目标
        }

        UpdateFacingDirection(); // 更新朝向
    }

    private void MoveTowardsTarget()// 移动逻辑
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    private void UpdateFacingDirection()// 更新朝向
    {
        float targetScaleX = Mathf.Sign(target.position.x - transform.position.x);
        transform.localScale = new Vector3(targetScaleX, 1, 1);
    }
    
    private void ChooseRandomTarget()// 选择目标
    {
        // 获取herohome的所有子物体
        int childCount = herohome.childCount;
        
        if (childCount > 0)
        {
            // 随机选择一个子物体作为目标
            int randomIndex = Random.Range(0, childCount);
            target = herohome.GetChild(randomIndex);
        }
        else
        {
            target = null; // 如果没有子物体，目标设为null
        }
    }
    
    
}