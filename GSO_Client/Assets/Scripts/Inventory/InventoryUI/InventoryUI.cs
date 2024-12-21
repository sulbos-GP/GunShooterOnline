using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public const float offsetX = 0;
    public const float offsetY = 0;

    public GridObject instantGrid; 

    public TextMeshProUGUI weightText;

    protected virtual void OnDisable()
    {
        InventoryController.instantItemDic.Clear();
        if (instantGrid != null)
        {
            Managers.Resource.Destroy(instantGrid.gameObject);
        }
    }
    public virtual void InventorySet()
    {
        if(instantGrid == null)
        {
            instantGrid = Managers.Resource.Instantiate("UI/InvenUI/GridUI", transform).GetComponent<GridObject>();
        }
    }

    

    public void SetWeightText(double GridWeigt, double limitWeight, bool valid = true)
    {
        if (valid == false)
        {
            weightText.text = $"-";
        }

        if (GridWeigt > limitWeight) 
        { 
            weightText.color = Color.red;
        }
        else
        {
            weightText.color = Color.white;
        }

        weightText.text = $"WEIGHT {GridWeigt} / {limitWeight}";
    }




    #region Blink
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
    #endregion
}
