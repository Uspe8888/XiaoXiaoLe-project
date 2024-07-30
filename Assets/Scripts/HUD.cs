using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class HUD : MonoBehaviour
{
    public Level level;

    public GameOver gameOver;

    public TextMeshProUGUI remainingText;
    public TextMeshProUGUI remainingSubText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI targetSubText;
    public TextMeshProUGUI scoreText;
    public UnityEngine.UI.Image[] stars;



    private int starIdx = 0;


    private void Start()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i == starIdx)
            {
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled = false;
            }
        }
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();

        int visibleStar = 0;

        if (score >= level.score1Star && score < level.score2Star) { visibleStar = 1; }
        else if (score >= level.score2Star && score < level.score3Star) { visibleStar = 2; }
        else if (score >= level.score3Star) { visibleStar = 3; }

        for (int i = 0; i < stars.Length; i++)
        {
            if (i == visibleStar) { stars[i].enabled = true; }
            else { stars[i].enabled = false; }
        }
        starIdx = visibleStar;
    }

    public void SetTarget(int target)
    {
        targetText.text = target.ToString();
    }

    public void SetRemaining(int remaining)
    {
        remainingText.text = remaining.ToString();
    }

    public void SetRemaining(string remaining)
    {
        remainingText.text = remaining;
    }

    public void SetLevelType(Level.LevelType type)
    {
        if (type == Level.LevelType.MOVES)
        {
            remainingSubText.text = "剩余步数";
            targetSubText.text = "目标分数";
        }
        else if (type == Level.LevelType.OBSTACLE)
        {
            remainingSubText.text = "剩余步数";
            targetSubText.text = "剩余桶子";
        }
        else if(type==Level.LevelType.TIMER)
        {
            remainingSubText.text = "剩余时间";
            targetSubText.text = "目标分数";
        }
    }

    public void OnGameWin(int score)
    {
        gameOver.ShowWin(score, starIdx);     
    }
    public void OnGameLose(int score)
    {
        gameOver.ShowLose();
    }
}
