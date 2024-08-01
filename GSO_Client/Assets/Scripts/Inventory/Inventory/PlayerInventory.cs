using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory
{
    //������ ������ �׶� ����
    public int bagLv = 0;

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        C_LoadInventory packet = new C_LoadInventory();
        //packet.PlayerId = Managers.Object.MyPlayer.Id; //�÷��̾ ���� ���¶� �÷��̾� ���̵� ���ҷ���
        //packet.InventoryId = packet.PlayerId;
        packet.PlayerId = packet.InventoryId = InventoryController.invenInstance.playerId; //�ӽ�
        Managers.Network.Send(packet);
        Debug.Log("C_LoadInventory����");
    }

    public void ChangeInventory()
    {
        //���潽���� �ٲ����� �κ��丮 ���� �׸����� ũ��� ������ ������ ũ�⸦ ����

    }
}
