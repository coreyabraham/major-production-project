using UnityEngine;
using UnityEngine.InputSystem;

public class RebindData : MonoBehaviour
{
    public InputActionReference ActionReference;
    public string[] CarriedBinds;
    public string[] OtherData;
    [HideInInspector] public bool AwaitingInput;
}
 