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
    /// 8x8�ĸ���
    /// </summary>
    public const int row_max = 8;
    public const int column_max = 8;

    /// <summary>
    /// �洢������map�Ƿ����
    /// </summary>
    private GameObject[,] mapArray;

    //private bool isGameStart = false;
    /// <summary>
    /// �����ɵ���
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
    /// ����һЩ��
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
    /// ��ʼ����ͼ����ʼ��Ϸ�����¿�ʼ��Ϸʱ����
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
    /// ������ɿ����϶��ķ���
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
    /// �ж���Ϸ�Ƿ���������취��ȫ������һ��
    /// </summary>
    public void CanGameContinue()
    {
        bool GameContinue = false;
        for (int i = 0; i < 3; i++)
        {
            //��ֹ�����ñ���
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
        StartText.text = "���¿�ʼ";
    }
    
    /// <summary>
    /// ����һ�л���һ���������
    /// </summary>
    public void ClearLine()
    {
        //��¼ĳһ�л������Ƿ����ˣ���Ҫ���ж����������Ȼ��������
        int[] ClearRow = new int[row_max];
        int[] ClearColumn = new int[column_max];

        //��¼һ�����˼��м��У���Ҫ��������
        int RowNum = 0;
        int ColumnNum = 0;


        for (int row = 0; row < row_max; row++)
        {
            for(int column = 0; column < column_max; column++)
            {
                if (mapArray[row, column].GetComponent<Grid>().hasCube == false)//��
                {
                    ClearRow[row]++;//�����0��˵����һ����Ҫ�����
                }
            }
        }

        for (int column = 0; column < column_max; column++)
        {
            for (int row = 0; row < row_max; row++)
            {
                if (mapArray[row, column].GetComponent<Grid>().hasCube == false)//��
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
    /// ��������᷵��һ������ֵ����������������Ƿ��ܹ��������˹����
    /// </summary>
    /// <param name="row">������ڸ��ӵ���</param>
    /// <param name="column">������ڸ��ӵ���</param>
    /// <param name="shapeType">�����ƵĶ���˹���������</param>
    /// <param name="putDown">�����True���ͷ��뷽�飬�����False�����ǵ����ļ��</param>
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
    /// ������·�����һЩ����
    /// </summary>
    public void PutDown()
    {
        ScoreNumber += 4;
        Score.text = ScoreNumber.ToString();
        CurShapeNum--;
    }
}

/// <summary>
/// ������7�ֶ���˹���������
/// I�����ź��ŵ�һ��
/// O����һ������
/// T�����3��1
/// Z��S�ʹ�����O�ڶ���ǰ�ƺ����������
/// L��J�ʹ�������3��1���������
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