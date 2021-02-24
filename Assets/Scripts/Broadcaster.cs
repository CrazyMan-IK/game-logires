//using System;
//using System.IO;
//using System.Net;
//using System.Net.Sockets;
//using System.Net.NetworkInformation;
//using System.Threading.Tasks;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.Serialization.Formatters.Binary;
//using UnityEngine;
//using CrazyGames.Logires.Models;

//namespace CrazyGames
//{
//    public static class Broadcaster
//    {
//        public static readonly int port = 43662;
//        public static readonly IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
//        public static readonly IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.255"), port);

//        private static UdpClient _client = null;
//        private static IPEndPoint _lastReceivedIP = new IPEndPoint(0, 0);

//        public delegate void MessageReceivedHandler(Message message);
//        public static event MessageReceivedHandler OnMessageReceived;

//        static Broadcaster()
//        {
//            _client = new UdpClient();
//            _client.EnableBroadcast = true;
//            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
//            _client.Client.Bind(localEndPoint);

//            _client.BeginReceive(ReceiveCallback, null);

//            Application.quitting += OnQuit;
//        }

//        private static void ReceiveCallback(IAsyncResult res)
//        {
//            try
//            {
//                var data = _client.EndReceive(res, ref _lastReceivedIP);

//                var mS = new MemoryStream(data);
//                var bF = new BinaryFormatter();
//                var message = (Message)bF.Deserialize(mS);

//                OnMessageReceived?.Invoke(message);

//                _client.BeginReceive(ReceiveCallback, null);
//            }
//            catch (Exception ex)
//            {
//                Debug.LogWarning(ex);
//            }
//        }

//        private static void OnQuit()
//        {
//            _client.Close();
//        }

//        public static void BroadcastMessage(Message message)
//        {
//            var mS = new MemoryStream();
//            var bF = new BinaryFormatter();
//            bF.Serialize(mS, message);
//            var data = mS.GetBuffer();

//            _client.Send(data, data.Length, remoteEndPoint);
//        }
//    }
//}