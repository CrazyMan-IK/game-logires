using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires.Models
{
    [Serializable]
    public class Save
    {
        public string Title { get; set; } = "<Unknown>";

        [Serializable]
        public class BlockSave
        {
            [Serializable]
            public class Vector2Save
            {
                Vector2Save() : this(0, 0) { }
                Vector2Save(float x, float y)
                {
                    X = x;
                    Y = y;
                }

                public float X { get; set; }
                public float Y { get; set; }

                public static implicit operator Vector2Save(Vector2 value)
                {
                    return new Vector2Save(value.x, value.y);
                }
                public static implicit operator Vector2Save(Vector3 value)
                {
                    return new Vector2Save(value.x, value.y);
                }

                public static implicit operator Vector2(Vector2Save value)
                {
                    return new Vector2(value.X, value.Y);
                }
                public static implicit operator Vector3(Vector2Save value)
                {
                    return new Vector3(value.X, value.Y);
                }
            }
            [Serializable]
            public class LinkedPin
            {
                public int LinkedBlockIndex { get; set; }
                public int LinkedPinIndex { get; set; }
            }

            public int BlockID { get; set; }
            public Vector2Save Position { get; set; }
            public List<LinkedPin> Inputs { get; private set; } = new List<LinkedPin>();
            public List<List<LinkedPin>> Outputs { get; private set; } = new List<List<LinkedPin>>();
        }

        public List<BlockSave> Blocks { get; set; } = new List<BlockSave>();
    }
}
