using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace CrazyGames.Logires.Utils
{
    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        private readonly object _syncObject = new object();

        public int Size { get; private set; }

        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (_syncObject)
            {
                if (Count > Size)
                {
                    TryDequeue(out var _);
                }
            }
        }
    }
}