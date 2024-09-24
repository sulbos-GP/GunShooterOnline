using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    //�׸���� �������� ������
    public const float offsetX = 20;
    public const float offsetY = 20;

    public GridObject instantGrid; //�ش� �κ��丮���� ������ �׸���

    //�κ��丮 ������ ������ ��� ����
    protected virtual void OnDisable()
    {
        InventoryController.instantItemDic.Clear();
        if (instantGrid != null)
        {
            Managers.Resource.Destroy(instantGrid.gameObject);
        }
        

    }

    /// <summary>
    /// �÷��̾ �κ��丮�� UI�� Ȱ��ȭ�ϰų� �ڽ��� ���ͷ�Ʈ�Ͽ� �κ��丮�� ������� packet���� �ش� ����ҿ� �ִ� �������� �����͸� ����
    /// </summary>
    public virtual void InventorySet()
    {
        instantGrid = Managers.Resource.Instantiate("UI/InvenUI/GridUI", transform).GetComponent<GridObject>();
    }
}
