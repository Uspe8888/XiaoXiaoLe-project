using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class ClearBoomPiece : ClearablePiece
{
    public override void Clear() // 重写清除方法
    {
        base.Clear(); // 调用基类的清除方法
        piece.GridRef.BoomClear(piece.X, piece.Y);
    }

}
