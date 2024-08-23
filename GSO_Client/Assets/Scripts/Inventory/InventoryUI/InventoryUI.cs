using Doozy.Runtime.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    //�׸���� �������� ������
    public const float offsetX = 20;
    public const float offsetY = 20;

    public GridObject instantGrid; //�ش� �κ��丮���� ������ �׸���

    public List<ItemData> itemList;

    //�κ��丮 ������ ������ ��� ����
    protected virtual void OnDisable()
    {
        foreach(ItemObject item in InventoryController.invenInstance.instantItemList)
        {
            item.DestroyItem();
        }
        InventoryController.invenInstance.instantItemList.Clear();
    }

    /// <summary>
    /// �÷��̾ �κ��丮�� UI�� Ȱ��ȭ�ϰų� �ڽ��� ���ͷ�Ʈ�Ͽ� �κ��丮�� ������� packet���� �ش� ����ҿ� �ִ� �������� �����͸� ����
    /// </summary>
    public virtual void InventorySet()
    {
        //����
        if(instantGrid == null)
        {
            instantGrid = Managers.Resource.Instantiate("UI/GridUI", transform).GetComponent<GridObject>();
        }
    }
}
