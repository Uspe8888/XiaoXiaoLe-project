
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Hero : MonoBehaviour
{
    public int heroHp = 10;
    private int currentHp = 10;

    public float moveSpeed = 1f; // 移动速度
    private float attackRange=2f;
   
    private MonsterAttack monsterAttack; // 引用攻击脚本
    public Transform monster; // 当前目标
    public Transform target;
    

    private void Start()
    {
        currentHp = heroHp;
        monster = GameObject.FindGameObjectWithTag("Monster").transform;
        
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
           // ChooseRandomTarget(); // 重新选择目标
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
        int childCount = monster.childCount;
        
        if (childCount > 0)
        {
            // 随机选择一个子物体作为目标
            int randomIndex = Random.Range(0, childCount);
            target = monster.GetChild(randomIndex);
        }
        else
        {
            target = null; // 如果没有子物体，目标设为null
        }
    }

    
    
    public void GetHurt(int damage)
    {
        currentHp -= damage;
        Debug.Log("当前血量"+currentHp);
    }
}