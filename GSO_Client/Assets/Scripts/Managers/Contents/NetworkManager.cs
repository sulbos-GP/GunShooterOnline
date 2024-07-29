using System;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using ServerCore;
using UnityEngine;
using UnityEngine.PlayerLoop;



public class NetworkManager
{
    private readonly ServerSession _session = new();

    public static ClientNetworkService mNetworkService = new ClientNetworkService();

    public int AccountId { get; set; }
    public int Token { get; set; }

    private bool istest = false;
    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }


    public void ConnectToGame(string ip = "")
    {
        istest = true;  //TODO 바꾸기
        
        //var host = Dns.GetHostName();
        //var host = "ec2-3-36-85-125.ap-northeast-2.compute.amazonaws.com";

        var host = Dns.GetHostName();
        if (ip.Length > 3) 
            host = ip;
        var ipHost = Dns.GetHostEntry(host);

        IPAddress ipAddr;
        if (istest)
        {
            ipAddr = IPAddress.Loopback;
            Debug.Log("디버그 모드");
            
        }
        else
        { ipAddr = ipHost.AddressList[0];
            foreach (var _ipAddress in ipHost.AddressList) 
            {
                if (_ipAddress.ToString().Contains("192") || _ipAddress.ToString().Contains("218") || _ipAddress.ToString().Contains("172") | _ipAddress.ToString().Contains("125"))
                {
                    ipAddr = _ipAddress;
                    //ipAddr = IPAddress.Any;
                    break;
                }
            }
        }

        

        Debug.Log($"tryConnection to {ipAddr}");
        var endPoint = new IPEndPoint(ipAddr, 7777);
        Func<Session> session = () => { return _session; };

        mNetworkService.Init(endPoint, session, "SomeConnectionKey");
        mNetworkService.Start();
    }

    public void Update()
    {
        _session.FlushSend();

         var list = PacketQueue.Instance.PopAll();
        foreach (var packet in list)
        {
            var handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(_session, packet.Message);
        }
    }
}