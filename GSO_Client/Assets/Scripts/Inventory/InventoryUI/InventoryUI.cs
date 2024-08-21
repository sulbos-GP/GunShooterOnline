using Doozy.Runtime.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    /*
     * 인벤토리 UI의 자식객체의 컴포넌트
     * 서버로부터 받은 인벤토리 데이터를 통해 인벤토리를 생성함
     */
    public GameObject gridPref; //그리드의 프리팹
    public InvenData invenData; //서버로 부터 받은 해당 인벤토리의 위치와 크기 및 소유한 그리드의 데이터
    public List<InventoryGrid> instantGridList; // 생성한 인벤토리의 그리드 목록
    
    private Transform grids; //그리드들의 부모 오브젝트

    //그리드와 배경사이의 오프셋
    public const float offsetX = 20;
    public const float offsetY = 20;

    public float invenWeight = 0; //그리드의 무게 총 결산.

    protected virtual float InvenWeight
    {
        get { return invenWeight; }
        set { invenWeight = value; }
    }

    protected virtual void Awake()
    {
        instantGridList = new List<InventoryGrid>();
    }

    public virtual void InventorySet()
    {
        grids = transform.GetChildByName("Grids");
        
        if (invenData == null ) 
        {
            Debug.Log("인벤토리 설정과정에서 인벤데이터가 없음");
            return; 
        }

        //인벤토리 데이터를 기반으로 변수 업데이트
        InvenDataSet();

        //플로팅 인벤토리는 삭제.
        //if (invenData.isFloatInven)
        //{
        //    AdjustInventorySize();
        //}
    }

    /// <summary>
    /// 인벤토리의 그리드데이터들로 그리드를 생성
    /// </summary>
    private void InvenDataSet()
    {
        foreach (GridData data in invenData.gridList) 
        {
            //그리드를 생성하여 Grids객체의 자식으로 생성
            InventoryGrid newGrid = Instantiate(gridPref, grids).GetComponent<InventoryGrid>();
            newGrid.gridData = data;
            newGrid.ownInven = this;
            newGrid.InitializeGrid();
            instantGridList.Add(newGrid);
        }
        
        UpdateInvenWeight();
    }
    
    /// <summary>
    /// 생성된 그리드들의 현재 무게를 모두 더하여 변수에 반영
    /// </summary>
    public void UpdateInvenWeight()
    {
        float weightIndex = 0;
        for (int i = 0; i < instantGridList.Count; i++)
        {
            weightIndex += instantGridList[i].GridWeight;
        }
        InvenWeight = weightIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool CanAffordWeight(float addWeight, ItemObject overlap)
    {
        UpdateInvenWeight();
        float curWeight = invenWeight;

        if(overlap != null)
        {
            curWeight -= overlap.itemData.item_weight;
        }

        if (curWeight + addWeight > invenData.limitWeight)
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
    if (instantGridList.Count == 0)
        return;

    float minX = float.MaxValue;
    float maxX = float.MinValue;
    float minY = float.MaxValue;
    float maxY = float.MinValue;

    // 각 그리드의 크기와 위치를 기반으로 인벤토리 크기 계산
    foreach (InventoryGrid newGrid in instantGridList)
    {
        RectTransform gridRect = newGrid.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        gridRect.GetWorldCorners(corners);

        // 그리드의 좌표와 크기
        Vector2 gridPos = newGrid.gridData.gridPos;
        Vector2 gridSize = new Vector2(newGrid.gridData.gridSize.x * InventoryGrid.WidthOfTile, newGrid.gridData.gridSize.y * InventoryGrid.WidthOfTile) ;

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

