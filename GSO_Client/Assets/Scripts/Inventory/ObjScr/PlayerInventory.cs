using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerInventory : MonoBehaviour
{
    //Object매니저에서 플레이어가 생성되면 패킷을 서버에 전송하고 응답패킷을 받으면 핸들러에서 해당 변수에 인벤데이터 할당
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
