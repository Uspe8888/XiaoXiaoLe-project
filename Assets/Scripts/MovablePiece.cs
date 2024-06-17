using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class MovablePiece : MonoBehaviour
{
    private GamePiece piece;
    private IEnumerator moveCoroutine;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

    // 以指定时间移动游戏棋子至新的坐标位置
    public void Move(int newX, int newY, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    // 使用协程平滑移动游戏棋子至新的坐标位置
    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        piece.X = newX;
        piece.Y = newY;

        Vector3 startPos = transform.position;
        Vector3 endPos = piece.GridRef.GetWorldPosition(newX, newY);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            // 使用Lerp函数平滑计算游戏棋子移动的位置
            piece.transform.position = Vector3.Lerp(startPos, endPos, time);
            yield return 0;
        }
        piece.transform.position = endPos;
    }
}

