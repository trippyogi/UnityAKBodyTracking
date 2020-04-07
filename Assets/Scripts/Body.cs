using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;

// Class with relevant information about body
// bodyId and 2d and 3d points of all joints
public struct Body
{
    public System.Numerics.Vector3[] JointPositions3D;
    public System.Numerics.Quaternion[] JointRotations;
    public JointConfidenceLevel[] JointPrecisions;
    public int Length;
    public uint Id;

    public Body(int maxJointsLength)
    {
        JointPositions3D = new System.Numerics.Vector3[maxJointsLength];
        JointRotations = new System.Numerics.Quaternion[maxJointsLength];
        JointPrecisions = new JointConfidenceLevel[maxJointsLength];
        Length = 0;
        Id = 0;
    }

    public void CopyFromBodyTrackingSdk(Microsoft.Azure.Kinect.BodyTracking.Body body)
    {
        Id = body.Id;
        Length = Microsoft.Azure.Kinect.BodyTracking.Skeleton.JointCount;

        for (var bodyPoint = 0; bodyPoint < Length; bodyPoint++)
        {
            // K4ABT joint position unit is in millimeter. We need to convert to meters before we use the values.
            JointPositions3D[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).Position / 1000.0f;
            JointRotations[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).Quaternion;
            JointPrecisions[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).ConfidenceLevel;
        }
    }
}