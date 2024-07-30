using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�׸����� ������
/*
public class SGrid
{
    public Vector2Int gridSize; // �ش� �׸����� ũ�� (x, y �� �� ����)
    public Vector2 gridPos;// �ش� �׸����� �κ��丮 ���� ���� ��ġ
    public List<SItem> itemList;// �ش� �׸���ȿ� �ִ� ������
}*/

//�������� ������
/*
public class SItem
{
    public int itemId; // �ش� �������� ������ ���̵�
    public int itemCode; // �ش� �������� DB���� ��ȸ�ϱ� ���� �ڵ�
    public Vector2Int itemPos; // �������� �׸��� �� ��ǥ���� ��ġ
    public int itemRotate; // �������� ȸ���ڵ�(rotate * 90)
    public int itemAmount; // �������� ����(�Ҹ�ǰ�� 64������)
    public List<int> searchedPlayerList; // �� �������� ��ȸ�� �÷��̾��� ���̵�
}*/

/*
public class SInven
{
    public float limitWeight; //�ش� �κ��丮�� �Ѱ蹫��
    public List<GridData> gridList; //�� �κ��丮���� ������ �׸����� ������
}
*/


//Ŭ�󿡼� �������� ��Ŷ���� �ð� �߰��Ұ�

//�κ��丮 ��û�� ���信 ���� ��Ŷ
/// <summary>
/// Ŭ���̾�Ʈ�� �������� �ش� id�� �κ��丮�� id�� ��û
/// 1. �÷��̾ ���ʷ� �ε�ɶ� �÷��̾��� �κ��丮 id�� ��û
/// 2. �÷��̾ �κ��丮�� ������ ������Ʈ�� ���ͷ�Ʈ �Ҷ� �ش� �κ��丮 ���̵�� ��û
/// </summary>
public class C_LoadInventory 
{
    //���ͷ����� �÷��̾�� �ҷ��� �κ��丮 ���̵�
    public int playerId;
    public int inventoryId;
}

/// <summary>
/// ������ ��û�� ���� �÷��̾�� �ش� �κ��丮�� ������ ������
/// Ŭ��� �� ������ �������� �ش� �κ��丮�� ���� ������ �����Ų�� SET
/// </summary>
public class S_LoadInventory
{

    // Ŭ���̾�Ʈ�� ��û�� �������� �����ִ� �κ��丮�� ����
    public int inventoryId; // �ش� �κ��丮�� ���̵�

    public InvenData inventoryData;

    /* ����(����� �׸��� ����Ʈ ��ȸ������ ���� ����
    public float curWeight; // ���� ����(��� �׸������ ������ ��, ������ �׸��� ����Ʈ�� �ָ� ������ ��)
                            // �׸���� �������� �����͸� ��� �������� �ڵ带 ���� �����ͺ��̽����� ���Ը� ��ȸ�Ҽ� �ֱ⶧��
    */
}


//�÷��̾ �κ��丮�� ������ �ű���
/// <summary>
/// �÷��̾ �������� �ٸ� ��ġ�� ��ġ�������� �������� ������ ������ ����
/// </summary>
public class C_MoveItem
{
    public int playerId; //�ű� �÷��̾��� id
    public int itemId; //�ű� �������� ������
    public Vector2Int itemPos; //�������� �ű� ��ǥ
    public int itemRotate;
    public int inventoryId; // �������� �ű�  �κ��丮�� id
    public int gridNum; //�κ��丮�� ���° �׸�������(Ȥ�� �׸����� id)
    
    //������ ������ �����. �ʿ��ϳ�?
    public Vector2Int lastItemPos; //���� �������� ��ǥ
    public int lastItemRotate;
    public int lastInventoryId; // ���� �κ��丮�� id
    public int lastGridNum; //���� �κ��丮�� ����� �׸�������
}

/// <summary>
/// ������ �ٸ� Ŭ���̾�Ʈ���� �ش� �������� ��� ��ġ�Ǿ� �ִ����� ����
/// Ŭ��� �� �������� �ش� �׸����� �ڽ����� ��ġ�ϰ� ��ġ�� ȸ���� ����
/// </summary>
public class S_MoveItem
{
    //� �������� �����ġ�� ��� ȸ������ ��� ��ġ�Ǿ�����
    public int itemId;
    public Vector2Int itemPos; //�������� �ű� ��ǥ
    public int itemRotate;
    public int inventoryId; // �������� �ű�  �κ��丮�� id
    public int gridNum; //�κ��丮�� ���° �׸�������(Ȥ�� �׸����� id)
}


//�÷��̾ �������� ������ ���
/// <summary>
/// Ŭ�󿡼� ������ ������ �������� ���� ����
/// </summary>
public class C_DeleteItem
{
    //������ �÷��̾��� Id�� ������ item�� id ����
    public int playerId;
    public int itemId;
}

/// <summary>
/// �������� ��� Ŭ���̾�Ʈ�� �ش� �������� �����Ǿ��ٴ� ���� ����
/// Ŭ��� �ش� ���̵� ���� �������� �˻��� ������Ʈ�� ���� ������ ����
/// </summary>
public class S_DeleteItem
{
    //������id�� �˻��Ͽ� �ش� ������ ����
    public int itemId;
}