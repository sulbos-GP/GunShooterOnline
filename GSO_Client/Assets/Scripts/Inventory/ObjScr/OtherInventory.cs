using Google.Protobuf.Protocol;
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


    public void SendOtherInventoryPacket()
    {
        /* ��Ŷ�� �޾ƿ��� ����
        if (GetComponent<OtherInventory>().InputInvenData.inventoryId == 0)
        {
            C_LoadInventory packet = new C_LoadInventory();
            packet.PlayerId = Managers.Object.MyPlayer.Id;
            packet.InventoryId = GetComponent<Box>().objectId;
            Managers.Network.Send(packet);
            Debug.Log($"C_LoadInventory, player : {packet.PlayerId}, inventory: {packet.InventoryId} ");
        }*/
    }
}
