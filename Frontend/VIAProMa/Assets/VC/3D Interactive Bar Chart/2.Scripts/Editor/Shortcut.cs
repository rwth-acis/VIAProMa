using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Uitility.VittorCloud
{
    public class Shortcut : Editor
    {

        [MenuItem("Tools/ActiveToggle _`")]
        static void ToggleActivationSelection()
        {
            try
            {
                var go = Selection.activeGameObject;
                go.SetActive(!go.activeSelf);
            }
            catch (System.Exception asd) { }
        }


        [MenuItem("Tools/Clear Console _]")] // CMD + SHIFT + C
        static void ClearConsole()
        {
            // This simply does "LogEntries.Clear()" the long way:
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
            //Debug.Log("Cleared");
        }


        [MenuItem("Tools/Clear PlayerFrebs _#]")] // CMD + SHIFT + C
        static void ClearPlayerFrebs()
        {
            // This simply does "LogEntries.Clear()" the long way:
            PlayerPrefs.DeleteAll();
            //Debug.Log("PlayerFrebs Clear");
        }
    }
}
