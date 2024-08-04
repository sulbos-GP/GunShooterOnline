using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InvenData InputInvenData;
    /*
    public InvenData InputInvenData
    {
        get => playerInvenData;
        set
        {
            playerInvenData = value;
        }
    }*/

    public void LoadPacketSend()
    {
        C_LoadInventory packet = new C_LoadInventory();
        packet.PlayerId = Managers.Object.MyPlayer.Id;
        packet.InventoryId = Managers.Object.MyPlayer.Id;
        Managers.Network.Send(packet);
        Debug.Log("C_LoadInventory(player)");
    }
}
