using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[System.Serializable]
public struct VolumeProcessQuality
{
    public bool ignoreOverride;
    public ScalableSettingLevelParameter.Level quality;
}

[System.Serializable]
public struct DOF_Data
{
    public Volume Volume;
    public bool Active;

    public DepthOfFieldModeParameter FocusMode;

    public MinFloatParameter NearRangeStart;
    public MinFloatParameter NearRangeEnd;

    public MinFloatParameter FarRangeStart;
    public MinFloatParameter FarRangeEnd;

    public VolumeProcessQuality Quality;

    public int NearSampleCount;
    public int FarSampleCount;

    public float NearMaxBlur;
    public float FarMaxBlur;
}