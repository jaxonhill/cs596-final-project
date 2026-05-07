using GameManaging;
using UnityEngine;

public class PlayerTargetingBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GlobalGameManager.AddAlly(transform);
    }
    
}
