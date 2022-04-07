using UnityEngine;

namespace Exhibition
{
    public class Opponent
    {
        public string Name;
        public string SpriteName;
        public Sprite Portrait() => Resources.Load<Sprite>($"Characters\\{SpriteName}");
        public Seed[] Seeds; 
        
        public Opponent()
        {
            
        }

        public Opponent(string name, string spriteName)
        {
            Name = name;
            SpriteName = spriteName;
        }
    }
}