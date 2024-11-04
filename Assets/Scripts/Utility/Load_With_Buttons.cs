using UnityEngine;
using UnityEngine.SceneManagement;

public class Load_With_Buttons : MonoBehaviour
{
    private bool SwitchingScenes = false;

    private void LoadScene(int index)
    {
        if (SwitchingScenes) return;
        
        string levelName = GameSystem.Instance.GetLevelName(index);
        if (string.IsNullOrWhiteSpace(levelName)) return;

        GameSystem.Instance.RequestLoadScene(levelName);
        
        SwitchingScenes = true;
    }

    private void Update()
    {
        if (!GameSystem.Instance.DebugPermitted || SwitchingScenes) return;
        
        bool result = int.TryParse(Input.inputString, out int index);
        if (!result) return;

        LoadScene(index);
    }
}
