using UnityEngine;

public class TempPlayerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GlobalGameManager.AddAlly(transform);
    }
    
}
