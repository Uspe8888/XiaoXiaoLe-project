using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class GamePiece : MonoBehaviour
{
    // 游戏棋子的x坐标
    private int x;
    // 游戏棋子的y坐标
    private int y;
    // 游戏棋子的类型
    private CustomGrid.PieceType type;
    // 游戏棋子所在的网格
    private CustomGrid grid;

    // x坐标的属性
    public int X
    {
        get => x;
        set { if (IsMovable()) { x = value; } }
    }
    // y坐标的属性
    public int Y
    {
        get => y;
        set { if (IsMovable()) { y = value; } }
    }
    // 游戏棋子类型的属性
    public CustomGrid.PieceType Type { get => type; }
    // 游戏棋子所在网格的引用
    public CustomGrid GridRef { get => grid; }

    // 可移动组件
    private MovablePiece movableComponent;
    public MovablePiece MovableComponent { get => movableComponent; }

    // 颜色组件
    private ColorPiece colorComponent;
    public ColorPiece ColorComponent { get => colorComponent; }

    // 可清除组件
    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent { get => clearableComponent; }

    // 初始化组件
    private void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        colorComponent = GetComponent<ColorPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
    }

    // 初始化游戏棋子
    public void Init(int _x, int _y, CustomGrid grid, CustomGrid.PieceType type)
    {
        x = _x;
        y = _y;
        this.grid = grid;
        this.type = type;
    }

    // 鼠标进入棋子区域
    void OnMouseEnter()
    {
        grid.EnterPiece(this);
    }

    // 鼠标按下棋子
    void OnMouseDown()
    {
        grid.PressPiece(this);
    }

    // 鼠标释放棋子
    void OnMouseUp()
    {
        grid.ReleasePiece();
    }

    // 检查棋子是否可移动
    public bool IsMovable()
    {
        return movableComponent != null;
    }
    // 检查棋子是否有颜色
    public bool IsColored()
    {
        return colorComponent != null;
    }
    // 检查棋子是否可清除
    public bool IsClearable()
    {
        return clearableComponent != null;
    }


}
