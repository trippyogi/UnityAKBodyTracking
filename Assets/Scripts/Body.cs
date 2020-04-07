using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;
using System.Numerics;
using UnityEngine;
using System.Runtime.Serialization;

// Class with relevant information about body
// bodyId and 2d and 3d points of all joints
public struct Body
{
    public System.Numerics.Vector3[] JointPositions3D;

    public System.Numerics.Vector2[] JointPositions2D;

    public System.Numerics.Quaternion[] JointRotations;

    public JointConfidenceLevel[] JointPrecisions;

    public int Length;

    public uint Id;

    public Body(int maxJointsLength)
    {
        JointPositions3D = new System.Numerics.Vector3[maxJointsLength];
        JointPositions2D = new System.Numerics.Vector2[maxJointsLength];
        JointRotations = new System.Numerics.Quaternion[maxJointsLength];
        JointPrecisions = new JointConfidenceLevel[maxJointsLength];

        Length = 0;
        Id = 0;
    }

    public static Body DeepCopy(Body copyFromBody)
    {
        var maxJointsLength = copyFromBody.Length;
        var copiedBody = new Body(maxJointsLength);

        for (var i = 0; i < maxJointsLength; i++)
        {
            copiedBody.JointPositions2D[i] = copyFromBody.JointPositions2D[i];
            copiedBody.JointPositions3D[i] = copyFromBody.JointPositions3D[i];
            copiedBody.JointRotations[i] = copyFromBody.JointRotations[i];
            copiedBody.JointPrecisions[i] = copyFromBody.JointPrecisions[i];
        }

        copiedBody.Id = copyFromBody.Id;
        copiedBody.Length = copyFromBody.Length;
        return copiedBody;
    }

    public void CopyFromBodyTrackingSdk(Microsoft.Azure.Kinect.BodyTracking.Body body, Calibration sensorCalibration)
    {
        Id = body.Id;
        Length = Microsoft.Azure.Kinect.BodyTracking.Skeleton.JointCount;

        for (var bodyPoint = 0; bodyPoint < Length; bodyPoint++)
        {
            // K4ABT joint position unit is in millimeter. We need to convert to meters before we use the values.
            JointPositions3D[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).Position / 1000.0f;
            JointRotations[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).Quaternion;
            JointPrecisions[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).ConfidenceLevel;

            var jointPosition = JointPositions3D[bodyPoint];
            var position2d = sensorCalibration.TransformTo2D(
                jointPosition,
                CalibrationDeviceType.Depth,
                CalibrationDeviceType.Depth);

            if (position2d != null)
            {
                JointPositions2D[bodyPoint] = position2d.Value;
            }
            else
            {
                JointPositions2D[bodyPoint].X = Constants.Invalid2DCoordinate;
                JointPositions2D[bodyPoint].Y = Constants.Invalid2DCoordinate;
            }
        }
    }
}