using UnityEngine;

public class CutsceneTester : MonoBehaviour
{
    private bool Started = false;

    private float Current = 0.0f;
    private readonly float Max = 1.0f;

    private CameraSystem Camera;

    public void StartEvent(CameraSystem CamSys)
    {
        Started = true;
        Camera = CamSys;
    }

    private void Update()
    {
        if (!Started) return;
        
        if (Current < Max)
        {
            Current += Time.deltaTime;
            return;
        }

        Current = 0.0f;
        Started = false;
        Camera.HookCameraToPlayer();
    }
}
