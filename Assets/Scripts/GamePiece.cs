using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class GamePiece : MonoBehaviour
{

    private int x;
    private int y;
    private CustomGrid.PieceType type;
    private CustomGrid grid;

    public int X
    {
        get => x;
        set { if (IsMovable()) { x = value; } }
    }
    public int Y
    {
        get => y;
        set { if (IsMovable()) { y = value; } }
    }
    public CustomGrid.PieceType Type { get => type; }
    public CustomGrid GridRef { get => grid; }

    private MovablePiece movableComponent;
    public MovablePiece MovableComponent { get => movableComponent; }

    private ColorPiece colorComponent;
    public ColorPiece ColorComponent { get => colorComponent; }

    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent { get => clearableComponent; }


    private void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        colorComponent = GetComponent<ColorPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
    }

    public void Init(int _x, int _y, CustomGrid grid, CustomGrid.PieceType type)
    {
        x = _x;
        y = _y;
        this.grid = grid;
        this.type = type;
    }

    void OnMouseEnter()
    {
        grid.EnterPiece(this);
    }

    void OnMouseDown()
    {
        grid.PressPiece(this);
    }

    void OnMouseUp()
    {
        grid.ReleasePiece();
    }



    public bool IsMovable()
    {
        return movableComponent != null;
    }
    public bool IsColored()
    {
        return colorComponent != null;
    }
    public bool IsClearable()
    {
        return clearableComponent != null;
    }

}
