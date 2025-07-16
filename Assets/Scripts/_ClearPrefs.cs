#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ClearPrefsOnLoad
{
    public static bool clearPrefs = false;
    static ClearPrefsOnLoad()
    {
        if (clearPrefs == true)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs apagados!");
        }
    }
}
#endif