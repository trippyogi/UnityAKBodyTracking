﻿using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class SkeletalTrackingProvider : BackgroundDataProvider
{
    bool readFirstFrame = false;
    TimeSpan initialTimestamp;

    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter { get; set; } = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

    public Stream RawDataLoggingFile = null;

    protected override void RunBackgroundThreadAsync(int id)
    {
        try
        {
            UnityEngine.Debug.Log("Starting body tracker background thread.");

            // Buffer allocations.
            BackgroundData currentFrameData = new BackgroundData();
            // Open device.
            using(Device device = Device.Open(id))
            {
                device.StartCameras(new DeviceConfiguration()
                {
                    CameraFPS = FPS.FPS30,
                    ColorResolution = ColorResolution.Off,
                    DepthMode = DepthMode.NFOV_Unbinned,
                    WiredSyncMode = WiredSyncMode.Standalone,
                });

                UnityEngine.Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum);

                var deviceCalibration = device.GetCalibration();

                using(Tracker tracker = Tracker.Create(deviceCalibration, new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
                {
                    UnityEngine.Debug.Log("Body tracker created.");
                    while(m_runBackgroundThread)
                    {
                        using(Capture sensorCapture = device.GetCapture())
                        {
                            // Queue latest frame from the sensor.
                            tracker.EnqueueCapture(sensorCapture);
                        }

                        // Try getting latest tracker frame.
                        using(Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                        {
                            if(frame == null)
                            {
                                UnityEngine.Debug.Log("Pop result from tracker timeout!");
                            }
                            else
                            {
                                IsRunning = true;
                                // Get number of bodies in the current frame.
                                currentFrameData.NumOfBodies = frame.NumberOfBodies;

                                // Copy bodies.
                                for(uint i = 0; i < currentFrameData.NumOfBodies; i++)
                                {
                                    currentFrameData.Bodies[i].CopyFromBodyTrackingSdk(frame.GetBody(i), deviceCalibration);
                                }

                                // Store depth image.
                                Capture bodyFrameCapture = frame.Capture;
                                Image depthImage = bodyFrameCapture.Depth;
                                if(!readFirstFrame)
                                {
                                    readFirstFrame = true;
                                    initialTimestamp = depthImage.DeviceTimestamp;
                                }

                                // Read image data from the SDK.
                                var depthFrame = MemoryMarshal.Cast<byte, ushort>(depthImage.Memory.Span);

                                if(RawDataLoggingFile != null && RawDataLoggingFile.CanWrite)
                                {
                                    binaryFormatter.Serialize(RawDataLoggingFile, currentFrameData);
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
            if(RawDataLoggingFile != null)
            {
                RawDataLoggingFile.Close();
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }
}
