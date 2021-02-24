using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires.Models
{
    [Serializable]
    public sealed class Message
    {
        public string ID { get; set; }
        public object Value { get; set; }
    }
}