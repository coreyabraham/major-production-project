using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CrowRoot : MonoBehaviour
{
    [field: SerializeField] private List<MultiPositionConstraint> MultiPositions = new();
    [field: SerializeField] private List<MultiAimConstraint> MultiAims = new();

    public void AdjustParameters(float Value)
    {
        for (int i = 0; i < MultiPositions.Count; i++)
            MultiPositions[i].weight = Value;

        for (int i = 0; i < MultiAims.Count; i++)
            MultiAims[i].weight = Value;
    }
}
