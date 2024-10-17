using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class HandPositionManager : MonoBehaviour
{
    public OVRSkeleton rightHandSkeleton;

    private Vector3 lastSavedIndexTipPosition; // Field to store the last saved position

    private bool CanSaveHandPosition()
    {
        if (rightHandSkeleton == null)
        {
            Debug.LogError("rightHandSkeleton is null");
            return false;
        }

        if (!rightHandSkeleton.IsDataValid || !rightHandSkeleton.IsDataHighConfidence)
        {
            Debug.LogError("Skeleton data is not valid or is low confidence");
            return false;
        }

        if (rightHandSkeleton.Bones.Count <= (int)OVRSkeleton.BoneId.Hand_IndexTip)
        {
            Debug.LogError("Index tip bone data is not available");
            return false;
        }

        return true;
    }

    // Method to get the last saved position of the index tip
    public Vector3 GetIndexTipPosition(int attempts = 10)
    {
        if (CanSaveHandPosition())
        {
            SaveHandPosition();
            return lastSavedIndexTipPosition;
        }
        else if (attempts > 0)
        {
            return GetIndexTipPosition(attempts - 1); // Recursive call with decremented attempts
        }

        Debug.Log($"Return zero vector, because the position cannot be obtained after {10 - attempts} attempts");
        return Vector3.zero; // Return zero vector if the position cannot be obtained after 10 attempts
    }

    private void SaveHandPosition()
    {
        OVRBone indexTipBone = rightHandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip];
        lastSavedIndexTipPosition = indexTipBone.Transform.position;
        Debug.Log($"Right Index Tip Position: {lastSavedIndexTipPosition}");
    }
}
