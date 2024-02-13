using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astra;
using AstraSDK;
using System;
using System.Reflection;
using UnityEngine.Events;

public class AstraSDKManager : Singleton<AstraSDKManager>
{
    private static AndroidJavaObject context;
	private static AndroidJavaObject currentActivity;

    public bool Initialized
    {
        get
        {
            return hasInit;
        }
    }

	private bool hasInit;

    [SerializeField]
    private string _license = "";
    //Get and set license
    public string License
    {
        get
        {
            return _license;
        }
        set
        {
            _license = value;
        }
    }
    
	private StreamSet streamSet;
	// private StreamReader streamReader;
    private StreamReader depthReader;
    private StreamReader colorReader;
    private StreamReader maskedColorReader;
    private StreamReader colorizedBodyReader;
    private StreamReader bodyReader;
	private DepthStream depthStream;
	private ColorStream colorStream;
    private MaskedColorStream maskedColorStream;
    private ColorizedBodyStream colorizedBodyStream;
    private BodyStream bodyStream;
    private DeviceController deviceController;

    public class AstraDeviceHandler : AndroidJavaProxy
    {
		AstraSDKManager manager;
        public AstraDeviceHandler(AstraSDKManager manager) : base("com.orbbec.astra.android.AstraDeviceManagerListener")
        {
			this.manager = manager;
        }

		void onOpenAllDevicesCompleted(AndroidJavaObject obj1, AndroidJavaObject obj2)
		{
			Debug.Log("AstraSDKManager: onOpenAllDevicesCompleted");
			manager.OnOpenAllDevices();
		}

		void onOpenDeviceCompleted(AndroidJavaObject obj, bool opened)
		{
			Debug.Log("AstraSDKManager: onOpenDeviceCompleted");
			manager.OnOpenDevice();
		}

		void onNoDevice()
		{
			Debug.Log("AstraDeviceHandler: onNoDevice");
			manager.OnNoDevice();
		}

        void onPermissionDenied(AndroidJavaObject obj)
        {
            Debug.Log("AstraDeviceHandler: onPermissionDenied");
			manager.OnPermissionDenied();
        }
    }

    // Use this for initialization
    IEnumerator Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		EnsureJavaActivity();
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            yield return null;
			OpenAllDevices();
        }
        else
        {
			Debug.Log("AstraSDKManager: request authorization");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
				OpenAllDevices();
            }
            else
            {
				Debug.LogError("AstraSDKManager: authorization failed");
            }
        }
#else
		yield return null;
		OnOpenAllDevices();
#endif
    }

	void Update()
	{
		if(hasInit)
		{
			Context.Update();
			UpdateTextures();
		}
	}

	void OnDestroy()
	{
		Context.Terminate();
	}

	private void UpdateTextures()
	{
		ReaderFrame frame;
        if (depthReader.TryOpenFrame(0, out frame))
        {
            using (frame)
            {
                DepthFrame depthFrame = frame.GetFrame<DepthFrame>();

                if (depthFrame != null)
                {
                    UpdateDepthTexture(depthFrame);
                }
            }
        }
        if (colorReader.TryOpenFrame(0, out frame))
        {
            using (frame)
            {
				ColorFrame colorFrame = frame.GetFrame<ColorFrame>();

				if(colorFrame != null)
				{
					UpdateColorTexture(colorFrame);
				}
            }
        }
        if (maskedColorReader.TryOpenFrame(0, out frame))
        {
            using (frame)
            {
				MaskedColorFrame maskedColorFrame = frame.GetFrame<MaskedColorFrame>();

                if (maskedColorFrame != null)
                {
                    UpdateMaskedColorTexture(maskedColorFrame);
                }
            }
        }
        if (colorizedBodyReader.TryOpenFrame(0, out frame))
        {
            using (frame)
            {
				ColorizedBodyFrame colorizedBodyFrame = frame.GetFrame<ColorizedBodyFrame>();

				if(colorizedBodyFrame != null)
				{
					UpdateColorizedBodyTexture(colorizedBodyFrame);
				}
            }
        }
        if (bodyReader.TryOpenFrame(0, out frame))
        {
            using (frame)
            {
				BodyFrame bodyFrame = frame.GetFrame<BodyFrame>();
				if(bodyFrame != null)
				{
					UpdateBody(bodyFrame);
				}
            }
        }
	}

	private Texture2D _depthTexture;
    //Get depth texture
    public Texture2D DepthTexture
    {
        get
        {
            return _depthTexture;
        }
    }

	private short[] _depthFrameData;
    private byte[] _depthTextureBuffer;
	public void UpdateDepthTexture(DepthFrame depthFrame)
    {
        if (depthFrame == null)
        {
            return;
        }
        // 拷贝深度流数据
        if (_depthFrameData == null || _depthFrameData.Length != depthFrame.Width * depthFrame.Height)
        {
            _depthFrameData = new short[depthFrame.Width * depthFrame.Height];
        }
        depthFrame.CopyData(ref _depthFrameData);

        // 深度纹理
        if (_depthTexture == null)
        {
            _depthTexture = new Texture2D(depthFrame.Width, depthFrame.Height, TextureFormat.RGB24, false);
        }
        else if (_depthTexture.width != depthFrame.Width || _depthTexture.height != depthFrame.Height)
        {
            _depthTexture.Reinitialize(depthFrame.Width, depthFrame.Height, TextureFormat.RGB24, false);
        }

        if (_depthTextureBuffer == null || _depthTextureBuffer.Length != depthFrame.Width * depthFrame.Height * 3)
        {
            _depthTextureBuffer = new byte[depthFrame.Width * depthFrame.Height * 3];
        }
        int length = _depthFrameData.Length;
        for (int i = 0; i < length; i++)
        {
            short depth = _depthFrameData[i];
            byte depthByte = (byte)0;
            if (depth != 0)
            {
                depthByte = (byte)(255 - (255 * depth / 10000.0f));
            }
            _depthTextureBuffer[i * 3 + 0] = depthByte;
            _depthTextureBuffer[i * 3 + 1] = depthByte;
            _depthTextureBuffer[i * 3 + 2] = depthByte;
        }

        if (_depthTextureBuffer != null && _depthTextureBuffer.Length > 0)
        {
            _depthTexture.LoadRawTextureData(_depthTextureBuffer);
            _depthTexture.Apply(false);
        }
    }

	private Texture2D _colorTexture;
    //Get depth texture
    public Texture2D ColorTexture
    {
        get
        {
            return _colorTexture;
        }
    }

	private byte[] _colorFrameData;
    public void UpdateColorTexture(ColorFrame colorFrame)
    {
        if (colorFrame == null)
        {
            return;
        }
        // 拷贝彩色流数据
        if (_colorFrameData == null || _colorFrameData.Length != colorFrame.ByteLength)
        {
            _colorFrameData = new byte[colorFrame.ByteLength];
        }
        colorFrame.CopyData(ref _colorFrameData);

        // 彩色纹理
        if (_colorTexture == null)
        {
            _colorTexture = new Texture2D(colorFrame.Width, colorFrame.Height, TextureFormat.RGB24, false);
        }
        else if (_colorTexture.width != colorFrame.Width || _colorTexture.height != colorFrame.Height)
        {
            _colorTexture.Reinitialize(colorFrame.Width, colorFrame.Height, TextureFormat.RGB24, false);
        }

        if (_colorFrameData != null && _colorFrameData.Length > 0)
        {
            _colorTexture.LoadRawTextureData(_colorFrameData);
            _colorTexture.Apply(false);
        }
    }

    private Texture2D _maskedColorTexture;
    //Get masked color texture
    public Texture2D MaskedColorTexture
    {
        get
        {
            return _maskedColorTexture;
        }
    }
    private byte[] _maskedColorFrameData;
    public void UpdateMaskedColorTexture(MaskedColorFrame maskedColorFrame)
    {
        if (maskedColorFrame == null)
        {
            return;
        }
        // 拷贝抠图流数据
        if (_maskedColorFrameData == null || _maskedColorFrameData.Length != maskedColorFrame.ByteLength)
        {
            _maskedColorFrameData = new byte[maskedColorFrame.ByteLength];
        }
        maskedColorFrame.CopyData(ref _maskedColorFrameData);

        // 抠图纹理
        if (_maskedColorTexture == null)
        {
            _maskedColorTexture = new Texture2D(maskedColorFrame.Width, maskedColorFrame.Height, TextureFormat.RGBA32, false);
        }
        else if (_maskedColorTexture.width != maskedColorFrame.Width || _maskedColorTexture.height != maskedColorFrame.Height)
        {
            _maskedColorTexture.Reinitialize(maskedColorFrame.Width, maskedColorFrame.Height, TextureFormat.RGBA32, false);
        }

        if (_maskedColorFrameData != null && _maskedColorFrameData.Length > 0)
        {
            _maskedColorTexture.LoadRawTextureData(_maskedColorFrameData);
            _maskedColorTexture.Apply(false);
        }
    }

    private Texture2D _colorizedBodyTexture;
    //Get colorized body texture
    public Texture2D ColorizedBodyTexture
    {
        get
        {
            return _colorizedBodyTexture;
        }
    }

    private byte[] _colorizedBodyFrameData;
    public void UpdateColorizedBodyTexture(ColorizedBodyFrame colorizedBodyFrame)
    {
        if (colorizedBodyFrame == null)
        {
            return;
        }
        // 拷贝label图流数据
        if (_colorizedBodyFrameData == null || _colorizedBodyFrameData.Length != colorizedBodyFrame.ByteLength)
        {
            _colorizedBodyFrameData = new byte[colorizedBodyFrame.ByteLength];
        }
        colorizedBodyFrame.CopyData(ref _colorizedBodyFrameData);

        // label图纹理
        if (_colorizedBodyTexture == null)
        {
            _colorizedBodyTexture = new Texture2D(colorizedBodyFrame.Width, colorizedBodyFrame.Height, TextureFormat.RGBA32, false);
        }
        else if (_colorizedBodyTexture.width != colorizedBodyFrame.Width || _colorizedBodyTexture.height != colorizedBodyFrame.Height)
        {
            _colorizedBodyTexture.Reinitialize(colorizedBodyFrame.Width, colorizedBodyFrame.Height, TextureFormat.RGBA32, false);
        }

        if (_colorizedBodyFrameData != null && _colorizedBodyFrameData.Length > 0)
        {
            _colorizedBodyTexture.LoadRawTextureData(_colorizedBodyFrameData);
            _colorizedBodyTexture.Apply(false);
        }
    }

	private Body[] _bodies = { };
    //Get bodies
    public Body[] Bodies
    {
        get
        {
            return _bodies;
        }
    }

    public bool CorrectBody
    {
        get
        {
            return _correctBody;
        }
        set
        {
            _correctBody = value;
        }
    }
	private bool _correctBody;
    private Astra.Plane _cachedPlane;

	public void UpdateBody(BodyFrame bodyFrame)
    {
        if (bodyFrame == null)
        {
            return;
        }
        if(_correctBody)
        {
            _bodies = GetCorrectBody(bodyFrame);
        }
        else
        {
            bodyFrame.CopyBodyData(ref _bodies);
        }
    }

	private Body[] GetCorrectBody(BodyFrame bodyFrame)
    {
        Body[] bodies = {};
        bodyFrame.CopyBodyData(ref bodies);
        if(bodies == null) return null;
        
        Astra.FloorInfo floor = bodyFrame.FloorInfo;
        Astra.Plane plane = null;
        if(floor.IsFloorDetected)
        {
            plane = floor.FloorPlane;
            _cachedPlane = plane;
        }
        else
        {
            plane = _cachedPlane;
        }
        if(plane == null) return bodies;
        Vector3 normal = new Vector3(plane.A, plane.B, plane.C).normalized;

        Quaternion correct = Quaternion.FromToRotation(normal, Vector3.up);

        foreach(var body in bodies)
        {
            var joints = body.Joints;
            if(joints == null) break;
            foreach(var joint in body.Joints)
            {
                var pos = joint.WorldPosition;
                var correctPos = correct * new Vector3(pos.X, pos.Y, pos.Z);
                var newPos = new Vector3D(correctPos.x, correctPos.y, correctPos.z);

                PropertyInfo property = typeof(Astra.Joint).GetProperty("WorldPosition");
                property.DeclaringType.GetProperty("WorldPosition");
                property.GetSetMethod(true).Invoke(joint, new object[] {newPos});
            }
        }
        return bodies;
    }

	private void OpenAllDevices()
	{
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

	private void OnOpenAllDevices()
	{
		Context.Initialize();
		hasInit = true;
        if (!string.IsNullOrEmpty(_license))
        {
            Debug.Log("Set license: " + _license);
            Astra.BodyTracking.SetLicense(_license);
        }
		Debug.Log("AstraSDKManager: create streamset");
		streamSet = StreamSet.Open();
		if(streamSet != null)
		{
			Debug.Log("AstraSDKManager: create stream reader");
			// streamReader = streamSet.CreateReader();
            depthReader = streamSet.CreateReader();
            colorReader = streamSet.CreateReader();
            maskedColorReader = streamSet.CreateReader();
            colorizedBodyReader = streamSet.CreateReader();
            bodyReader = streamSet.CreateReader();
            deviceController = streamSet.CreateDeviceController();
		}
        if(depthReader != null)
        {
            depthStream = depthReader.GetStream<DepthStream>();
        }
        if(colorReader != null)
        {
            colorStream = colorReader.GetStream<ColorStream>();
        }
        if(maskedColorReader != null)
        {
            maskedColorStream = maskedColorReader.GetStream<MaskedColorStream>();
        }
        if(colorizedBodyReader != null)
        {
            colorizedBodyStream = colorizedBodyReader.GetStream<ColorizedBodyStream>();
        }
        if(bodyReader != null)
        {
            bodyStream = bodyReader.GetStream<BodyStream>();
        }
		// if(streamReader != null)
		// {
		// 	depthStream = streamReader.GetStream<DepthStream>();
		// 	colorStream = streamReader.GetStream<ColorStream>();
        //     maskedColorStream = streamReader.GetStream<MaskedColorStream>();
        //     colorizedBodyStream = streamReader.GetStream<ColorizedBodyStream>();
        //     bodyStream = streamReader.GetStream<BodyStream>();
		// }
        OnInitializeSuccess.Invoke();
		// streamReader.FrameReady += OnFrameReady;
	}

	private void OnOpenDevice()
	{

	}

	private void OnNoDevice()
    {
        hasInit = false;
		Debug.Log("AstraSDKManager: init failed no device");
        OnInitializeFailed.Invoke();
    }

    private void OnPermissionDenied()
    {
        hasInit = false;
        Debug.Log("AstraSDKManager: init failed premission denied");
        OnInitializeFailed.Invoke();
    }

	private void OnFrameReady(object sender, FrameReadyEventArgs e)
	{
		var depthFrame = e.Frame.GetFrame<DepthFrame>();
		if(depthFrame != null)
		{
			Debug.Log("AstraSDKManager: get depth frame");
		}

		var colorFrame = e.Frame.GetFrame<ColorFrame>();
		if(colorFrame != null)
		{
			Debug.Log("AstraSDKManager: get color frame");
		}
	}

	public ImageMode[] AvailableDepthModes
    {
        get
        {
            return depthStream.AvailableModes;
        }
    }

    //Get and set depth mode
    public ImageMode DepthMode
    {
        get
        {
             return depthStream.GetMode();
        }
        set
        {

            depthStream.SetMode(value);
        }
    }

    //Get and set depth stream on
    public bool IsDepthOn
    {
        get
        {
            return _isDepthOn;
        }
        set
        {
            if(value)
            {
                StartDepthStream();
            }
            else
            {
                StopDepthStream();
            }
        }
    }
    private bool _isDepthOn;

    public void StartDepthStream()
	{
        // if(streamReader != null)
		// {
		// 	depthStream = streamReader.GetStream<DepthStream>();
		// }
		if(depthStream != null)
        {
			depthStream.Start();
            _isDepthOn = true;
        }
	}

	public void StopDepthStream()
	{
        // if(streamReader != null)
		// {
		// 	depthStream = streamReader.GetStream<DepthStream>();
		// }
		if(depthStream != null)
        {
			depthStream.Stop();
            _isDepthOn = false;
        }
	}

	public ImageMode[] AvailableColorModes
    {
        get
        {
            return colorStream.AvailableModes;
        }
    }

    //Get and set color mode
    public ImageMode ColorMode
    {
        get
        {
            return colorStream.GetMode();
        }
        set
        {
            colorStream.SetMode(value);
        }
    }

    public bool IsColorOn
    {
        get
        {
            return _isColorOn;
        }
        set
        {
            if(value)
            {
                StartColorStream();
            }
            else
            {
                StopColorStream();
            }
        }
    }

    private bool _isColorOn;

	public void StartColorStream()
	{
        // if(streamReader != null)
		// {
		// 	colorStream = streamReader.GetStream<ColorStream>();
		// }
		if(colorStream != null)
		{
			colorStream.Start();
            _isColorOn = true;
		}
	}

	public void StopColorStream()
	{
        // if(streamReader != null)
		// {
		// 	colorStream = streamReader.GetStream<ColorStream>();
		// }
		if(colorStream != null)
		{
			colorStream.Stop();
            _isColorOn = false;
		}
	}

    public bool IsMaskedColorOn
    {
        get
        {
            return _isMaskedColorOn;
        }
        set
        {
            if(value)
            {
                StartMaskedColorStream();
            }
            else
            {
                StopMaskedColorStream();
            }
        }
    }

    private bool _isMaskedColorOn;

    public void StartMaskedColorStream()
    {
        // if(streamReader != null)
		// {
        //     maskedColorStream = streamReader.GetStream<MaskedColorStream>();
		// }
        if(maskedColorStream != null)
		{
			maskedColorStream.Start();
            _isMaskedColorOn = true;
		}
    }

	public void StopMaskedColorStream()
    {
        // if(streamReader != null)
		// {
        //     maskedColorStream = streamReader.GetStream<MaskedColorStream>();
		// }
        if(maskedColorStream != null)
		{
			maskedColorStream.Stop();
            _isMaskedColorOn = false;
		}
    }

    public bool IsColorizedBodyOn
    {
        get
        {
            return _isColorizedBodyOn;
        }
        set
        {
            if(value)
            {
                StartColorizedBodyStream();
            }
            else
            {
                StopColorizedBodyStream();
            }
        }
    }

    private bool _isColorizedBodyOn;

    public void StartColorizedBodyStream()
    {
        // if(streamReader != null)
		// {
        //     colorizedBodyStream = streamReader.GetStream<ColorizedBodyStream>();
		// }
        if(colorizedBodyStream != null)
		{
			colorizedBodyStream.Start();
            _isColorizedBodyOn = true;
		}
    }

	public void StopColorizedBodyStream()
	{
        // if(streamReader != null)
		// {
        //     colorizedBodyStream = streamReader.GetStream<ColorizedBodyStream>();
		// }
		if(colorizedBodyStream != null)
		{
			colorizedBodyStream.Stop();
            _isColorizedBodyOn = false;
		}
	}
    
    public bool IsBodyOn
    {
        get
        {
            return _isBodyOn;
        }
        set
        {
            if(value)
            {
                StartBodyStream();
            }
            else
            {
                StopBodyStream();
            }
        }
    }

    private bool _isBodyOn;

    public void StartBodyStream()
    {
        // if(streamReader != null)
		// {
        //     bodyStream = streamReader.GetStream<BodyStream>();
		// }
        if(bodyStream != null)
		{
			bodyStream.Start();
            _isBodyOn = true;
		}
    }

	public void StopBodyStream()
	{
        // if(streamReader != null)
		// {
        //     bodyStream = streamReader.GetStream<BodyStream>();
		// }
		if(bodyStream != null)
		{
			bodyStream.Stop();
            _isBodyOn = false;
		}
	}

    public void StartRecordDepth(string fileName)
    {
        if(depthStream != null)
        {
            depthStream.StartRecord(fileName);
        }
    }

    public void StopRecordDepth()
    {
        if(depthStream != null)
        {
            depthStream.StopRecord();
        }
    }

    public bool LdpEnable
    {
        set
        {
            depthStream.Stop();
            deviceController.EnableLdp(value);
            depthStream.Start();
        }
    }

    public bool LaserEnable
    {
        set
        {
            deviceController.EnableLaser(value);
        }
    }

    public USBInfo UsbInfo
    {
        get
        {
            return depthStream.usbInfo;
        }
    }

    public bool IsDepthMirroring
    {
        get
        {
            return depthStream.IsMirroring;
        }
        set
        {
            depthStream.IsMirroring = value;
        }
    }

    public bool IsColorMirroring
    {
        get
        {
            return colorStream.IsMirroring;
        }
        set
        {
            colorStream.IsMirroring = value;
        }
    }

    public void SetDefaultBodyFeatures(BodyTrackingFeatures features)
    {
        if(bodyStream != null)
        {
            bodyStream.SetDefaultBodyFeatures(features);
        }
    }

    public void SetSkeletonProfile(SkeletonProfile profile)
    {
        if(bodyStream != null)
        {
            bodyStream.SetSkeletonProfile(profile);
        }
    }

    public void SetSkeletonOptimization(SkeletonOptimization optimization)
    {
        if(bodyStream != null)
        {
            bodyStream.SetSkeletonOptimization(optimization);
        }
    }

    public UnityEvent OnInitializeSuccess = new UnityEvent();
    //Initialize failed event
    public UnityEvent OnInitializeFailed = new UnityEvent();
}
