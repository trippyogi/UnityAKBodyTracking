using System;
using System.Runtime.Serialization;

// Class which contains all data sent from background thread to main thread.
[Serializable]
public class BackgroundData : ISerializable
{
    // Number of detected bodies.
    public ulong NumOfBodies { get; set; }

    // List of all bodies in current frame, each body is list of Body objects.
    public Body[] Bodies { get; set; }

    public BackgroundData(int maxDepthImageSize = 1024 * 1024 * 3, int maxBodiesCount = 20, int maxJointsSize = 100)
    {
        Bodies = new Body[maxBodiesCount];
        for (var i = 0; i < maxBodiesCount; i++)
        {
            Bodies[i] = new Body(maxJointsSize);
        }
    }

    public BackgroundData(SerializationInfo info, StreamingContext context)
    {
        NumOfBodies = (ulong)info.GetValue("NumOfBodies", typeof(ulong));
        Bodies = (Body[])info.GetValue("Bodies", typeof(Body[]));
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("NumOfBodies", NumOfBodies, typeof(ulong));
        var ValidBodies = new Body[NumOfBodies];
        for (var i = 0; i < (int)NumOfBodies; i ++)
        {
            ValidBodies[i] = Bodies[i];
        }
        info.AddValue("Bodies", ValidBodies, typeof(Body[]));
    }
}

