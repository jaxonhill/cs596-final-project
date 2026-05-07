using TriInspector;
using UnityEngine;

namespace GameManaging
{
    [CreateAssetMenu(fileName = "GameSettingsObject"), HideMonoScript]
    public class GameSettingsObject : ScriptableObject
    {
        public bool Debug;
    }
}
