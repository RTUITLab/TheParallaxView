using UnityEngine;
using Astra;

namespace AstraSDK
{
    public class AstraUnityContext
    {
        public class AstraDeviceHandler : AndroidJavaProxy
        {
            AstraUnityContext context;
            public AstraDeviceHandler(AstraUnityContext context) : base("com.orbbec.astra.android.AstraDeviceManagerListener")
            {
                this.context = context;
            }

            void onOpenAllDevicesCompleted(AndroidJavaObject obj1, AndroidJavaObject obj2)
            {
                Debug.Log("AstraDeviceHandler: onOpenAllDevicesCompleted");
                context.OnOpenAllDevices();
            }

            void onOpenDeviceCompleted(AndroidJavaObject obj, bool opened)
            {
                Debug.Log("AstraDeviceHandler: onOpenDeviceCompleted");
                context.OnOpenDevice();
            }

            void onNoDevice()
            {
                Debug.Log("AstraDeviceHandler: onNoDevice");
            }
        }

        private static AstraUnityContext _instance;

        public static AstraUnityContext Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AstraUnityContext();
                }
                return _instance;
            }
        }

        private static AndroidJavaObject context;
	    private static AndroidJavaObject currentActivity;

        private bool _initialized = false;

        public delegate void InitializeEventHandler();
        public event InitializeEventHandler OnInitializeSuccess;
        public event InitializeEventHandler OnInitializeFailed;

        public delegate void TerminateEventHandler();
        public event TerminateEventHandler OnTerminated;

        ~AstraUnityContext()
        {
            Debug.Log("Finalizer of AstraUnityContext");
            Terminate();
        }

        // Use this for initialization
        public void Initialize()
        {
            if(_initialized) return;

			Debug.Log("AstraUnityContext initialize");

            EnsureJavaActivity();

            OpenAllDevices();

            // if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            // {
            //     yield return null;
            //     OpenAllDevices();
            // }
            // else
            // {
            //     Debug.Log("AstraSDKManager: request authorization");
            //     yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            //     if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            //     {
            //         OpenAllDevices();
            //     }
            //     else
            //     {
            //         Debug.LogError("AstraSDKManager: authorization failed");
            //     }
            // }
        }

        public void Terminate()
        {
            if (!_initialized)
            {
                return;
            }

            Debug.Log("Astra SDK terminating.");
            
            Context.Terminate();

            if(OnTerminated != null)
            {
                OnTerminated();
            }
            _initialized = false;
        }

        void Update()
        {
            if(_initialized)
            {
                Context.Update();
            }
        }

        private void OpenAllDevices()
        {
            if(context == null) return;

            Debug.Log("AstraSDKManager: openAllDevices");
            context.Call("initialize");
            context.Call("openAllDevices");
        }

        private void EnsureJavaActivity()
        {
            if (context == null)
            {
                Debug.Log("AstraAndroidContext.EnsureJavaActivity() Getting Java activity");
                AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
                currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                context = new AndroidJavaObject("com.orbbec.astra.android.AstraAndroidContext", currentActivity, new AstraDeviceHandler(this));
                Debug.Log("AstraUnityContext.EnsureJavaActivity() Got Java activity");
            }
        }

        public void OnOpenAllDevices()
        {
            Context.Initialize();

            _initialized = true;

            if(OnInitializeSuccess != null)
            {
                OnInitializeSuccess.Invoke();
            }
            // Debug.Log("AstraSDKManager: create streamset");
            // streamSet = StreamSet.Open();
            // if(streamSet != null)
            // {
            //     Debug.Log("AstraSDKManager: create stream reader");
            //     streamReader = streamSet.CreateReader();
            // }
            // streamReader.FrameReady += OnFrameReady;
        }

        public void OnOpenDevice()
        {

        }
    }
}