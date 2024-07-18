using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ItemObjData", menuName = "Inventory/ItemObjData")]
public class ItemObjData : ScriptableObject
{
    /*
     * ��ũ���ͺ� ������Ʈ�� �������� �����͸� �����մϴ�.
     * itemCode(������ ������ ���� �ڵ�) , �������� �̸�, �������� �˻��ϴ� �ð�, ũ��, �̹���
     * �� �����Ͽ� ������ �����տ� �����˴ϴ�.
     * 
     * �����δ� ��Ʈ�ѷ��� ����Ʈ�� �־� ��Ʈ�ѷ����� �����Ҷ� ����Ʈ ���� �������� �ϳ��� 
     * ������ŵ�ϴ�.
     */

    public int itemCode; //�������� ����
    public string itemName;
    public float itemSearchTime;

    [Header("�������� ũ��")]
    public int width = 1;
    public int height = 1;

    [Header("�������� �̹���")]
    public Sprite itemSprite;
}
