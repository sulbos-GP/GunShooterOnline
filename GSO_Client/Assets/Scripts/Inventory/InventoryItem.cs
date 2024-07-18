using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    /*
     * �� �ڵ�� item�����տ� �����Ǹ� �������� ��ü�� �����մϴ�. 
     * �������� ������ �� �������� ���� ����(��ġ? ,ȸ��?, ������? ��)
     * 
     * 1. Set �Լ��� ��Ʈ�ѷ����� ������ ���ÿ� �ҷ����� ��ϵ� �����͸� ���� ������ �����Ű��
     *    ȸ���� ���迩�θ� �ʱ�ȭ�մϴ�.
     *    
     * 2. SearchingItem �Լ��� ��Ʈ�ѷ����� �ش� ������ ������Ʈ�� Ŭ�������� isHide�� true���
     *    �� �Լ��� �����ŵ�ϴ�. �ڷ�ƾ�� ����Ͽ� ��ȸ�ð� �� isHide�� �����ϰ� �������� �̹���
     *    �� �ٲٰ� �˴ϴ�.
     *    
     * 3. RotateRight, RotateLeft �Լ��� ��Ʈ�ѷ����� ȸ�� ����� �ٶ� ����մϴ�.
     *    rotated������ �ٲٸ� ���� RectTransform�� �����ŵ�ϴ�.
    */


    public ItemObjData itemData; //������(������ �ڵ�, �̸�, ��ȸ�ð�, ũ�� , �̹���)
    public Sprite hideSprite; //��ȸ ���� ������ ��������Ʈ

    //�������� ȸ���� ���� ���̿� ���� ��ȭ
    #region Height
    public int Height
    {
        get
        {
            if (curItemRotate % 2 == 1)
            {
                return itemData.height;
            }
            return itemData.width;
        }
    }
    #endregion
    #region Width
    public int Width
    {
        get
        {
            if (curItemRotate % 2 == 1)
            {
                return itemData.width;
            }
            return itemData.height;
        }
    }
    #endregion

    public bool ishide; //������ ���� ������
    private bool isOnSearching; //������ ��ȸ��
    public List<int> searchedPlayerId = new List<int>(); //�� ����Ʈ�� ���Ե� �÷��̾�� �������� ������

    //���� �������� ��ġ, ȸ����, �׸���
    public ItemGrid curItemGrid;
    public Vector2Int curItemPos; //�������� �⺻��ġ
    public int curItemRotate = 0; // 1 : 0 or 360, 2: 90, 3 : 180, 4 : 270

    //��� ����
    public ItemGrid backUpItemGrid; //�������� ���� ������ �׸���
    public Vector2Int backUpItemPos; //�������� ���� ��ġ
    public int backUpItemRotate; //�������� ���� ȸ����

    /// <summary>
    /// �������� �����͸� ����
    /// </summary>
    public void Set(ItemObjData itemData)
    {
        //������ ������ ������Ʈ
        this.itemData = itemData;
        //������ ������ ������Ʈ
        Vector2 size = new Vector2();
        size.x = Width * ItemGrid.tilesizeWidth;
        size.y = Height * ItemGrid.tilesizeHeight;
        transform.GetComponent<RectTransform>().sizeDelta = size;

        //��ȸ�÷��̾� ����Ʈ�� ���Ե� �÷��̾� ���ο� ���� ����
        if (searchedPlayerId.Contains(InventoryController.invenInstance.playerId) == false)
        {
            transform.GetComponent<Image>().sprite = hideSprite;
            ishide = true;
            isOnSearching = false;
        }
        else
        {
            transform.GetComponent<Image>().sprite = itemData.itemSprite;
            ishide = false;
            isOnSearching = true;
        }
    }

    /// <summary>
    /// ������ �������� Ŭ���� ��� ������ ��ȸ
    /// </summary>
    public void SearchingItem()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        //�������� ��ȸ �ð����� ����� ������ ����
        StartCoroutine(SearchingTimer(itemData.itemSearchTime));
    }

    private IEnumerator SearchingTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ishide = false;
        transform.GetComponent<Image>().sprite = itemData.itemSprite;
        searchedPlayerId.Add(InventoryController.invenInstance.playerId);
    }

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateRight()
    {
        curItemRotate = (curItemRotate + 1) % 4;
        Rotate(curItemRotate);
    }

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateLeft()
    {
        curItemRotate = (curItemRotate - 1) % 4;
        Rotate(curItemRotate);
    }

    /// <summary>
    /// �������� ȸ��. rect�� ȸ���ϸ� �̹��� ���� ȸ����.
    /// </summary>
    public void Rotate(int rotateInt)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
    }
}
