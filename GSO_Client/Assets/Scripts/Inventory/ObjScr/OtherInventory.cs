using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherInventory : MonoBehaviour
{
    //�ڽ� Ȥ�� �� ��ü �� �÷��̾ ���ͷ�Ʈ�� ��ü�� ������Ʈ
    //�ش� ��ü�� ���ͷ�Ʈ �ϸ� �� ������ �κ������Ͱ� �Ѿ��
    //otherInvenUI���� �̰��� �κ������͸� ��ȸ�Ұ�
    public InvenData InputInvenData
    {
        get => otherInventoryData;
        set
        {
            otherInventoryData = value;
        }
    }
    [SerializeField]
    private InvenData otherInventoryData;
    private OtherInventoryUI otherInvenUI;
}
