using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;

class Constants
{
    public const float Invalid2DCoordinate = -1;

    public const int InvalidCalculationWindow = -1;

    public const ulong InvalidBodySelectionIndex = ulong.MaxValue;

    public class Validation
    {
        public class ErrorMessages
        {
            public const string InitialStateStability = "Please stand upright in the beginning of the jump";
            public const string EndingStateStability = "Please stand upright in the end of the jump";

            public const string MovementDisplacementHorizontal = "Oops, you need to land where you started";
            public const string MovementDisplacementVertical = "Please stand upright during the jump";

            public const string HandsDisplacement = "Please keep hands on hips while you jump";

            public const string AngleDisplacement = "Please stand upright in the beginning of the jump";

            public const string MaximalHeight = "Please jump again";

            public const string NotEnoughPoints = "Jump does not have sufficient number of points";
        }
    }

    public static readonly Dictionary<JointId, JointId> ParentJointMap = new Dictionary<JointId, JointId>()
    {
        {JointId.Pelvis, JointId.Count},
        {JointId.SpineNavel, JointId.Pelvis},
        {JointId.SpineChest, JointId.SpineNavel},
        {JointId.Neck, JointId.SpineChest},
        {JointId.ClavicleLeft, JointId.SpineChest},
        {JointId.ShoulderLeft, JointId.ClavicleLeft},
        {JointId.ElbowLeft, JointId.ShoulderLeft},
        {JointId.WristLeft, JointId.ElbowLeft},
        {JointId.HandLeft, JointId.WristLeft},
        {JointId.HandTipLeft, JointId.HandLeft},
        {JointId.ThumbLeft, JointId.HandLeft},
        {JointId.ClavicleRight, JointId.SpineChest},
        {JointId.ShoulderRight, JointId.ClavicleRight},
        {JointId.ElbowRight, JointId.ShoulderRight},
        {JointId.WristRight, JointId.ElbowRight},
        {JointId.HandRight, JointId.WristRight},
        {JointId.HandTipRight, JointId.HandRight},
        {JointId.ThumbRight, JointId.HandRight},
        {JointId.HipLeft, JointId.SpineNavel},
        {JointId.KneeLeft, JointId.HipLeft},
        {JointId.AnkleLeft, JointId.KneeLeft},
        {JointId.FootLeft, JointId.AnkleLeft},
        {JointId.HipRight, JointId.SpineNavel},
        {JointId.KneeRight, JointId.HipRight},
        {JointId.AnkleRight, JointId.KneeRight},
        {JointId.FootRight, JointId.AnkleRight},
        {JointId.Head, JointId.Pelvis},
        {JointId.Nose, JointId.Head},
        {JointId.EyeLeft, JointId.Head},
        {JointId.EarLeft, JointId.Head},
        {JointId.EyeRight, JointId.Head},
        {JointId.EarRight, JointId.Head},
    };
}