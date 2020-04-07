using UnityEngine;
public class BodyTracker : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    public GameObject Tracker;
    private BackgroundData _lastFrameData = new BackgroundData();
    private BackgroundDataProvider _backgroundDataProvider;

    void Start()
    {
        var skeletalTrackingProvider = new SkeletalTrackingProvider();

        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        skeletalTrackingProvider.StartClientThread(TRACKER_ID);
        _backgroundDataProvider = skeletalTrackingProvider;
    }

    void Update()
    {
        if (!_backgroundDataProvider.IsRunning) return;
        if (!_backgroundDataProvider.GetCurrentFrameData(ref _lastFrameData)) return;
        if (_lastFrameData.NumOfBodies != 0)
        {
            Tracker.GetComponent<TrackerHandler>().UpdateTracker(_lastFrameData);
        }
    }

    void OnDestroy()
    {
        // Stop background threads.
        _backgroundDataProvider?.StopClientThread();
    }
}
