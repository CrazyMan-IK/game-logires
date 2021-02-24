using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using CrazyGames.Logires.Models;

namespace CrazyGames.Logires
{
    public class Multicaster : MonoBehaviour
    {
        public static readonly int port = 43662;
        public static readonly IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
        public static readonly IPAddress multicastAddress = IPAddress.Parse("224.5.6.7");
        public static readonly IPEndPoint remoteEndPoint = new IPEndPoint(multicastAddress, port);

        private UdpClient _client = null;
        private IPEndPoint _lastReceivedIP = new IPEndPoint(0, 0);
        private static Multicaster _instance = null;

        public delegate void MessageReceivedHandler(Message message);
        public event MessageReceivedHandler OnMessageReceived;

        public static Multicaster Instance
        {
            get
            {
                if (_instance == null)
                {
                    var newInstance = new GameObject("Multicaster").AddComponent<Multicaster>();
                    _instance = newInstance;
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _client = new UdpClient(port);
            _client.JoinMulticastGroup(multicastAddress);

            _client.BeginReceive(ReceiveCallback, null);
        }

        private void OnDestroy()
        {
            if (_client != null)
            {
                _client.DropMulticastGroup(multicastAddress);
                _client.Close();
            }
        }

        private void ReceiveCallback(IAsyncResult res)
        {
            try
            {
                var data = _client.EndReceive(res, ref _lastReceivedIP);

                var mS = new MemoryStream(data);
                var bF = new BinaryFormatter();
                var message = (Message)bF.Deserialize(mS);

                OnMessageReceived?.Invoke(message);

                _client.BeginReceive(ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

        public void BroadcastMessage(Message message)
        {
            var mS = new MemoryStream();
            var bF = new BinaryFormatter();
            bF.Serialize(mS, message);
            var data = mS.GetBuffer();

            _client.Send(data, data.Length, remoteEndPoint);
        }
    }
}