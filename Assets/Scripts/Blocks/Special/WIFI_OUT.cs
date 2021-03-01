using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Models;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public struct UdpState
    {
        public UdpClient client;
        public IPEndPoint endPoint;
    }

    public class WIFI_OUT : WIFI, IHaveOutputs
    {
        [SerializeField] private BitPin _output = null;

        public IReadOnlyList<Pin> Outputs { get => new[] { _output }; }

        private void OnEnable()
        {
            Multicaster.Instance.OnMessageReceived += OnMessageReceived;
        }

        private void OnDisable()
        {
            Multicaster.Instance.OnMessageReceived -= OnMessageReceived;
        }

        private void OnMessageReceived(Message message)
        {
            if (message.ID == _id)
            {
                if (message.Value is List<bool> val)
                {
                    _output.Value = val;
                }
            }
        }

        public override int GetID() => 13;
    }
}