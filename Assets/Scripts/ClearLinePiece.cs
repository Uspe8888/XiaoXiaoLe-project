using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class ClearLinePiece : ClearablePiece // 清除行或列的方块类，继承自ClearablePiece
{
    public bool isRow; // 是否是行

    public override void Clear() // 重写清除方法
    {
        base.Clear(); // 调用基类的清除方法
        if (isRow) // 如果是行
        {
            piece.GridRef.ClearRow(piece.Y); // 清除整行
        }
        else // 如果是列
        {
            piece.GridRef.ClearColumn(piece.X); // 清除整列
        }
    }
}

