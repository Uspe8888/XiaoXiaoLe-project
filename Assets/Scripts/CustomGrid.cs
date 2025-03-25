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
        ROW_CLEAR, // 行消除类型
        COLUMN_CLEAR, // 列消除类型
        RAINBOW, // 彩虹类型
        BOOM, //炸弹类型
        COUNT, // 类型数量
    }

    // 使结构体可以在Unity编辑器中序列化显示
    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type; // 游戏块类型
        public GameObject prefab; // 游戏块预制体
    }

    [System.Serializable]
    public struct PiecePostion
    {
        public PieceType type;
        public int x;
        public int y;
    }

    public int xDim; // 网格的X维度
    public int yDim; // 网格的Y维度

    public float fillTime;

    public Level level;

    public PiecePrefab[] piecePrefabs; // 游戏块预制体数组
    public PiecePostion[] initialPieces;

    public GameObject backgroundPrefab; // 背景预制体

    private Dictionary<PieceType, GameObject> piecePrefabDict; // 用于存储游戏块类型和对应预制体的字典
    private GamePiece[,] pieces; // 用于存储游戏块二维数组
    private bool inverse = false;

    private GamePiece pressedPiece; // 按下时的游戏块
    private GamePiece enteredPiece;

    private bool gameOver = false; // 游戏是否结束

    private bool startClear = false;

    public void ClearAwake() // Unity生命周期函数，当脚本实例被初始化时调用
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

            //生成障碍游戏块
            for (int i = 0; i < initialPieces.Length; i++)
            {
                if (initialPieces[i].x >= 0 && initialPieces[i].y >= 0 && initialPieces[i].x < xDim &&
                    initialPieces[i].y < yDim)
                {
                    SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
                }
            }

            // 遍历网格的X和Y维度，生成游戏块
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if (pieces[x, y] == null)
                    {
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                    }
                }
            }

            StartCoroutine(Fill()); // 开始填充网格的协程
        
    }

    public IEnumerator Fill()
    {
        bool needsRefill = true; // 是否需要重新填充的标志
        while (needsRefill) // 如果需要重新填充，则继续循环
        {
            yield return new WaitForSeconds(fillTime); // 等待一段时间
            while (FillStep()) // 执行填充步骤，直到没有块移动
            {
                inverse = !inverse; // 切换方向标志
                yield return new WaitForSeconds(fillTime); // 等待一段时间
            }

            needsRefill = ClearAllValidMatches(); // 清除所有有效的匹配，并检查是否需要重新填充
        }
    }

    public bool FillStep() // 执行一步填充
    {
        bool movedPiece = false; // 是否有块移动的标志
        for (int y = yDim - 2; y >= 0; y--) // 从倒数第二行开始向上遍历
        {
            for (int loopX = 0; loopX < xDim; loopX++) // 遍历每一列
            {
                int x = loopX;
                if (inverse) // 如果方向标志为true，则反向遍历
                {
                    x = xDim - 1 - loopX;
                }

                GamePiece piece = pieces[x, y]; // 获取当前块
                if (piece.IsMovable()) // 如果当前块可移动
                {
                    GamePiece pieceBelow = pieces[x, y + 1]; // 获取下方的块
                    if (pieceBelow.Type == PieceType.EMPTY) // 如果下方块为空
                    {
                        Destroy(pieceBelow.gameObject); // 销毁下方块
                        piece.MovableComponent.Move(x, y + 1, fillTime); // 移动当前块到下方
                        pieces[x, y + 1] = piece; // 更新块的位置
                        SpawnNewPiece(x, y, PieceType.EMPTY); // 在当前位置生成新的空块
                        movedPiece = true; // 标记有块移动
                    }
                    else // 如果下方块不为空，检查旁边是否有空位
                    {
                        for (int diag = -1; diag <= 1; diag++) // 检查左右两侧
                        {
                            if (diag != 0) // 不检查当前列
                            {
                                int diagX = x + diag;
                                if (inverse) // 如果方向标志为true，则反向计算列
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
                                            else if (!pieceAbove.IsMovable() &&
                                                     pieceAbove.Type != PieceType.EMPTY) // 如果遇到不可移动的非空块，标记为false
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
                GameObject newPiece = Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1),
                    Quaternion.identity);
                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent
                    .SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }
        }

        return movedPiece; // 如果有任何块移动，返回true
    }


    public float interval = 0.2f;

    public Vector2 GetWorldPosition(int x, int y)
    {
        // 计算并返回游戏块在游戏世界中的位置
        // transform.position 是当前对象的位置
        // xDim 和 yDim 是游戏块的尺寸
        // x 和 y 是游戏块在网格中的坐标
        return new Vector2(
            transform.position.x - xDim / 2.0f + x + (x * interval), // 计算游戏块的 x 坐标
            transform.position.y + yDim / 2.0f - y - (y * interval) // 计算游戏块的 y 坐标
        );
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        // 在指定位置生成一个新的游戏块
        GameObject newPiece = Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform; // 设置新游戏块的父对象

        GamePiece pieceComponent = newPiece.GetComponent<GamePiece>(); // 获取游戏块组件
        pieces[x, y] = pieceComponent; // 将游戏块组件添加到游戏块数组中
        pieceComponent.Init(x, y, this, type); // 初始化游戏块

        return pieceComponent; // 返回新创建的游戏块组件
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        // 检查两个游戏块是否相邻
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) // 检查是否在同一列且相邻
               || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1); // 检查是否在同一行且相邻
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (gameOver)
        {
            return;
        }

        // 交换两个游戏块的位置
        if (piece1.IsMovable() && piece2.IsMovable()) // 检查两个游戏块是否可移动
        {
            pieces[piece1.X, piece1.Y] = piece2; // 更新游戏块数组中的位置
            pieces[piece2.X, piece2.Y] = piece1;

            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null
                                                             || piece1.Type == PieceType.RAINBOW ||
                                                             piece2.Type == PieceType.RAINBOW)
            {
                // 如果交换后有匹配的游戏块
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime); // 移动游戏块
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

                if (piece1.Type == PieceType.RAINBOW && piece1.IsClearable() && piece2.IsColored()) //如果交换的两个块是彩虹消除块
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();
                    if (clearColor)
                    {
                        clearColor.Color = piece2.ColorComponent.Color;
                    }

                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.RAINBOW && piece2.IsClearable() && piece1.IsColored()) //如果交换的两个块是彩虹消除块
                {
                    ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();
                    if (clearColor)
                    {
                        clearColor.Color = piece1.ColorComponent.Color;
                    }

                    ClearPiece(piece2.X, piece2.Y);
                }

                ClearAllValidMatches(); // 清除所有有效的匹配


                //如果交换的两个块是特殊的消除块，则清除掉
                if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }

                pressedPiece = null; // 清空按下和进入的游戏块
                enteredPiece = null; // 清空按下和进入的游戏块
                StartCoroutine(Fill()); // 填充游戏块

                level.OnMove();
            }
            else
            {
                // 如果没有匹配的游戏块，则恢复原来的位置
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }

    public void PressPiece(GamePiece piece)
    {
        // 记录被按下的游戏块
        pressedPiece = piece;
    }

    public void EnterPiece(GamePiece piece)
    {
        // 记录进入的游戏块
        enteredPiece = piece;
    }

    public void ReleasePiece()
    {
        // 释放游戏块时，如果两个游戏块相邻，则交换它们的位置
        if (IsAdjacent(pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    private bool isStraightMatch;

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        // 检查游戏棋子是否是有颜色的
        if (piece.IsColored())
        {
            // 获取棋子的颜色
            ColorPiece.ColorType color = piece.ColorComponent.Color;
            // 初始化水平和垂直方向的棋子列表
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            // 初始化匹配的棋子列表
            List<GamePiece> matchingPieces = new List<GamePiece>();

            // 将当前棋子添加到水平棋子列表中
            horizontalPieces.Add(piece);

            // 检查水平方向的匹配
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0)
                    {
                        x = newX - xOffset;
                    } // 向左检查
                    else
                    {
                        x = newX + xOffset;
                    } // 向右检查

                    if (x < 0 || x >= xDim)
                    {
                        break;
                    } // 超出边界则停止

                    if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]); // 添加匹配的棋子
                    }
                    else
                    {
                        break;
                    } // 遇到不匹配的棋子则停止
                }
            }

            // 如果水平方向匹配的棋子数大于等于3，则将这些棋子添加到匹配列表中
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            // 如果水平方向匹配的棋子数大于等于3，则检查垂直方向的匹配
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0)
                            {
                                y = newY - yOffset;
                            } // 向上检查
                            else
                            {
                                y = newY + yOffset;
                            } // 向下检查

                            if (y < 0 || y >= yDim)
                            {
                                break;
                            } // 超出边界则停止

                            if (pieces[horizontalPieces[i].X, y].IsColored() &&
                                pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]); // 添加匹配的棋子
                            }
                            else
                            {
                                break;
                            } // 遇到不匹配的棋子则停止
                        }
                    }

                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    } // 如果垂直方向匹配的棋子数小于2，则清空列表
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]); // 将垂直方向匹配的棋子添加到匹配列表中
                        }

                        break;
                    }
                }
            }

            // 如果匹配的棋子数大于等于3，则返回匹配的棋子列表
            if (matchingPieces.Count >= 3)
            {
                isStraightMatch = horizontalPieces.Count == 5;
                return matchingPieces;
            }

            // 清空水平和垂直方向的棋子列表
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            // 检查垂直方向的匹配
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;
                    if (dir == 0)
                    {
                        y = newY - yOffset;
                    } // 向上检查
                    else
                    {
                        y = newY + yOffset;
                    } // 向下检查

                    if (y < 0 || y >= yDim)
                    {
                        break;
                    } // 超出边界则停止

                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]); // 添加匹配的棋子
                    }
                    else
                    {
                        break;
                    } // 遇到不匹配的棋子则停止
                }
            }

            // 如果垂直方向匹配的棋子数大于等于3，则将这些棋子添加到匹配列表中
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            // 如果垂直方向匹配的棋子数大于等于3，则检查水平方向的匹配
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < yDim; xOffset++)
                        {
                            int x;
                            if (dir == 0)
                            {
                                x = newX - xOffset;
                            } // 向左检查
                            else
                            {
                                x = newX + xOffset;
                            } // 向右检查

                            if (x < 0 || x >= xDim)
                            {
                                break;
                            } // 超出边界则停止

                            if (pieces[x, verticalPieces[i].Y].IsColored() &&
                                pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]); // 添加匹配的棋子
                            }
                            else
                            {
                                break;
                            } // 遇到不匹配的棋子则停止
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    } // 如果水平方向匹配的棋子数小于2，则清空列表
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]); // 将水平方向匹配的棋子添加到匹配列表中
                        }

                        break;
                    }
                }
            }

            // 如果匹配的棋子数大于等于3，则返回匹配的棋子列表
            if (matchingPieces.Count >= 3)
            {
                isStraightMatch = verticalPieces.Count == 5;
                return matchingPieces;
            }
        }

        isStraightMatch = false;
        return null; // 如果没有匹配的棋子，则返回null
    }


    public bool ClearAllValidMatches() // 清除所有有效的匹配
    {
        //isHeroSpawned = false; // 重置英雄生成标志

        // 初始化一个布尔变量，用于标记是否需要重新填充网格
        bool needsRefill = false;
        // 遍历整个网格
        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                // 检查当前位置的游戏块是否可清除
                if (pieces[x, y].IsClearable())
                {
                    // 获取与当前游戏块匹配的所有游戏块
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);
                    // 如果找到匹配的游戏块
                    if (match != null)
                    {
                        // 定义特殊块的类型，初始值为 PieceType.COUNT
                        PieceType specialPieceType = PieceType.COUNT;

                        // 从匹配的游戏块列表中随机选择一个游戏块
                        GamePiece randomPiece = match[Random.Range(0, match.Count)];

                        // 获取随机选择的游戏块的坐标
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;

                        // 如果匹配的游戏块数量为4
                        if (match.Count == 4)
                        {
                            // 如果按下的游戏块或进入的游戏块为空
                            if (pressedPiece == null || enteredPiece == null)
                            {
                                // 随机选择一种特殊块类型（行清除或列清除）
                                specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR,
                                    (int)PieceType.COLUMN_CLEAR);
                            }
                            // 如果按下的游戏块和进入的游戏块在同一列
                            else if (pressedPiece.X == enteredPiece.X)
                            {
                                // 设置特殊块类型为列清除
                                specialPieceType = PieceType.COLUMN_CLEAR;
                            }
                            // 如果按下的游戏块和进入的游戏块在同一行
                            else if (pressedPiece.Y == enteredPiece.Y)
                            {
                                // 设置特殊块类型为行清除
                                specialPieceType = PieceType.ROW_CLEAR;
                            }
                        }
                        else if (match.Count >= 5)
                        {
                            if (isStraightMatch)
                            {
                                specialPieceType = PieceType.RAINBOW;
                            }
                            else
                            {
                                Debug.Log("Boom");
                                specialPieceType = PieceType.BOOM;
                            }
                        }


                        // 遍历匹配的游戏块列表
                        for (int i = 0; i < match.Count; i++)
                        {
                            // 清除匹配的游戏块，并根据清除结果更新 needsRefill
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                // 标记需要重新填充
                                needsRefill = true;

                                // 如果当前清除的游戏块是按下的游戏块或进入的游戏块
                                if (match[i] == pressedPiece || match[i] == enteredPiece)
                                {
                                    // 更新特殊块的坐标
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }

                        // 如果有特殊块类型，则生成特殊块
                        if (specialPieceType != PieceType.COUNT)
                        {
                            Destroy(pieces[specialPieceX, specialPieceY]); // 销毁原来的块
                            GamePiece newPiece =
                                SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType); // 生成新的特殊块

                            // 如果特殊块类型是行清除或列清除，则设置新方块的颜色
                            if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUMN_CLEAR)
                                && newPiece.IsColored() && match[0].IsColored())
                            {
                                // 设置新方块的颜色为匹配的第一个方块的颜色
                                newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                            }

                            else if (specialPieceType == PieceType.RAINBOW && newPiece.IsColored())
                            {
                                // 设置新方块的颜色为任意颜色
                                newPiece.ColorComponent.SetColor(ColorPiece.ColorType.ANY);
                            }

                            else if (specialPieceType == PieceType.BOOM && newPiece.IsColored())
                            {
                                newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                            }
                        }
                    }
                }
            }
        }

        // 返回是否需要重新填充网格的标志
        return needsRefill;
    }


    //  public HeroHome hero;
    //  private bool isHeroSpawned = false;// 标志是否已经生成英雄

    // ReSharper disable Unity.PerformanceAnalysis
    public bool ClearPiece(int x, int y) // 清除指定位置的游戏块
    {
        // 检查游戏块是否可清除且未被清除
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            // if (!isHeroSpawned)// 如果英雄还没有生成
            // {
            //     ColorPiece.ColorType color = pieces[x, y].ColorComponent.Color;
            //     hero.spawnHero(color); // 只在此处生成一次
            //     isHeroSpawned = true; // 设置标志为true
            // }


            // 清除游戏块
            pieces[x, y].ClearableComponent.Clear();
            // 在清除的位置生成一个新的空块
            SpawnNewPiece(x, y, PieceType.EMPTY);
            // 清除周围的障碍物
            ClearObstacles(x, y);

            // 返回清除成功的标志    
            return true;
        }

        // 返回清除失败的标志
        return false;
    }

    private void ClearObstacles(int x, int y) // 清除指定位置周围的障碍物
    {
        // 遍历指定位置周围的相邻位置
        for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            // 确保相邻位置在网格范围内且不是当前位置
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                // 检查相邻位置的游戏块是否是障碍物且可清除
                if (pieces[adjacentX, y].Type == PieceType.BARREL && pieces[adjacentX, y].IsClearable())
                {
                    // 清除障碍物
                    pieces[adjacentX, y].ClearableComponent.Clear();
                    // 在清除的位置生成一个新的空块
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }

        // 遍历指定位置周围的相邻位置
        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            // 确保相邻位置在网格范围内且不是当前位置
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                // 检查相邻位置的游戏块是否是障碍物且可清除
                if (pieces[x, adjacentY].Type == PieceType.BARREL && pieces[x, adjacentY].IsClearable())
                {
                    // 清除障碍物
                    pieces[x, adjacentY].ClearableComponent.Clear();
                    // 在清除的位置生成一个新的空块
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }

    public void ClearRow(int row) // 清除指定行的所有游戏块
    {
        // 遍历指定行的所有游戏块
        for (int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row); // 清除指定位置的游戏块
        }
    }

    public void ClearColumn(int column) // 清除指定列的所有游戏块
    {
        // 遍历指定列的所有游戏块
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y); // 清除指定位置的游戏块
        }
    }

    public void ClearColor(ColorPiece.ColorType color) // 清除指定颜色的所有游戏块
    {
        // 遍历整个网格
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].IsColored() && pieces[x, y].ColorComponent.Color == color
                    || color == ColorPiece.ColorType.ANY)
                {
                    ClearPiece(x, y); // 清除指定位置的游戏块
                }
            }
        }
    }

    public void BoomClear(int x, int y)
    {
        // 清除当前方块
        ClearPiece(x, y);

        // 定义周围的八个方向
        int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
        int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };

        // 遍历周围的八个方向
        for (int i = 0; i < 8; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];

            // 检查新位置是否在网格范围内
            if (newX >= 0 && newX < xDim && newY >= 0 && newY < yDim)
            {
                ClearPiece(newX, newY);
            }
        }
    }


    public void GameOver() // 游戏结束
    {
        gameOver = true;
    }

    public List<GamePiece> GetPieceOfType(PieceType type)
    {
        List<GamePiece> pieceOfType = new List<GamePiece>();
        // 遍历整个网格
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].Type == type)
                {
                    pieceOfType.Add(pieces[x, y]);
                }
            }
        }

        return pieceOfType;
    }
}