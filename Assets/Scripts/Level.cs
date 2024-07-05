using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Level : MonoBehaviour
{
    public enum LevelType
    {
        TIMER,//时间关卡
        OBSTACLE,//障碍关卡
        MOVES,//步数关卡
    };

    public CustomGrid grid;

    public int score1Star;
    public int score2Star;
    public int score3Star;

    protected LevelType type;
    public LevelType Type { get => type; }

    protected int currentScore;//当前分数

    public virtual void GameWin()
    {
        Debug.Log("You Win");
        grid.GameOver();
    }

    public virtual void GameLose()
    {
        Debug.Log("You Lose");
        grid.GameOver();
    }
    public virtual void OnMove()
    {
       
    }
    public virtual void OnPieceCleared(GamePiece piece)
    {
        currentScore += piece.score;
        Debug.Log("Current Score: " + currentScore);
    }



}
