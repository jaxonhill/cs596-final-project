using UnityEngine;

namespace GameManaging
{
    public static class GameSettings
    {
        private static GameSettingsObject settingsObject;

        public static bool Debug;
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Start()
        {
            settingsObject = Resources.Load<GameSettingsObject>("GameSettingsObject");
            Debug = settingsObject.Debug;
        }
    }
}


    

