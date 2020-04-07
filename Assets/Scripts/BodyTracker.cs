using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

namespace Assets.Scripts
{
    public class BodyTracker : MonoBehaviour
    {
        [SerializeField] DeviceSettings _deviceSettings = null;
        
        private ThreadedDriver _driver;


        void Start()
        {
            _driver = new ThreadedDriver(_deviceSettings);
        }

        void OnDestroy()
        {
            _driver.Dispose();
        }

        void Update()
        {
        }
    }
}
