using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelPrefab", menuName = "Scriptables/Level Prefab", order = 1)]
public class LevelPrefab : ScriptableObject
{
    [field: SerializeField] public string FriendlyName { get; set; }
    
    // ADD SCENE OPTIONS FOR 'SceneHandler.cs' HERE!
    
    //[field: SerializeField] public Scene UnityScene { get; set; }
}
