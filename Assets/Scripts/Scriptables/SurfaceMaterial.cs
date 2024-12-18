using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Surface Material", menuName = "Scriptables/Gameplay/Surface Material", order = 1)]
public class SurfaceMaterial : ScriptableObject
{
    [Tooltip("The Material this surface targets.")]
    public PhysicMaterial Material;

    [Tooltip("The speed the Player will be updated to when standing on.")]
    public float PlayerSpeedModifier;

    [Tooltip("Dictates how \"PlayerSpeedModifier\" is applied")]
    public MathType MathUsage;

    [Tooltip("Prevents the Player from jumping when on this surface.")]
    public bool PreventJumping;

    [Tooltip("Prevents the Player from scurrying when on this surface.")]
    public bool PreventScurrying;

    [Tooltip("The Footstep Sounds that'll play at random whenever you move on this surface.")]
    public AudioClip[] Footsteps;
}
