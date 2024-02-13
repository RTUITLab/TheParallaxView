using UnityEngine;
using System.Collections;
using System;
using Astra;
using AstraSDK;

public class StreamViewModel
{
    public Bindable<bool> depthStream = new Bindable<bool>();
    public Bindable<bool> colorStream = new Bindable<bool>();
    public Bindable<bool> bodyStream = new Bindable<bool>();
    public Bindable<bool> colorizedBodyStream = new Bindable<bool>();
    public Bindable<bool> maskedColorStream = new Bindable<bool>();
    public Bindable<bool> depthRecord = new Bindable<bool>();
    public Bindable<bool> ldpEnable = new Bindable<bool>();

    private static StreamViewModel _instance;
    public static StreamViewModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new StreamViewModel();
            }
            return _instance;
        }
    }

    private StreamViewModel()
    {
        depthStream.onValueChanged += OnDepthStreamChanged;
        colorStream.onValueChanged += OnColorStreamChanged;
        bodyStream.onValueChanged += OnBodyStreamChanged;
        colorizedBodyStream.onValueChanged += OnColorizedBodyStreamChanged;
        maskedColorStream.onValueChanged += OnMaskedColorStreamChanged;
        depthRecord.onValueChanged += OnDepthRecordChanged;
        ldpEnable.onValueChanged += OnLdpEnableChanged;
    }

    private void OnDepthStreamChanged(bool value)
    {
        AstraSDKManager.Instance.IsDepthOn = value;
    }

    private void OnColorStreamChanged(bool value)
    {
        AstraSDKManager.Instance.IsColorOn = value;
    }

    private void OnBodyStreamChanged(bool value)
    {
        AstraSDKManager.Instance.IsBodyOn = value;
    }

    private void OnColorizedBodyStreamChanged(bool value)
    {
        AstraSDKManager.Instance.IsColorizedBodyOn = value;
    }

    private void OnMaskedColorStreamChanged(bool value)
    {
        AstraSDKManager.Instance.IsMaskedColorOn = value;
    }

    private void OnLdpEnableChanged(bool value)
    {
        AstraSDKManager.Instance.LdpEnable = value;
    }

    private void OnDepthRecordChanged(bool value)
    {
        if (value)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var path = GetExternalStorageDirectory();
            AstraSDKManager.Instance.StartRecordDepth(path + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".oni");
#else
            AstraSDKManager.Instance.StartRecordDepth(DateTime.Now.ToString("yyyyMMddHHmmss") + ".oni");
#endif
        }
        else
        {
            AstraSDKManager.Instance.StopRecordDepth();
        }
    }

    private String GetExternalStorageDirectory()
    {
        string path = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass ajc = new AndroidJavaClass("android.os.Environment"))
                {
                    path = ajc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error fetching native Android internal storage dir: " + e.Message);
            }
        }
        return path;
    }
}
