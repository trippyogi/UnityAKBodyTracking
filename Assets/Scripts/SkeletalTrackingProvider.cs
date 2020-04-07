using System;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class SkeletalTrackingProvider : BackgroundDataProvider
{
    private bool _readFirstFrame = false;

    protected override void RunBackgroundThreadAsync(int id)
    {
        try
        {
            Debug.Log("Starting body tracker background thread.");

            // Buffer allocations.
            var currentFrameData = new BackgroundData();
            // Open device.
            using (var device = Device.Open(id))
            {
                device.StartCameras(new DeviceConfiguration()
                {
                    CameraFPS = FPS.FPS30,
                    ColorResolution = ColorResolution.Off,
                    DepthMode = DepthMode.NFOV_Unbinned,
                    WiredSyncMode = WiredSyncMode.Standalone,
                });

                Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum);

                var deviceCalibration = device.GetCalibration();

                using (var tracker = Tracker.Create(deviceCalibration,
                    new TrackerConfiguration()
                        {ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default}))
                {
                    Debug.Log("Body tracker created.");
                    while (m_runBackgroundThread)
                    {
                        using (var sensorCapture = device.GetCapture())
                        {
                            // Queue latest frame from the sensor.
                            tracker.EnqueueCapture(sensorCapture);
                        }

                        // Try getting latest tracker frame.
                        using (var frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                        {
                            if (frame == null)
                            {
                                Debug.Log("Pop result from tracker timeout!");
                            }
                            else
                            {
                                IsRunning = true;
                                // Get number of bodies in the current frame.
                                currentFrameData.NumOfBodies = frame.NumberOfBodies;

                                // Copy bodies.
                                for (uint i = 0; i < currentFrameData.NumOfBodies; i++)
                                {
                                    currentFrameData.Bodies[i]
                                        .CopyFromBodyTrackingSdk(frame.GetBody(i), deviceCalibration);
                                }

                                if (!_readFirstFrame)
                                {
                                    _readFirstFrame = true;
                                }

                                // Update data variable that is being read in the UI thread.
                                SetCurrentFrameData(ref currentFrameData);
                            }
                        }
                    }

                    tracker.Dispose();
                }

                device.Dispose();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}