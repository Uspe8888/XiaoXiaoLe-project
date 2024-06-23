using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomGrid : MonoBehaviour
{
    // 定义游戏块的类型枚举
    public enum PieceType
    {
        EMPTY,
        BARREL,
        NORMAL, // 普通类型
        COUNT,  // 类型数量
    }

    // 使结构体可以在Unity编辑器中序列化显示
    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type; // 游戏块类型
        public GameObject prefab; // 游戏块预制体
    }

    public int xDim; // 网格的X维度
    public int yDim; // 网格的Y维度

    public float fillTime;

    public PiecePrefab[] piecePrefabs; // 游戏块预制体数组

    public GameObject backgroundPrefab; // 背景预制体

    private Dictionary<PieceType, GameObject> piecePrefabDict; // 用于存储游戏块类型和对应预制体的字典
    private GamePiece[,] pieces; // 用于存储游戏块二维数组
    private bool inverse = false;

    private GamePiece pressedPiece; // 按下时的游戏块
    private GamePiece enteredPiece;

    private void Start() // Unity生命周期函数，当脚本实例被初始化时调用
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>(); // 初始化字典

        // 遍历游戏块预制体数组，将预制体添加到字典中
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type)) // 如果字典中不存在当前类型
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab); // 添加到字典
            }
        }

        // 遍历网格的X和Y维度，生成背景
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                // 实例化背景预制体，设置位置和旋转，并设置父对象为当前游戏对象
                GameObject background = Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        pieces = new GamePiece[xDim, yDim]; // 初始化游戏块二维数组
        // 遍历网格的X和Y维度，生成游戏块
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                SpawnNewPiece(x, y, PieceType.EMPTY);
            }
        }

        // 特殊处理某些位置的游戏块
        Destroy(pieces[4, 4].gameObject);
        SpawnNewPiece(4, 4, PieceType.BARREL);
        Destroy(pieces[1, 4].gameObject);
        SpawnNewPiece(1, 4, PieceType.BARREL);
        Destroy(pieces[6, 4].gameObject);
        SpawnNewPiece(6, 4, PieceType.BARREL);

        StartCoroutine(Fill()); // 开始填充网格的协程
    }

    public IEnumerator Fill()
    {
        bool needsRefill = true;
        while (needsRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }
            needsRefill = ClearAllValidMatches();
        }
    }

    public bool FillStep()
    {
        bool movedPiece = false;
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }

                GamePiece piece = pieces[x, y];
                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];
                    if (pieceBelow.Type == PieceType.EMPTY)
                    {

                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else // 如果下方块不为空，检查旁边是否有空位
                    {
                        for (int diag = -1; diag <= 1; diag++) // 检查左右两侧
                        {
                            if (diag != 0) // 不检查当前列
                            {
                                int diagX = x + diag;
                                if (inverse) // 如果inverse为true，则反向计算列
                                {
                                    diagX = x - diag;
                                }
                                if (diagX >= 0 && diagX < xDim) // 确保列索引有效
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1]; // 获取旁边块
                                    if (diagonalPiece.Type == PieceType.EMPTY) // 如果旁边块为空
                                    {
                                        bool hasPieceAbove = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--) // 检查从当前行到顶部是否有不可移动的块
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            if (pieceAbove.IsMovable()) // 如果遇到可移动的块，跳出循环
                                            {
                                                break;
                                            }
                                            else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY) // 如果遇到不可移动的非空块，标记为false
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }
                                        if (!hasPieceAbove) // 如果上方没有不可移动的块
                                        {
                                            // 移动当前块到旁边空位，并生成新的空块
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break; // 找到空位后跳出循环
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        // 处理第一行的空位
        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBelow = pieces[x, 0]; // 获取第一行的块
            if (pieceBelow.Type == PieceType.EMPTY) // 如果第一行的块为空
            {
                // 在该位置生成新的块，并向下移动
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }
        }

        return movedPiece; // 如果有任何块移动，返回true
    }





    // 获取游戏世界中特定网格位置的方法
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(
            transform.position.x - xDim / 2.0f + x,
            transform.position.y + yDim / 2.0f - y
        );
    }

    // 实例化新的游戏块并放置到网格中的方法
    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        GamePiece pieceComponent = newPiece.GetComponent<GamePiece>();
        pieces[x, y] = pieceComponent;
        pieceComponent.Init(x, y, this, type);

        return pieceComponent;
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
            || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (piece1.IsMovable() && piece2.IsMovable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

                ClearAllValidMatches();
                StartCoroutine(Fill());
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }

    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }

    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }

    public void ReleasePiece()
    {
        if (IsAdjacent(pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColored())
        {
            ColorPiece.ColorType color = piece.ColorComponent.Color;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //获取水平方向的相同颜色块
            horizontalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0) { x = newX - xOffset; }//左
                    else { x = newX + xOffset; }//右
                    if (x < 0 || x >= xDim) { break; }
                    if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else { break; }
                }
            }
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                { matchingPieces.Add(horizontalPieces[i]); }
            }
            //如果找到匹配，垂直遍历L型和T型
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0) { y = newY - yOffset; }//上
                            else { y = newY + yOffset; }//下
                            if (y < 0 || y >= yDim) { break; }
                            if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else { break; }
                        }
                    }
                    if (verticalPieces.Count < 2) { verticalPieces.Clear(); }
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            { return matchingPieces; }
            //获取垂直方向的相同颜色块
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;
                    if (dir == 0) { y = newY - yOffset; }//上
                    else { y = newY + yOffset; }//下
                    if (y < 0 || y >= yDim) { break; }
                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else { break; }
                }
            }
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                { matchingPieces.Add(verticalPieces[i]); }
            }
            //如果找到匹配，水平遍历L型和T型            
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < yDim; xOffset++)
                        {
                            int x;
                            if (dir == 0) { x = newX - xOffset; }//左
                            else { x = newX + xOffset; }//右
                            if (x < 0 || x >= xDim) { break; }
                            if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else { break; }
                        }
                    }
                    if (horizontalPieces.Count < 2) { horizontalPieces.Clear(); }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            { return matchingPieces; }

        }
        return null;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;
        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (pieces[x, y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);
                    if (match != null)
                    {
                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;
                            }
                        }
                    }
                }
            }
        }
        return needsRefill;
    }

    public bool ClearPiece(int x, int y)
    {
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            return true;
        }
        return false;
    }

}