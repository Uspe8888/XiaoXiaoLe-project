using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class LevelMoves : Level
{
    public int numMoves;//目标步数
    public int targetScore;//目标分数
    private int movesUsed = 0;//已用步数

    private void Start()
    {
        type = LevelType.MOVES;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(targetScore);
        hud.SetRemaining(numMoves);

        Debug.Log("Number of moves:" + numMoves + "Target score:" + targetScore);
    }
    public override void OnMove()
    {
        movesUsed++;

        hud.SetRemaining(numMoves - movesUsed);
        Debug.Log("Moves remaining:" + (numMoves - movesUsed));

        if(numMoves-movesUsed==0)
        {
            if(currentScore>=targetScore)
            {
                GameWin();
            }
            else
            {
                GameLose();
            }
        }


    }

}
