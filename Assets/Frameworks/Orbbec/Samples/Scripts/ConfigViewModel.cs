using UnityEngine;
using System.Collections;
using System;
using AstraSDK;

public class ConfigViewModel
{
    public struct ImageMode
    {
        public int width;
        public int height;

        public ImageMode(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    public Bindable<ImageMode> depthMode = new Bindable<ImageMode>();
    public Bindable<ImageMode> colorMode = new Bindable<ImageMode>();
    public Bindable<bool> depthMirror = new Bindable<bool>();
    public Bindable<bool> colorMirror = new Bindable<bool>();
    public Bindable<Astra.BodyTrackingFeatures> skeletonFeatures = new Bindable<Astra.BodyTrackingFeatures>();
    public Bindable<Astra.SkeletonProfile> skeletonProfile = new Bindable<Astra.SkeletonProfile>();
    public Bindable<Astra.SkeletonOptimization> skeletonOptimization = new Bindable<Astra.SkeletonOptimization>();

    private static ConfigViewModel _instance;
    public static ConfigViewModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ConfigViewModel();
            }
            return _instance;
        }
    }

    private ConfigViewModel()
    {
        depthMode.onValueChanged += OnDepthModeChanged;
        colorMode.onValueChanged += OnColorModeChanged;
        depthMirror.onValueChanged += OnDepthMirrorChanged;
        colorMirror.onValueChanged += OnColorMirrorChanged;
        skeletonFeatures.onValueChanged += OnSkeletonFeaturesChanged;
        skeletonProfile.onValueChanged += OnSkeletonProfileChanged;
        skeletonOptimization.onValueChanged += OnSkeletonOptimizationChanged;
    }

    private void OnDepthModeChanged(ImageMode imageMode)
    {
        Astra.ImageMode[] modes = AstraSDKManager.Instance.AvailableDepthModes;
        modes = Array.FindAll(modes, mode => mode.Width == imageMode.width && mode.Height == imageMode.height);
        if(modes != null && modes.Length > 0)
        {
            Array.Sort(modes, (x,y) => y.FramesPerSecond.CompareTo(x.FramesPerSecond));
            var mode = modes[0];
            AstraSDKManager.Instance.DepthMode = mode;
            Debug.Log(String.Format("Current Depth mode: {0}x{1}@{2}", mode.Width, mode.Height, mode.FramesPerSecond));
        }

        // foreach (var mode in modes)
        // {
        //     if (mode.Width == imageMode.width && mode.Height == imageMode.height)
        //     {
        //         AstraManager.Instance.DepthMode = mode;
        //         break;
        //     }
        // }
    }

    private void OnColorModeChanged(ImageMode imageMode)
    {
        Astra.ImageMode[] modes = AstraSDKManager.Instance.AvailableColorModes;
        modes = Array.FindAll(modes, mode => mode.Width == imageMode.width && mode.Height == imageMode.height && mode.PixelFormat == Astra.PixelFormat.RGB888);
        Array.Sort(modes, (x,y) => y.FramesPerSecond.CompareTo(x.FramesPerSecond));
        if(modes != null && modes.Length > 0)
        {
            Array.Sort(modes, (x,y) => y.FramesPerSecond.CompareTo(x.FramesPerSecond));
            var mode = modes[0];
            AstraSDKManager.Instance.ColorMode = mode;
            Debug.Log(String.Format("Current Color mode: {0}x{1}@{2}", mode.Width, mode.Height, mode.FramesPerSecond));
        }
        // foreach (var mode in modes)
        // {
        //     if (mode.Width == imageMode.width && mode.Height == imageMode.height)
        //     {
        //         AstraManager.Instance.ColorMode = mode;
        //         break;
        //     }
        // }
    }

    private void OnDepthMirrorChanged(bool isMirror)
    {
        AstraSDKManager.Instance.IsDepthMirroring = isMirror;
    }

    private void OnColorMirrorChanged(bool isMirror)
    {
        AstraSDKManager.Instance.IsColorMirroring = isMirror;
    }

    private void OnSkeletonFeaturesChanged(Astra.BodyTrackingFeatures features)
    {
        AstraSDKManager.Instance.SetDefaultBodyFeatures(features);
    }

    private void OnSkeletonProfileChanged(Astra.SkeletonProfile profile)
    {
        AstraSDKManager.Instance.SetSkeletonProfile(profile);
    }

    private void OnSkeletonOptimizationChanged(Astra.SkeletonOptimization optimization)
    {
        AstraSDKManager.Instance.SetSkeletonOptimization(optimization);
    }
}
