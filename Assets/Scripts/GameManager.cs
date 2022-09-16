using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Canvas;
    [SerializeField]
    private GameObject StartUICanvas;
    [SerializeField]
    private Text Score;
    [SerializeField]
    private Text StartText;
    [SerializeField]
    private GameObject EndText;
    [SerializeField]
    private int ScoreNumber;
    [SerializeField]
    private int CurShapeNum=0;
    [SerializeField]
    private Shape[] CurShape;
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// 8x8的格子
    /// </summary>
    public const int row_max = 8;
    public const int column_max = 8;

    /// <summary>
    /// 存储了整个map是否存在
    /// </summary>
    private GameObject[,] mapArray;

    //private bool isGameStart = false;
    /// <summary>
    /// 先生成单例
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 进行一些绑定
    /// </summary>
    private void Start()
    {
        Canvas = GameObject.Find("GameUI");
        StartUICanvas = GameObject.Find("StartUI");
        Score = GameObject.Find("Score").GetComponent<Text>();
        StartText = GameObject.Find("StartText").GetComponent<Text>();
        EndText = GameObject.Find("EndText");
        EndText.SetActive(false);
        GameObject.Find("Start").GetComponent<Button>().onClick.AddListener(InitMap);

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public int GetCurShapeNum()
    {
        return CurShapeNum;
    }


    /// <summary>
    /// 初始化地图，开始游戏和重新开始游戏时调用
    /// </summary>
    public void InitMap()
    {
        mapArray = new GameObject[column_max, row_max];
        for (int row = 0; row < row_max; row++)
        {
            for (int column = 0; column < column_max; column++)
            {
                mapArray[row, column] = Instantiate(Resources.Load<GameObject>("Prefabs/Grid"));
                mapArray[row, column].transform.SetParent(Canvas.transform.Find("BackGround"));
                mapArray[row, column].GetComponent<Grid>().row = row;
                mapArray[row, column].GetComponent<Grid>().column = column;
            }
        }
        //isGameStart = true;
        SpawnBlock();
        StartUICanvas.SetActive(false);
    }


    /// <summary>
    /// 随机生成可以拖动的方块
    /// </summary>
    public void SpawnBlock()
    {
        for(int i = 1; i < 4; i++)
        {
            int randomType = Random.Range(1, 8);
            GameObject shape = Instantiate(Resources.Load<GameObject>("Prefabs/Shape" + randomType));
            
            CurShape[CurShapeNum] = shape.GetComponent<Shape>();
            CurShape[CurShapeNum].transform.SetParent(Canvas.transform);
            CurShape[CurShapeNum].CalCenter();

            CurShape[CurShapeNum].Init(new Vector2(310.5f*i,400)-0.5f*CurShape[CurShapeNum].GetCenter());

            CurShapeNum++;
        }
        
    }

    /// <summary>
    /// 判断游戏是否结束，本办法，全都遍历一遍
    /// </summary>
    public void CanGameContinue()
    {
        bool GameContinue = false;
        for (int i = 0; i < 3; i++)
        {
            //防止空引用报错
            if (CurShape[i])
            {
                for (int row = 0; row < row_max; row++)
                {
                    for (int column = 0; column < column_max; column++)
                    {
                        if (CanPutDown(row, column, CurShape[i].shapeType,false))
                        {
                            GameContinue = true;
                            return;
                        }
                    }
                }
                
            }
        }

        if (GameContinue == false)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        ScoreNumber = 0;
        for (int i = 0; i < 3; i++)
        {
            if (CurShape[i])
            {
                Destroy(CurShape[i].gameObject);
            }
        }
        CurShapeNum = 0;
        Score.text = ScoreNumber.ToString();
        //isGameStart = false;
        for (int row = 0; row < row_max; row++)
        {
            for (int column = 0; column < column_max; column++)
            {
                Destroy(mapArray[row, column]);
            }
        }
        EndText.SetActive(true);
        StartUICanvas.SetActive(true);
        StartText.text = "重新开始";
    }
    
    /// <summary>
    /// 消除一行或者一列满的情况
    /// </summary>
    public void ClearLine()
    {
        //记录某一行或者列是否满了，需要先判断再清除，不然会有问题
        int[] ClearRow = new int[row_max];
        int[] ClearColumn = new int[column_max];

        //记录一共满了几行几列（需要做减法）
        int RowNum = 0;
        int ColumnNum = 0;


        for (int row = 0; row < row_max; row++)
        {
            for(int column = 0; column < column_max; column++)
            {
                if (mapArray[row, column].GetComponent<Grid>().hasCube == false)//行
                {
                    ClearRow[row]++;//如果是0就说明这一行需要被清除
                }
            }
        }

        for (int column = 0; column < column_max; column++)
        {
            for (int row = 0; row < row_max; row++)
            {
                if (mapArray[row, column].GetComponent<Grid>().hasCube == false)//列
                {
                    ClearColumn[column]++;
                }
            }
        }

        for (int row = 0; row < row_max; row++)
        {
            if (ClearRow[row] == 0)
            {
                for(int column = 0; column < column_max; column++)
                {
                    mapArray[row, column].GetComponent<Grid>().hasCube = false;
                    mapArray[row, column].GetComponent<Image>().color = Color.blue;
                }
                ScoreNumber += column_max;
                RowNum++;
            }
        }

        for (int column = 0; column < column_max; column++)
        {
            if (ClearColumn[column] == 0)
            {
                for (int row = 0; row < row_max; row++)
                {
                    mapArray[row, column].GetComponent<Grid>().hasCube = false;
                    mapArray[row, column].GetComponent<Image>().color = Color.blue;
                }
                ScoreNumber += column_max;
                ColumnNum++;
            }
        }
        ScoreNumber -= (ColumnNum*RowNum);
        Score.text = ScoreNumber.ToString();
    }



    /// <summary>
    /// 这个方法会返回一个布尔值，代表了这个格子是否能够放入俄罗斯方块
    /// </summary>
    /// <param name="row">鼠标所在格子的行</param>
    /// <param name="column">鼠标所在格子的列</param>
    /// <param name="shapeType">鼠标控制的俄罗斯方块的类型</param>
    /// <param name="putDown">如果是True，就放入方块，如果是False，就是单纯的检测</param>
    /// <returns></returns>
    public bool CanPutDown(int row,int column, ShapeType shapeType,bool putDown)
    {
        switch (shapeType)
        {
            case ShapeType.I:
                if (column <= column_max - 4)
                {
                    for (int column_add = 0; column_add < 4; column_add++)
                    {
                        if (mapArray[row, column + column_add].GetComponent<Grid>().hasCube == true && putDown==false)
                        {
                            return false;
                        }
                        if (mapArray[row, column + column_add].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row, column + column_add].GetComponent<Grid>().hasCube = true;
                            mapArray[row, column + column_add].GetComponent<Image>().color = Color.yellow;
                        }
                    }
                }
                else
                {
                    return false;
                }
                break;
            case ShapeType.O:
                if (row <= row_max - 2 && column <= column_max - 2)
                {
                    for(int column_add = 0; column_add < 2; column_add++)
                    {
                        if (mapArray[row, column+ column_add].GetComponent<Grid>().hasCube == true && putDown == false)
                        {
                            return false;
                        }
                        if (mapArray[row, column + column_add].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row, column + column_add].GetComponent<Grid>().hasCube = true;
                            mapArray[row, column + column_add].GetComponent<Image>().color = Color.yellow;
                        }
                    }
                    for (int column_add = 0; column_add < 2; column_add++)
                    {
                        if (mapArray[row+1, column + column_add].GetComponent<Grid>().hasCube == true && putDown == false)
                        {
                            return false;
                        }
                        if (mapArray[row+1, column + column_add].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row+1, column + column_add].GetComponent<Grid>().hasCube = true;
                            mapArray[row+1, column + column_add].GetComponent<Image>().color = Color.yellow;
                        }

                    }
                }
                else
                {
                    return false;
                }
                break;
            case ShapeType.T:
                if (row <= row_max - 2 && column <= column_max - 3)
                {
                    for (int column_add = 0; column_add < 3; column_add++)
                    {
                        if (mapArray[row, column + column_add].GetComponent<Grid>().hasCube == true && putDown == false)
                        {
                            return false;
                        }
                        if (mapArray[row, column + column_add].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row, column + column_add].GetComponent<Grid>().hasCube = true;
                            mapArray[row, column + column_add].GetComponent<Image>().color = Color.yellow;
                        }
                    }
                    if (mapArray[row+1, column + 1].GetComponent<Grid>().hasCube == true && putDown == false)
                    {
                        return false;
                    }
                    if (mapArray[row + 1, column + 1].GetComponent<Grid>().hasCube == false && putDown == true)
                    {
                        mapArray[row + 1, column + 1].GetComponent<Grid>().hasCube = true;
                        mapArray[row + 1, column + 1].GetComponent<Image>().color = Color.yellow;
                    }
                }
                else
                {
                    return false;
                }
                break;
            case ShapeType.Z:
                if (row <= row_max - 2 && column <= column_max - 3)
                {
                    for (int column_add = 0; column_add < 2; column_add++)
                    {
                        if (mapArray[row, column + column_add].GetComponent<Grid>().hasCube == true && putDown == false)
                        {
                            return false;
                        }
                        if (mapArray[row, column + column_add].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row, column + column_add].GetComponent<Grid>().hasCube = true;
                            mapArray[row, column + column_add].GetComponent<Image>().color = Color.yellow;
                        }
                    }
                    for (int column_add = 1; column_add < 3; column_add++)
                    {
                        if (mapArray[row+1, column + column_add].GetComponent<Grid>().hasCube == true && putDown == false)
                        {
                            return false;
                        }
                        if (mapArray[row+1, column + column_add].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row+1, column + column_add].GetComponent<Grid>().hasCube = true;
                            mapArray[row+1, column + column_add].GetComponent<Image>().color = Color.yellow;
                        }
                    }
                }
                else
                {
                    return false;
                }
                break;
            case ShapeType.S:
                if (row <= row_max - 2 && column <= column_max - 2 && column>=1)
                {
                    for (int column_add = 0; column_add < 2; column_add++)
                    {
                        if (mapArray[row, column + column_add].GetComponent<Grid>().hasCube == true && putDown == false)
                        {
                            return false;
                        }
                        if (mapArray[row, column + column_add].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row, column + column_add].GetComponent<Grid>().hasCube = true;
                            mapArray[row, column + column_add].GetComponent<Image>().color = Color.yellow;
                        }
                    }
                    for (int column_add = -1; column_add < 1; column_add++)
                    {
                        if (mapArray[row + 1, column + column_add].GetComponent<Grid>().hasCube == true && putDown == false)
                        {
                            return false;
                        }
                        if (mapArray[row + 1, column + column_add].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row + 1, column + column_add].GetComponent<Grid>().hasCube = true;
                            mapArray[row + 1, column + column_add].GetComponent<Image>().color = Color.yellow;
                        }
                    }
                }
                else
                {
                    return false;
                }
                break;
            case ShapeType.L:
                if (row <= row_max - 3 && column <= column_max - 2)
                {
                    for (int row_add = 0; row_add < 3; row_add++)
                    {
                        if (mapArray[row + row_add, column].GetComponent<Grid>().hasCube == true && putDown == false)
                        {
                            return false;
                        }
                        if (mapArray[row + row_add, column].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row + row_add, column].GetComponent<Grid>().hasCube = true;
                            mapArray[row + row_add, column].GetComponent<Image>().color = Color.yellow;
                        }
                    }
                    if (mapArray[row + 2, column+1].GetComponent<Grid>().hasCube == true && putDown == false)
                    {
                        return false;
                    }
                    if (mapArray[row + 2, column + 1].GetComponent<Grid>().hasCube == false && putDown == true)
                    {
                        mapArray[row + 2, column + 1].GetComponent<Grid>().hasCube = true;
                        mapArray[row + 2, column + 1].GetComponent<Image>().color = Color.yellow;
                    }
                }
                else
                {
                    return false;
                }
                break;
            case ShapeType.J:
                if (row <= row_max - 3 && column >= 1)
                {
                    for (int row_add = 0; row_add < 3; row_add++)
                    {
                        if (mapArray[row + row_add, column].GetComponent<Grid>().hasCube == true && putDown == false)
                        {
                            return false;
                        }
                        if (mapArray[row + row_add, column].GetComponent<Grid>().hasCube == false && putDown == true)
                        {
                            mapArray[row + row_add, column].GetComponent<Grid>().hasCube = true;
                            mapArray[row + row_add, column].GetComponent<Image>().color = Color.yellow;
                        }
                    }
                    if (mapArray[row + 2, column - 1].GetComponent<Grid>().hasCube == true && putDown == false)
                    {
                        return false;
                    }
                    if (mapArray[row + 2, column-1].GetComponent<Grid>().hasCube == false && putDown == true)
                    {
                        mapArray[row + 2, column - 1].GetComponent<Grid>().hasCube = true;
                        mapArray[row + 2, column - 1].GetComponent<Image>().color = Color.yellow;
                    }
                }
                else
                {
                    return false;
                }
                break;
            default:
                break;
        }
        return true;

    }

    /// <summary>
    /// 处理放下方块后的一些事情
    /// </summary>
    public void PutDown()
    {
        ScoreNumber += 4;
        Score.text = ScoreNumber.ToString();
        CurShapeNum--;
    }
}

/// <summary>
/// 代表了7种俄罗斯方块的类型
/// I代表着横着的一根
/// O代表一个方块
/// T代表横3竖1
/// Z和S就代表了O第二行前移后移两种情况
/// L和J就代表了竖3横1的两种情况
/// </summary>
public enum ShapeType
{
    I,
    O,
    T,
    Z,
    S,
    L,
    J
}