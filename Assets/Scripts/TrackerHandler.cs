using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;


public class TrackerHandler : MonoBehaviour
{
    public bool drawSkeletons = true;

    public void updateTracker(BackgroundData trackerFrameData)
    {
        //this is an array in case you want to get the n closest bodies
        var closestBody = FindClosestTrackedBody(trackerFrameData);

        // render the closest body
        var skeleton = trackerFrameData.Bodies[closestBody];
        RenderSkeleton(skeleton, 0);
    }

    int FindIndexFromId(BackgroundData frameData, int id)
    {
        var retIndex = -1;
        for (var i = 0; i < (int) frameData.NumOfBodies; i++)
        {
            if ((int) frameData.Bodies[i].Id != id) continue;
            retIndex = i;
            break;
        }

        return retIndex;
    }

    private int FindClosestTrackedBody(BackgroundData trackerFrameData)
    {
        var closestBody = -1;
        const float MAX_DISTANCE = 5000.0f;
        var minDistanceFromKinect = MAX_DISTANCE;
        for (var i = 0; i < (int) trackerFrameData.NumOfBodies; i++)
        {
            var pelvisPosition = trackerFrameData.Bodies[i].JointPositions3D[(int) JointId.Pelvis];
            var pelvisPos =
                new Vector3((float) pelvisPosition.X, (float) pelvisPosition.Y, (float) pelvisPosition.Z);
            if (!(pelvisPos.magnitude < minDistanceFromKinect)) continue;
            closestBody = i;
            minDistanceFromKinect = pelvisPos.magnitude;
        }

        return closestBody;
    }

    public void turnOnOffSkeletons()
    {
        drawSkeletons = !drawSkeletons;
        const int bodyRenderedNum = 0;
        for (var jointNum = 0; jointNum < (int) JointId.Count; jointNum++)
        {
            transform.GetChild(bodyRenderedNum).GetChild(jointNum).gameObject.GetComponent<MeshRenderer>().enabled =
                drawSkeletons;
            transform.GetChild(bodyRenderedNum).GetChild(jointNum).GetChild(0).GetComponent<MeshRenderer>().enabled =
                drawSkeletons;
        }
    }

    public void RenderSkeleton(Body skeleton, int skeletonNumber)
    {
        for (var jointNum = 0; jointNum < (int) JointId.Count; jointNum++)
        {
            var jointPos = new Vector3(skeleton.JointPositions3D[jointNum].X,
                -skeleton.JointPositions3D[jointNum].Y, skeleton.JointPositions3D[jointNum].Z);
            var offsetPosition = transform.rotation * jointPos;
            var positionInTrackerRootSpace = transform.position + offsetPosition;
            var jointRot = new Quaternion(skeleton.JointRotations[jointNum].X,
                skeleton.JointRotations[jointNum].Y, skeleton.JointRotations[jointNum].Z,
                skeleton.JointRotations[jointNum].W);
            transform.GetChild(skeletonNumber).GetChild(jointNum).localPosition = jointPos;
            transform.GetChild(skeletonNumber).GetChild(jointNum).localRotation = jointRot;

            const int boneChildNum = 0;
            if (Constants.ParentJointMap[(JointId) jointNum] != JointId.Head &&
                Constants.ParentJointMap[(JointId) jointNum] != JointId.Count)
            {
                var parentTrackerSpacePosition = new Vector3(
                    skeleton.JointPositions3D[(int) Constants.ParentJointMap[(JointId) jointNum]].X,
                    -skeleton.JointPositions3D[(int) Constants.ParentJointMap[(JointId) jointNum]].Y,
                    skeleton.JointPositions3D[(int) Constants.ParentJointMap[(JointId) jointNum]].Z);
                var boneDirectionTrackerSpace = jointPos - parentTrackerSpacePosition;
                var boneDirectionWorldSpace = transform.rotation * boneDirectionTrackerSpace;
                var boneDirectionLocalSpace =
                    Quaternion.Inverse(transform.GetChild(skeletonNumber).GetChild(jointNum).rotation) *
                    Vector3.Normalize(boneDirectionWorldSpace);
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localScale =
                    new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localRotation =
                    Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position =
                    transform.GetChild(skeletonNumber).GetChild(jointNum).position - 0.5f * boneDirectionWorldSpace;
            }
            else
            {
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).gameObject
                    .SetActive(false);
            }
        }
    }
}