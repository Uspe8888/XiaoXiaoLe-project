using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ClearablePiece : MonoBehaviour
{
    // 动画片段
    public AnimationClip clearAnimation;
    // 是否正在被清除
    private bool isBeingCleared = false;

    // 获取是否正在被清除的状态
    public bool IsBeingCleared { get => isBeingCleared; }

    // 游戏棋子组件
    protected GamePiece piece;

    // 初始化时获取游戏棋子组件
    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

    // 清除棋子
    // ReSharper disable Unity.PerformanceAnalysis
    public virtual void Clear()// 这个方法是虚方法，子类可以重写
    {

        piece.GridRef.level.OnPieceCleared(piece);
        isBeingCleared = true;
        StartCoroutine(ClearCorountine());
    }

    // 清除棋子的协程
    private IEnumerator ClearCorountine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(clearAnimation.name);

            yield return new WaitForSeconds(clearAnimation.length);

            Destroy(gameObject);
        }
    }
}
