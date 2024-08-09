using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerInventory : MonoBehaviour
{
    //Object�Ŵ������� �÷��̾ �����Ǹ� ��Ŷ�� ������ �����ϰ� ������Ŷ�� ������ �ڵ鷯���� �ش� ������ �κ������� �Ҵ�
    public InvenData InputInvenData;
    

    public void SendPlayerInvenLoadPacket()
    {
        C_LoadInventory packet = new C_LoadInventory();
        packet.PlayerId = Managers.Object.MyPlayer.Id;
        packet.InventoryId =  Managers.Object.MyPlayer.Id;
        Managers.Network.Send(packet);
        Debug.Log("C_LoadInventory(player)");
    }
}
