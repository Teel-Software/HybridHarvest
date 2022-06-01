using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Exhibition
{
    public class Opponent
    {
        [JsonIgnore] public Sprite Portrait => Resources.Load<Sprite>($"Characters\\{SpriteName}");

        public string Name;
        public string SpriteName;

        public List<Seed> Seeds;

        public Opponent()
        {
        }

        public Opponent(string name, string spriteName, List<Seed> seeds = null)
        {
            Name = name;
            SpriteName = spriteName;
            Seeds = seeds ?? new List<Seed>();
        }
    }
}