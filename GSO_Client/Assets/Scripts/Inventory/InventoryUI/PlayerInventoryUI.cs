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
    public TextMeshProUGUI moneyText; //���� ���� ������ ���ý� �߰�
                                      //
    public Data_master_item_backpack equipBag; //���� ������ ����

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
    /// �ڵ鷯���� �ҷ��� �Լ�
    /// </summary>
    public override void InventorySet()
    {
        base.InventorySet();

        Vector2Int newSize = new Vector2Int(equipBag.total_scale_x,equipBag.total_scale_y);


        //������ �׸��带 �ʱ⼼���ϰ� ����ִ� ������
        instantGrid.InstantGrid(newSize, equipBag.total_weight); // ������ ũ��� �ٲܰ�
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

        equipBag = targetBag; //��ó���� ������ �����Ұ��
        return true;
    }

    public bool ResetInventoryGrid()
    {
        // �⺻ ���� ������ �ҷ�����
        if (!TryGetBagData(-1, out Data_master_item_backpack basicBag))
        {
            return false;
        }

        // �⺻ ���� ����
        return ChangeInvenSize(basicBag);
    }

    //�κ��丮�� ���������� ���� ��ü ����
    public bool ChangeInvenSize(Data_master_item_backpack targetBag)
    {
        if (IsSameBag(targetBag)) // ���� �����̸� ��ȭ ���� ����
        {
            return true;
        }

        Vector2Int newSize = new Vector2Int(targetBag.total_scale_x, targetBag.total_scale_y);

        //������ ���ų� ��ü�� �������� �Ǵ�
        if (IsBagSizeReducing(equipBag, newSize) &&
            (!instantGrid.CheckAvailableToChange(newSize) || !CompareChangeWeight(targetBag)))
        {
            Debug.Log("�ش� ���濡 ���� �� ����");
            return false;
        }

        equipBag = targetBag;
        instantGrid.UpdateGridObject(newSize, targetBag.total_weight);
        return true;
    }

    // �ٲ� ������ ���԰� ���� ������ �ִ� �����۵��� ���Ը� ���簡���Ѱ�
    private bool CompareChangeWeight(Data_master_item_backpack targetBag)
    {
        if (targetBag.total_weight < instantGrid.GridWeight)
        {
            StartBlink();
            return false;
        }
        return true;
    }

    //���� db���� �ش� ���̵�� �˻�. �����ϸ� true
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

    // ���� ������ ����� �ٲ� ������ ������ true
    private bool IsSameBag(Data_master_item_backpack targetBag)
    {
        return equipBag.Key == targetBag.Key;
    }

    // ������ ����� �پ��� true
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
            // ��� �ð��� ���� ���� ��ȭ
            weightText.color = Color.Lerp(Color.red, Color.white, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weightText.color = Color.white;
    }

}
