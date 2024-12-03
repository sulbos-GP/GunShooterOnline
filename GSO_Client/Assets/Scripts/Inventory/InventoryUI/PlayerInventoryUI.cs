using Google.Protobuf.Protocol;
using GooglePlayGames.BasicApi;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryUI : InventoryUI
{
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI moneyText; //돈에 대한 정보가 나올시 추가
                                      //
    public Data_master_item_backpack equipBag; //현재 장착한 가방

    public void WeightTextSet(double GridWeigt, double limitWeight)
    {
        weightText.text = $"WEIGHT \n {GridWeigt} / {limitWeight}"; 
    }

    private void Awake()
    {

    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    /// <summary>
    /// 핸들러에서 불려질 함수
    /// </summary>
    public override void InventorySet()
    {
        base.InventorySet();

        Vector2Int newSize = new Vector2Int(equipBag.total_scale_x,equipBag.total_scale_y);


        //생성된 그리드를 초기세팅하고 들어있는 아이템
        instantGrid.InstantGrid(newSize, equipBag.total_weight); // 가방의 크기로 바꿀것
    }

    public bool SetInventoryGrid(int itemId)
    {
        if (!TryGetBagData(itemId, out Data_master_item_backpack targetBag))
        {
            return false;
        }

        if (instantGrid != null)
        {
            return ChangeInvenSize(targetBag);
        }

        equipBag = targetBag; //맨처음에 가방을 장착할경우
        return true;
    }

    public bool ResetInventoryGrid()
    {
        // 기본 가방 데이터 불러오기
        if (!TryGetBagData(-1, out Data_master_item_backpack basicBag))
        {
            return false;
        }

        // 기본 가방 장착
        return ChangeInvenSize(basicBag);
    }

    //인벤토리가 열려있을때 가방 교체 로직
    public bool ChangeInvenSize(Data_master_item_backpack targetBag)
    {
        if (IsSameBag(targetBag)) // 같은 가방이면 변화 없이 성공
        {
            return true;
        }

        Vector2Int newSize = new Vector2Int(targetBag.total_scale_x, targetBag.total_scale_y);

        //가방을 빼거나 교체가 가능한지 판단
        if (IsBagSizeReducing(equipBag, newSize) &&
            (!instantGrid.CheckAvailableToChange(newSize) || !CompareChangeWeight(targetBag)))
        {
            Debug.Log("해당 가방에 놓을 수 없음");
            return false;
        }

        equipBag = targetBag;
        instantGrid.UpdateGridObject(newSize, targetBag.total_weight);
        return true;
    }

    // 바꿀 가방의 무게가 현재 가지고 있는 아이템들의 무게를 감당가능한가
    private bool CompareChangeWeight(Data_master_item_backpack targetBag)
    {
        if (targetBag.total_weight < instantGrid.GridWeight)
        {
            StartBlink();
            return false;
        }
        return true;
    }

    //가방 db에서 해당 아이디로 검색. 존재하면 true
    public bool TryGetBagData(int itemId, out Data_master_item_backpack bagData)
    {
        if (itemId == -1)
        {
            Data_master_item_backpack noBag = new Data_master_item_backpack()
            {
                Key = -1,
                total_scale_x = 3,
                total_scale_y = 2,
                total_weight = 5,
            };
            bagData = noBag;
            return true;
        }

        bagData = Data_master_item_backpack.GetData(itemId);
        if (bagData == null)
        {
            return false;
        }

        return true;
    }

    // 현재 장착한 가방과 바꿀 가방이 같으면 true
    private bool IsSameBag(Data_master_item_backpack targetBag)
    {
        return equipBag.Key == targetBag.Key;
    }

    // 가방의 사이즈가 줄어들면 true
    private bool IsBagSizeReducing(Data_master_item_backpack currentBag, Vector2Int newSize)
    {
        return currentBag.total_scale_x > newSize.x || currentBag.total_scale_y > newSize.y;
    }


    public Coroutine blinkCoroutine = null;
    public void StartBlink()
    {
        if (blinkCoroutine != null)
        {
            StopBlink();
        }

        blinkCoroutine = StartCoroutine(BlinkEffect());
    }

    public void StopBlink()
    {
        StopCoroutine(blinkCoroutine);
        blinkCoroutine = null;
    }
    public IEnumerator BlinkEffect()
    {
        float elapsedTime = 0f;
        float transitionTime = 1f;
        weightText.color = Color.red;
        while (elapsedTime < transitionTime)
        {
            // 경과 시간에 따라 색상 변화
            weightText.color = Color.Lerp(Color.red, Color.white, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weightText.color = Color.white;
    }

}
