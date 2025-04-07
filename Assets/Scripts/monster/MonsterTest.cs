using UnityEngine;


/// <summary>
/// 
/// </summary>
public class MonsterTest : RoleController
{
    public Transform herohome;
    
    protected override void InitializeTarget()
    {
        ChooseRandomTarget();
    }


    private void ChooseRandomTarget()
    {
        if (herohome.childCount > 0)
        {
            int randomIndex = Random.Range(0, herohome.childCount);
            target = herohome.GetChild(randomIndex);
        }
        else
        {
            target = null;
        }
    }

}
