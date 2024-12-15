using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    //그리드와 배경사이의 오프셋
    public const float offsetX = 0;
    public const float offsetY = 0;

    public GridObject instantGrid; //해당 인벤토리에서 생성된 그리드

    public TextMeshProUGUI weightText;

    //인벤토리 닫을시 아이템 모두 제거
    protected virtual void OnDisable()
    {
        InventoryController.instantItemDic.Clear();
        if (instantGrid != null)
        {
            Managers.Resource.Destroy(instantGrid.gameObject);
        }
    }

    /// <summary>
    /// 플레이어가 인벤토리의 UI를 활성화하거나 박스와 인터렉트하여 인벤토리가 열릴경우 packet으로 해당 저장소에 있는 아이템의 데이터를 받음
    /// </summary>
    public virtual void InventorySet()
    {
        instantGrid = Managers.Resource.Instantiate("UI/InvenUI/GridUI", transform).GetComponent<GridObject>();
    }

    public void SetWeightText(double GridWeigt, double limitWeight, bool valid = true)
    {
        if (valid == false)
        {
            weightText.text = $"-";
        }

        weightText.text = $"WEIGHT \n {GridWeigt} / {limitWeight}";
    }




    #region 아이템 블링크
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
    #endregion
}
