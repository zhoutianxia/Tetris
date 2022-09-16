using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class Shape : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    /// <summary>
    /// ��ʼ��λ��
    /// </summary>
    private Vector2 offset;
    /// <summary>
    /// ����������
    /// </summary>
    private Vector2 Center;
    
    /// <summary>
    /// Shapeö�٣��������еķ��鶼����ͨ�����ö����������
    /// </summary>
    public ShapeType shapeType;

    /// <summary>
    /// ��ʼ��λ�ú���״
    /// </summary>
    /// <param name="pos"></param>
    public void Init(Vector2 pos)
    {
        transform.position = pos;
        offset = pos;
    }

    public Vector2 GetCenter()
    {
        return Center;
    }
    /// <summary>
    /// �����Shape������λ��
    /// </summary>
    public void CalCenter()
    {
        float Max_X = float.MinValue;
        float Max_Y = float.MinValue;
        float Min_X = float.MaxValue;
        float Min_Y = float.MaxValue;
        for (int CubeNum = 0; CubeNum < transform.childCount; CubeNum++)
        {
            Max_X = Mathf.Max(transform.GetChild(CubeNum).GetComponent<RectTransform>().anchoredPosition.x,Max_X); 
            Max_Y = Mathf.Max(transform.GetChild(CubeNum).GetComponent<RectTransform>().anchoredPosition.y,Max_Y); 
            Min_X = Mathf.Min(transform.GetChild(CubeNum).GetComponent<RectTransform>().anchoredPosition.x,Min_X); 
            Min_Y = Mathf.Min(transform.GetChild(CubeNum).GetComponent<RectTransform>().anchoredPosition.y,Min_Y);
        }
        Center = new Vector2((Max_X + Min_X) / 2, (Max_Y + Min_Y) / 2);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //�Ŵ�
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = eventData.position;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //��ǰָ��ĸ���û������
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Grid>())
        {
            int row = eventData.pointerCurrentRaycast.gameObject.GetComponent<Grid>().row;
            int column = eventData.pointerCurrentRaycast.gameObject.GetComponent<Grid>().column;
            bool canChange = GameManager.Instance.CanPutDown(row, column, shapeType, false);//�Ƿ���Ա��滻
            if (canChange)
            {
                GameManager.Instance.CanPutDown(row, column, shapeType,true);
                GameManager.Instance.PutDown();
                if(GameManager.Instance.GetCurShapeNum() == 0)
                {
                    GameManager.Instance.SpawnBlock();
                }
                GameManager.Instance.ClearLine();
                GameManager.Instance.CanGameContinue();
                Destroy(gameObject);
            }
            else
            {
                PutDownFail();
            }
        }
        else
        {
            PutDownFail();
        }
    }
    /// <summary>
    /// δ�ܳɹ����뷽��
    /// </summary>
    public void PutDownFail()
    {
        transform.position = offset;
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }



}
