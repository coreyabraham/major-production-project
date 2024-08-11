using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable, CreateAssetMenu(fileName = "Level Prefab", menuName = "Scriptables/Management/Level Prefab", order = 1)]
public class LevelPrefab : ScriptableObject
{
    public string FriendlyName { get; set; }
    public Scene UnityScene { get; set; }

    // ADD SCENE OPTIONS FOR 'SceneHandler.cs' HERE!
}
