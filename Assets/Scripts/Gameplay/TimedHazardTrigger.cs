using UnityEngine;
using UnityEngine.Rendering;


// This script only handles the timed aspect of the trigger, NOT the hazard itself.
// It only enables and disables the hazard's trigger using times specified by the Designers.
public class TimedHazardTrigger : MonoBehaviour
{
    #region Public Variables
    [field: Header("Times of Duration")]

    [field: Tooltip("How long the triggers that detect the player are inactive for before enabling themselves.")]
    [field: SerializeField] float durationWhileOff;
    [field: Tooltip("How long the triggers that detect the player are active for before disabling themselves.")]
    [field: SerializeField] float durationWhileOn;

    [field: Header("Stove-Specific Variables")]

    [field: Tooltip("As of now, this is hard-coded so that the flames VFX will pause along with the trigger.")]
    [field: SerializeField] bool affectParticles;

    [field: Header("Debug Variables")]

    [field: Tooltip("Enables the trigger's Mesh Renderer to physically show it when it's active. If false, the Mesh Renderer will be disabled.")]
    [field: SerializeField] bool useDebugRenderer;
    #endregion

    #region Private Variables
    private float timer = 0;
    private BoxCollider trigger;
    private MeshRenderer meshRenderer;
    private GameObject[] vfx;
    private PlaySound sound;
    #endregion

    #region Functions - Private
    private void ToggleActiveState()
    {
        trigger.enabled = !trigger.enabled;

        if (sound && trigger.enabled) { sound.PlaySoundOnce(0); sound.StopSoundOnce(1); sound.PlaySoundOnce(2); }
        else if (sound && !trigger.enabled) { sound.PlaySoundOnce(1); sound.StopSoundOnce(0); sound.StopSoundOnce(2); }

        if (transform.childCount > 0 && affectParticles)
        {
            for (int i = 0; i < vfx.Length; i++)
            {
                if (!sound) { vfx[i].SetActive(!vfx[i].activeSelf); return; }
                if (i >= 2) { vfx[i].SetActive(!vfx[i].activeSelf); }
            }
        }


        if (useDebugRenderer && meshRenderer != null) { meshRenderer.enabled = !meshRenderer.enabled; }
        timer = 0;
    }


    private void Update()
    {
        timer += Time.deltaTime;


        if (trigger.enabled)
        {
            if (timer >= durationWhileOn) { ToggleActiveState(); }
            return;
        }
        
        if (timer >= durationWhileOff) { ToggleActiveState(); }
    }


    private void Start()
    {
        trigger = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (useDebugRenderer && meshRenderer != null) { meshRenderer.enabled = true; }
        else if (!useDebugRenderer && meshRenderer != null) { meshRenderer.enabled = false; }

        if (transform.childCount > 0 && affectParticles)
        {
            vfx = new GameObject[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                vfx[i] = transform.GetChild(i).gameObject;
            }
        }

        if (GetComponent<PlaySound>())
        {
            sound = GetComponent<PlaySound>();
        }
    }
    #endregion
}
