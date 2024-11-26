using UnityEngine;


// This script is now used simply for additional Death Stuff related to the player.
// As of now, it handles sounds.
public class HazardSounds : MonoBehaviour, ITouchable
{
    #region Public Variables
    [field: Header("Rat Sounds")]

    [Tooltip("This sound plays when the player enters this trigger.\n\nLeave as unreferenced to disable it.")]
    [field: SerializeField] private AudioSource deathVoice;

    [field: Header("Crow Sounds")]

    [Tooltip("This sound plays when the crow spots the player.\n\nLeave as unreferenced to disable it.")]
    [field: SerializeField] private AudioSource caw;
    [Tooltip("This sound plays as the crow flies towards the player.\n\nLeave as unreferenced to disable it.")]
    [field: SerializeField] private AudioSource swoop;

    [field: Header("Mouse Trap Sounds")]

    [Tooltip("This sound will be used when the player touches a mouse trap.\n\nLeave as unreferenced to disable it.")]
    [field: SerializeField] private AudioSource trapSnap;

    [field: Header("Environment Death Sounds")]

    [Tooltip("This sound will be used for both burning and drowning. It will play when the player touches fire or submerges themselves in water.\n\nLeave as unreferenced to disable it.")]
    [field: SerializeField] private AudioSource burnOrDrown;
    #endregion


    #region Private Variables
    private float hazardResetTimer = 0, soundResetTimer = 0;
    private bool isPlayingSound = false;
    [HideInInspector] public bool hasBeenSpotted = false;        // Should be reset back to false if the player dies and respawns.

    public bool Enabled { get; set; } = true;
    public bool HideOnStartup { get; set; } = false;
    #endregion


    #region Functions - Private
    private void PlaySounds()
    {
        isPlayingSound = true;
        hasBeenSpotted = false;
        hazardResetTimer = 0;


        if (deathVoice) { deathVoice.enabled = true; }

        if (caw && swoop) { caw.enabled = true;
                            swoop.enabled = true; }

        if (trapSnap) { trapSnap.enabled = true; }

        if (burnOrDrown) { burnOrDrown.enabled = true; }

    }

    private void StopSounds()
    {
        isPlayingSound = false;
        soundResetTimer = 0;


        if (deathVoice) { deathVoice.enabled = false; }

        if (caw && swoop) { caw.enabled = false;
                            swoop.enabled = false; }

        if (trapSnap) { trapSnap.enabled = false; }

        if (burnOrDrown) { burnOrDrown.enabled = false; }
    }

    public void Entered(PlayerSystem Player)
    {
        if (!Player) { return; }
        if (Player.IsHidden) { return; }
        hasBeenSpotted = true;
    }

    public void Left(PlayerSystem Player) { }

    public void Staying(PlayerSystem Player) { }
    #endregion

    private void Update()
    {
        if (hasBeenSpotted) { hazardResetTimer += Time.deltaTime; }
        if (isPlayingSound) { soundResetTimer += Time.deltaTime; }

        if (hazardResetTimer >= 1) { PlaySounds(); }
        if (soundResetTimer >= 2) { StopSounds(); }
    }
}
