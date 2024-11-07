using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DynamicDepthOfField : MonoBehaviour
{
	[field: SerializeField] private DOF_Data DepthOfFieldData;
	private DepthOfField DOF;
	
	public GameObject _cameraPos;
	public GameObject _player;
	
	private float focalDistance;
	public float focalPlaneRange;
	
	public float VFXLerpSpeed;
	
    void Start()
    {
		if (!DepthOfFieldData.Volume) return;

        bool result = DepthOfFieldData.Volume.profile.TryGet(out DepthOfField component);

        if (!result)
        {
            Debug.LogWarning(name + " | Couldn't get Depth of Field Processing Effect!");
            return;
        }
		
		DOF = component;
    }
	
    void Update()
    {
        focalDistance = Mathf.Abs(_cameraPos.transform.position.z - _player.transform.position.z);
		
		DOF.nearFocusEnd.value = focalDistance - focalPlaneRange;
		DOF.farFocusStart.value = focalDistance + focalPlaneRange;
		
		ModifyDepthOfField();
    }
	
	private void ModifyDepthOfField()
    {
        if (!DOF) return;

        DOF.active = DepthOfFieldData.Active;

        if (!DOF.active) return;

        DOF.nearFocusStart.Interp(DOF.nearFocusStart.value, DepthOfFieldData.NearRangeStart.value, Time.fixedDeltaTime * VFXLerpSpeed);
        DOF.nearFocusEnd.Interp(DOF.nearFocusEnd.value, DepthOfFieldData.NearRangeEnd.value, Time.fixedDeltaTime * VFXLerpSpeed);

        DOF.farFocusStart.Interp(DOF.farFocusStart.value, DepthOfFieldData.FarRangeStart.value, Time.fixedDeltaTime * VFXLerpSpeed);
        DOF.farFocusEnd.Interp(DOF.farFocusEnd.value, DepthOfFieldData.FarRangeEnd.value, Time.fixedDeltaTime * VFXLerpSpeed);

        DOF.nearSampleCount = (int)Mathf.Lerp(DOF.nearSampleCount, DepthOfFieldData.NearSampleCount, Time.fixedDeltaTime * VFXLerpSpeed);
        DOF.farSampleCount = (int)Mathf.Lerp(DOF.farSampleCount, DepthOfFieldData.FarSampleCount, Time.fixedDeltaTime * VFXLerpSpeed);

        DOF.nearMaxBlur = Mathf.Lerp(DOF.nearMaxBlur, DepthOfFieldData.NearMaxBlur, Time.fixedDeltaTime * VFXLerpSpeed);
        DOF.farMaxBlur = Mathf.Lerp(DOF.farMaxBlur, DepthOfFieldData.FarMaxBlur, Time.fixedDeltaTime * VFXLerpSpeed);
        

        DOF.nearFocusStart.overrideState = DepthOfFieldData.NearRangeStart.overrideState;
        DOF.nearFocusEnd.overrideState = DepthOfFieldData.NearRangeEnd.overrideState;

        DOF.farFocusStart.overrideState = DepthOfFieldData.FarRangeStart.overrideState;
        DOF.farFocusEnd.overrideState = DepthOfFieldData.FarRangeEnd.overrideState;

        DOF.focusMode = DepthOfFieldData.FocusMode;

        DOF.quality.levelAndOverride = ((int)DepthOfFieldData.Quality.quality, DepthOfFieldData.Quality.ignoreOverride);
    }
}
