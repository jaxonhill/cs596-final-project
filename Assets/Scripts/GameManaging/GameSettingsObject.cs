using TriInspector;
using UnityEngine;

namespace GameManaging
{
    [CreateAssetMenu(fileName = "GameSettingsObject"), HideMonoScript]
    public class GameSettingsObject : ScriptableObject
    {
        [SerializeField] private bool Debug;
    
        public bool GetDebug() {return Debug;}
    }
}
