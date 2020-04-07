using System;
using System.Threading.Tasks;

public abstract class BackgroundDataProvider
{
    public bool IsRunning { get; set; } = false;
    protected volatile bool RunBackgroundThread;
    private BackgroundData _frameBackgroundData = new BackgroundData();
    private bool _latest = false;
    private readonly object _lockObj = new object();

    public void StartClientThread(int id)
    {
        RunBackgroundThread = true;
        Task.Run(() => RunBackgroundThreadAsync(id));
    }

    protected abstract void RunBackgroundThreadAsync(int id);

    public void StopClientThread()
    {
        UnityEngine.Debug.Log("Stopping BackgroundDataProvider thread.");
        RunBackgroundThread = false;
    }

    public void SetCurrentFrameData(ref BackgroundData currentFrameData)
    {
        lock (_lockObj)
        {
            var temp = currentFrameData;
            currentFrameData = _frameBackgroundData;
            _frameBackgroundData = temp;
            _latest = true;
        }
    }

    public bool GetCurrentFrameData(ref BackgroundData dataBuffer)
    {
        lock (_lockObj)
        {
            var temp = dataBuffer;
            dataBuffer = _frameBackgroundData;
            _frameBackgroundData = temp;
            bool result = _latest;
            _latest = false;
            return result;
        }
    }
}
