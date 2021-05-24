using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrazyGames.Logires.Utils
{
    public class ValueWrapper<T> where T : struct
    {
        public T Value { get; set; }

        public ValueWrapper(T value)
        {
            Value = value;
        }

        public static implicit operator T(ValueWrapper<T> wrapper)
        {
            return wrapper.Value;
        }

        public static implicit operator ValueWrapper<T>(T value)
        {
            return new ValueWrapper<T>(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}