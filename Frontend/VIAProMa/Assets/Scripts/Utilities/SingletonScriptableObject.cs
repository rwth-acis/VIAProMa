using UnityEngine;

namespace i5.VIAProMa.Utilities
{
    public class SingletonScriptableObject<T> : ScriptableObject
    where T : ScriptableObject
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] storedScriptableObjects = Resources.FindObjectsOfTypeAll<T>();
                    if (storedScriptableObjects.Length == 0)
                    {
                        Debug.LogError("This scriptable object singleton does not exist");
                        return null;
                    }
                    else if (storedScriptableObjects.Length > 1)
                    {
                        Debug.LogError("Multiple versions of a scriptable object singleton exist");
                        return null;
                    }

                    instance = storedScriptableObjects[0];
                }
                return instance;
            }
        }
    }
}