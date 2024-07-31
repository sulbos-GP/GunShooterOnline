using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    /*
     * ��ũ���ͺ� ������Ʈ�� �������� �����͸� �����մϴ�.
     * itemCode(������ ������ ���� �ڵ�) , �������� �̸�, �������� �˻��ϴ� �ð�, ũ��, �̹���
     * �� �����Ͽ� ������ �����տ� �����˴ϴ�.
     * 
     * �����δ� ��Ʈ�ѷ��� ����Ʈ�� �־� ��Ʈ�ѷ����� �����Ҷ� ����Ʈ ���� �������� �ϳ��� 
     * ������ŵ�ϴ�.
     */


    

    [Header("������ �����ͺ��̽� ����")]
    public int itemId; // �ش� �������� ������ ���̵�
    public int itemCode; //�������� ����(�ش� �������� DB���� ��ȸ�ϱ� ���� �ڵ�)
    public Vector2Int itemPos; // �������� �׸��� �� ��ǥ���� ��ġ
    public int itemRotate; // �������� ȸ���ڵ�(rotate * 90)
    public int itemAmount = 1; // �������� ����(�Ҹ�ǰ�� 64������)
    public List<int> searchedPlayerId; // �� �������� ��ȸ�� �÷��̾��� ���̵�

    [Header("�ӽ� ��뺯��")]
    //�ӽú���(������ �ڵ带 ���� �����ͺ��̽����� �ҷ����Ⱑ �����Ҷ�����)
    public string item_name;
    public float item_weight; //�������� ����
    public int item_type;
    public int item_string_value;
    public int item_purchase_price;
    public int item_sell_price;
    public float item_searchTime;
    public int width = 1;
    public int height = 1;
    public bool isItemConsumeable; //�ӽ�(������ Ÿ������ ���߰���, ������ ������ �Ҹ�ǰ���� �Ǵ���. ���� �ڵ带 ���� ��ȸ�� ����)
    public Sprite itemSprite;
}
