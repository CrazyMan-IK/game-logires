using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public abstract class WIFI : Block
    {
        [SerializeField] protected string _id = "default";
    }
}