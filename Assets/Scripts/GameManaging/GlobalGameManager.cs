using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace GameManaging
{
    /// Manages dynamic data that multiple scripts need access to 
    public static class GlobalGameManager
    {
        private static List<Transform> friendlies = new();
        private static List<Transform> enemies = new();

        public static void ClearLists()
        {
            friendlies = new List<Transform>();
            enemies = new List<Transform>();
        }

        public static void AddAlly(Transform ally)
        {
            if(friendlies.Contains(ally)) return;
            friendlies.Add(ally);
        }

        public static void RemoveAlly(Transform ally)
        {
            if(!friendlies.Contains(ally)) return;
            friendlies.Remove(ally);
        }

        public static void AddEnemy(Transform enemy)
        {
            if (enemies.Contains(enemy)) return;
            enemies.Add(enemy);
        }

        public static void RemoveEnemy(Transform enemy)
        {
            if (!enemies.Contains(enemy)) return;
            enemies.Remove(enemy);
        }
    
        public static List<Transform> GetFriendlies(){return friendlies;}
    
        public static List<Transform> GetEnemies(){return enemies;}

        public static List<Transform> GetTargets(Transform entity)
        {
            if (friendlies.Contains(entity)) return enemies.NotNull().ToList();
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (enemies.Contains(entity)) return friendlies.NotNull().ToList();
            return null;
        }

        public static List<string> GetTargetTags(Transform entity)
        {
            if (friendlies.Contains(entity)) return new List<string> {"Enemy"} ;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (enemies.Contains(entity)) return new List<string> {"Player", "Friendly"};
            return null;
        }
    }
}
