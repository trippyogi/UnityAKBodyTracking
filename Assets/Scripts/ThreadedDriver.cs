using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

namespace Assets.Scripts
{
    public class ThreadedDriver : IDisposable
    {
        #region Public properties and methods

        public ThreadedDriver(DeviceSettings settings)
        {
            _settings = settings;

            _captureThread = new Thread(CaptureThread);
            _captureThread.Start();
        }

        public void Dispose()
        {
            _terminate = true;
            _captureThread.Join();

            TrimQueue(0);
            ReleaseLastFrame();

            GC.SuppressFinalize(this);
        }

        public void ReleaseLastFrame()
        {
            _lockedFrame.capture?.Dispose();
            _lockedFrame.color?.Dispose();
            _lockedFrame = (null, null);
        }

        #endregion

        #region Private objects

        DeviceSettings _settings;

        #endregion

        #region Capture queue

        ConcurrentQueue<(Capture capture, Image color)>
            _queue = new ConcurrentQueue<(Capture, Image)>();

        (Capture capture, Image color) _lockedFrame;

        // Trim the queue to a specified count.
        void TrimQueue(int count)
        {
            while (_queue.Count > count)
            {
                (Capture capture, Image color) temp;
                _queue.TryDequeue(out temp);
                temp.capture?.Dispose();
                temp.color?.Dispose();
            }
        }

        #endregion

        #region Capture thread

        Thread _captureThread;
        bool _terminate;

        void CaptureThread()
        {
            // If there is no available device, do nothing.
            if (Device.GetInstalledCount() == 0) return;

            // Open the default device.
            var device = Device.Open();
            Debug.Log("Device Open");

            // Start capturing with custom settings.
            device.StartCameras(new DeviceConfiguration()
            {
                CameraFPS = FPS.FPS30,
                ColorResolution = ColorResolution.Off,
                DepthMode = DepthMode.NFOV_Unbinned,
                WiredSyncMode = WiredSyncMode.Standalone,
            });

            Debug.Log("Camera Started");

            var tracker = Tracker.Create(device.GetCalibration(),
                new TrackerConfiguration()
                    {ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default});

            Debug.Log("Body tracker created");

            // Initially apply the device settings.
            var setter = new DeviceSettingController(device, _settings);

            while (!_terminate)
            {
                // Get a frame capture.
                using (var capture = device.GetCapture())
                {
                    tracker.EnqueueCapture(capture);
                    var bodyFrame = tracker.PopResult();

                    // Do stuff here
                    Debug.Log("Doing stuff");
                }

                // Remove old frames.
                TrimQueue(1);

                // Apply changes on the device settings.
                setter.ApplySettings(device, _settings);
            }

            // Cleaning up.
            tracker.Dispose();
            device.Dispose();
        }

        #endregion
    }
}