using Doozy.Runtime.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /*
     * �׸����� ���� ���� �κ��丮 ũ�� ����
     * 
     * ����� �ӽ÷� ����Ʈ�� �׸��带 �߰��ϳ� �׸��尡 �����ɶ� 
     * 
     *  InventoryGrid grid1 = Instantiate(itemGridPrefab, inventoryManager.transform).GetComponent<InventoryGrid>();
     *   grid1.Init(5, 5); // �׸��� �ʱ�ȭ (5x5)
     *
     *   // �� ��° �׸��� ����
     *   InventoryGrid grid2 = Instantiate(itemGridPrefab, inventoryManager.transform).GetComponent<InventoryGrid>();
     *   grid2.Init(3, 3); // �׸��� �ʱ�ȭ (3x3)
     *
     *  // �׸��带 �κ��丮 �Ŵ����� �߰�
     *  inventoryManager.instantGrid.Add(grid1);
     *  inventoryManager.instantGrid.Add(grid2);
     *
     *  // �κ��丮 ũ�� ����
     *   inventoryManager.AdjustInventorySize();
     *   �̷������� �߰��ؾ���
     */
    public int InvenId; //�κ��丮 ������ id =. �������� id
    public GameObject gridPref;
    public InvenData invenData; //�ش� �κ��丮�� ��ġ�� ũ�� �� ������ �׸����� ������

    public List<InventoryGrid> instantGrid; // ������ �κ��丮�� �׸��� ���

    private Transform grids; //�׸������ ���� ������Ʈ

    //�׸���� �������� ������
    public const float offsetX = 20;
    public const float offsetY = 20;

    public float invenWeight = 0; //�׸����� ���� �� ���.
    public float limitWeight; //�κ��丮�� �Ѱ� ����
    public float InvenWeight
    {
        get { return invenWeight; }
        set { invenWeight = value; }
    }

    protected virtual void Init()
    {
        grids = transform.GetChildByName("Grids");

        if(invenData == null ) { return; }
        InvenId = invenData.inventoryId;
        limitWeight = invenData.limitWeight;

        //�κ��丮 �����͸� ������� ���� ������Ʈ
        InventorySet();

        //�����Ϸ�� ���� �κ��丮�� ���Ը� ����
        UpdateInvenWeight();

        //�÷��� �κ��丮�� ����.
        //if (invenData.isFloatInven)
        //{
        //    AdjustInventorySize();
        //}
    }

    private void InventorySet()
    {

        //�׸��� ����
        for(int i =0; i < invenData.gridList.Count; i++)
        {
            InventoryGrid gridInstance = Instantiate(gridPref, grids).GetComponent<InventoryGrid>();
            gridInstance.gridData = invenData.gridList[i];
            instantGrid.Add(gridInstance);
        }
    }

    //���߿� inven �±��� ��ü���� �±׷� ã�Ƽ� ����Ʈ�� �ְ�
    //�κ��丮���� �� �Լ� �����Ͽ� �ϳ��� ���ϸ� �� �κ��丮 ������ ��
    public void UpdateInvenWeight()
    {
        InvenWeight = 0;
        for (int i = 0; i < instantGrid.Count; i++)
        {
            InvenWeight += instantGrid[i].gridWeight;
        }
    }

    public bool CheckingInvenWeight(float addWeight, ItemObject overlap)
    {
        UpdateInvenWeight();
        float curWeight = invenWeight;

        if(overlap != null)
        {
            curWeight -= overlap.itemData.item_weight;
        }

        if (curWeight + addWeight > limitWeight)
        {
            Debug.Log("�κ��丮�� ���԰� �Ѱ踦 ����");
            return false;
        }

        return true;
    }

    
}

/* �÷��� �κ��丮�� �κ��丮�� ��Ȱ��ȭ�ϴ� �ڵ� (�÷��� �κ��丮 ������)
public void InventoryClose()
{
    if (InventoryController.invenInstance.isItemSelected == false)
    {
        gameObject.SetActive(false);
        InventoryController.invenInstance.SelectedInven = null;
    }

// �κ��丮�� ũ�⸦ �����ϴ� �޼���
public void AdjustInventorySize()
{
    if (instantGrid.Count == 0)
        return;

    float minX = float.MaxValue;
    float maxX = float.MinValue;
    float minY = float.MaxValue;
    float maxY = float.MinValue;

    // �� �׸����� ũ��� ��ġ�� ������� �κ��丮 ũ�� ���
    foreach (InventoryGrid grid in instantGrid)
    {
        RectTransform gridRect = grid.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        gridRect.GetWorldCorners(corners);

        // �׸����� ��ǥ�� ũ��
        Vector2 gridPos = grid.gridData.gridPos;
        Vector2 gridSize = new Vector2(grid.gridData.gridSize.x * InventoryGrid.WidthOfTile, grid.gridData.gridSize.y * InventoryGrid.WidthOfTile) ;

        // �׸����� �ڳ� ��ġ ���
        float gridMinX = gridPos.x;
        float gridMaxX = gridPos.x + gridSize.x;
        float gridMinY = gridPos.y - gridSize.y; // �Ʒ��� �������Ƿ� y�� gridSize.y�� ����
        float gridMaxY = gridPos.y;

        minX = Mathf.Min(minX, gridMinX);
        maxX = Mathf.Max(maxX, gridMaxX);
        minY = Mathf.Min(minY, gridMinY);
        maxY = Mathf.Max(maxY, gridMaxY);

    }

    // �κ��丮 ������Ʈ�� ũ�⸦ ����
    float width = maxX - minX + offsetX*2;
    float height = maxY - minY + offsetY*2;

    inventoryRect.sizeDelta = new Vector2(width, height);

    Debug.Log($"InvenData Size - Width: {width}, Height: {height}");
}*/

