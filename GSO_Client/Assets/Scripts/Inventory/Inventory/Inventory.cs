using Doozy.Runtime.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /*
     * 그리드의 수에 따른 인벤토리 크기 변경
     * 
     * 현재는 임시로 리스트에 그리드를 추가하나 그리드가 생성될때 
     * 
     *  InventoryGrid grid1 = Instantiate(itemGridPrefab, inventoryManager.transform).GetComponent<InventoryGrid>();
     *   grid1.Init(5, 5); // 그리드 초기화 (5x5)
     *
     *   // 두 번째 그리드 생성
     *   InventoryGrid grid2 = Instantiate(itemGridPrefab, inventoryManager.transform).GetComponent<InventoryGrid>();
     *   grid2.Init(3, 3); // 그리드 초기화 (3x3)
     *
     *  // 그리드를 인벤토리 매니저에 추가
     *  inventoryManager.instantGrid.Add(grid1);
     *  inventoryManager.instantGrid.Add(grid2);
     *
     *  // 인벤토리 크기 조정
     *   inventoryManager.AdjustInventorySize();
     *   이런식으로 추가해야함
     */
    public int InvenId; //인벤토리 고유의 id =. 소유자의 id
    public GameObject gridPref;
    public InvenData invenData; //해당 인벤토리의 위치와 크기 및 소유한 그리드의 데이터

    public List<InventoryGrid> instantGrid; // 생성한 인벤토리의 그리드 목록

    private Transform grids; //그리드들의 집합 오브젝트

    //그리드와 배경사이의 오프셋
    public const float offsetX = 20;
    public const float offsetY = 20;

    public float invenWeight = 0; //그리드의 무게 총 결산.
    public float limitWeight; //인벤토리의 한계 무게
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

        //인벤토리 데이터를 기반으로 변수 업데이트
        InventorySet();

        //생성완료시 현재 인벤토리의 무게를 구함
        UpdateInvenWeight();

        //플로팅 인벤토리는 삭제.
        //if (invenData.isFloatInven)
        //{
        //    AdjustInventorySize();
        //}
    }

    private void InventorySet()
    {

        //그리드 생성
        for(int i =0; i < invenData.gridList.Count; i++)
        {
            InventoryGrid gridInstance = Instantiate(gridPref, grids).GetComponent<InventoryGrid>();
            gridInstance.gridData = invenData.gridList[i];
            instantGrid.Add(gridInstance);
        }
    }

    //나중에 inven 태그인 객체들을 태그로 찾아서 리스트로 넣고
    //인벤토리마다 이 함수 실행하여 하나로 더하면 총 인벤토리 무게의 합
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
            Debug.Log("인벤토리의 무게가 한계를 넘음");
            return false;
        }

        return true;
    }

    
}

/* 플로팅 인벤토리의 인벤토리를 비활성화하는 코드 (플로팅 인벤토리 사용안함)
public void InventoryClose()
{
    if (InventoryController.invenInstance.isItemSelected == false)
    {
        gameObject.SetActive(false);
        InventoryController.invenInstance.SelectedInven = null;
    }

// 인벤토리의 크기를 조정하는 메서드
public void AdjustInventorySize()
{
    if (instantGrid.Count == 0)
        return;

    float minX = float.MaxValue;
    float maxX = float.MinValue;
    float minY = float.MaxValue;
    float maxY = float.MinValue;

    // 각 그리드의 크기와 위치를 기반으로 인벤토리 크기 계산
    foreach (InventoryGrid grid in instantGrid)
    {
        RectTransform gridRect = grid.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        gridRect.GetWorldCorners(corners);

        // 그리드의 좌표와 크기
        Vector2 gridPos = grid.gridData.gridPos;
        Vector2 gridSize = new Vector2(grid.gridData.gridSize.x * InventoryGrid.WidthOfTile, grid.gridData.gridSize.y * InventoryGrid.WidthOfTile) ;

        // 그리드의 코너 위치 계산
        float gridMinX = gridPos.x;
        float gridMaxX = gridPos.x + gridSize.x;
        float gridMinY = gridPos.y - gridSize.y; // 아래로 내려가므로 y에 gridSize.y를 빼줌
        float gridMaxY = gridPos.y;

        minX = Mathf.Min(minX, gridMinX);
        maxX = Mathf.Max(maxX, gridMaxX);
        minY = Mathf.Min(minY, gridMinY);
        maxY = Mathf.Max(maxY, gridMaxY);

    }

    // 인벤토리 오브젝트의 크기를 조정
    float width = maxX - minX + offsetX*2;
    float height = maxY - minY + offsetY*2;

    inventoryRect.sizeDelta = new Vector2(width, height);

    Debug.Log($"InvenData Size - Width: {width}, Height: {height}");
}*/

