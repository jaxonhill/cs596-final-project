using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages dynamic data that multiple scripts need access to
/// </summary>
public static class GlobalGameManager
{
    private static readonly List<Transform> friendlies = new();
    private static readonly List<Transform> enemies = new();

    public static void AddAlly(Transform ally) { friendlies.Add(ally); }
    
    public static void RemoveAlly(Transform ally){friendlies.Remove(ally);}
    
    public static void AddEnemy(Transform enemy) { enemies.Add(enemy); }
    
    public static void RemoveEnemy(Transform enemy){enemies.Remove(enemy);}
    
    public static List<Transform> GetFriendlies(){return friendlies;}
    
    public static List<Transform> GetEnemies(){return enemies;}
}
