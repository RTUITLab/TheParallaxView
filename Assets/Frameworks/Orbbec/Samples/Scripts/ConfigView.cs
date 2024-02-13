using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Astra;
using System;
using AstraSDK;

public class ConfigView : MonoBehaviour
{
    public ObToggle depthMirror;
    public ObToggle colorMirror;
    public ObToggle[] depthModeToggles;
    public ObToggle[] colorModeToggles;
    public ObToggle[] skeletonFeaturesToggles;
    public ObToggle[] skeletonProfileToggles;
    public ObToggle[] skeletonOptimizationToggles;
    public ObButton recordDepthButton;

    // Use this for initialization
    void Awake()
    {
        depthMirror.onValueChanged.AddListener((value) =>
        {
            ConfigViewModel.Instance.depthMirror.Value = value;
            depthMirror.OnOff(value);
        });
        colorMirror.onValueChanged.AddListener((value) =>
        {
            ConfigViewModel.Instance.colorMirror.Value = value;
            colorMirror.OnOff(value);
        });

        depthModeToggles[0].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.depthMode.Value = new ConfigViewModel.ImageMode(160, 120);
            }
            depthModeToggles[0].OnOff(value);
        });
        depthModeToggles[1].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.depthMode.Value = new ConfigViewModel.ImageMode(320, 240);
            }
            depthModeToggles[1].OnOff(value);
        });
        depthModeToggles[2].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.depthMode.Value = new ConfigViewModel.ImageMode(640, 480);
            }
            depthModeToggles[2].OnOff(value);
        });
        depthModeToggles[3].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.depthMode.Value = new ConfigViewModel.ImageMode(640, 400);
            }
            depthModeToggles[3].OnOff(value);
        });

        colorModeToggles[0].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.colorMode.Value = new ConfigViewModel.ImageMode(320, 240);
            }
            colorModeToggles[0].OnOff(value);
        });
        colorModeToggles[1].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.colorMode.Value = new ConfigViewModel.ImageMode(640, 480);
            }
            colorModeToggles[1].OnOff(value);
        });
        colorModeToggles[2].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.colorMode.Value = new ConfigViewModel.ImageMode(1280, 720);
            }
            colorModeToggles[2].OnOff(value);
        });
        colorModeToggles[3].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.colorMode.Value = new ConfigViewModel.ImageMode(1920, 1080);
            }
            colorModeToggles[3].OnOff(value);
        });

        skeletonFeaturesToggles[0].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.skeletonFeatures.Value = Astra.BodyTrackingFeatures.Segmentation;
            }
            skeletonFeaturesToggles[0].OnOff(value);
        });
        skeletonFeaturesToggles[1].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.skeletonFeatures.Value = Astra.BodyTrackingFeatures.Skeleton;
            }
            skeletonFeaturesToggles[1].OnOff(value);
        });
        skeletonFeaturesToggles[2].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.skeletonFeatures.Value = Astra.BodyTrackingFeatures.HandPose;
            }
            skeletonFeaturesToggles[2].OnOff(value);
        });

        skeletonProfileToggles[0].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.skeletonProfile.Value = Astra.SkeletonProfile.Full;
            }
            skeletonProfileToggles[0].OnOff(value);
        });
        skeletonProfileToggles[1].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.skeletonProfile.Value = Astra.SkeletonProfile.Basic;
            }
            skeletonProfileToggles[1].OnOff(value);
        });
        skeletonProfileToggles[2].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.skeletonProfile.Value = Astra.SkeletonProfile.UpperBody;
            }
            skeletonProfileToggles[2].OnOff(value);
        });

        skeletonOptimizationToggles[0].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.skeletonOptimization.Value = Astra.SkeletonOptimization.BestAccuracy;
            }
            skeletonOptimizationToggles[0].OnOff(value);
        });
        skeletonOptimizationToggles[1].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.skeletonOptimization.Value = Astra.SkeletonOptimization.Balanced;
            }
            skeletonOptimizationToggles[1].OnOff(value);
        });
        skeletonOptimizationToggles[2].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ConfigViewModel.Instance.skeletonOptimization.Value = Astra.SkeletonOptimization.MinimizeMemory;
            }
            skeletonOptimizationToggles[2].OnOff(value);
        });

        recordDepthButton.onClick.AddListener(() =>
        {
            StreamViewModel.Instance.depthRecord.Value = !StreamViewModel.Instance.depthRecord.Value;
            recordDepthButton.OnOff(StreamViewModel.Instance.depthRecord.Value);
        });

        AstraSDKManager.Instance.OnInitializeFailed.AddListener(() =>
        {
            depthMirror.interactable = false;
            colorMirror.interactable = false;
            foreach (var toggle in depthModeToggles)
            {
                toggle.interactable = false;
            }
            foreach (var toggle in colorModeToggles)
            {
                toggle.interactable = false;
            }
            foreach (var toggle in skeletonFeaturesToggles)
            {
                toggle.interactable = false;
            }
            foreach (var toggle in skeletonProfileToggles)
            {
                toggle.interactable = false;
            }
            foreach (var toggle in skeletonOptimizationToggles)
            {
                toggle.interactable = false;
            }
            recordDepthButton.interactable = false;
        });

        AstraSDKManager.Instance.OnInitializeSuccess.AddListener(() =>
        {
            var pid = AstraSDKManager.Instance.UsbInfo.Pid;
            if (pid == Constant.BUS_CL_PID)
            {
                colorMirror.interactable = false;
                foreach (var toggle in colorModeToggles)
                {
                    toggle.interactable = false;
                }
            }

            ConfigViewModel.Instance.depthMode.Value = new ConfigViewModel.ImageMode(640, 480);
            ConfigViewModel.Instance.depthMirror.Value = true;
            if (pid != Constant.BUS_CL_PID)
            {
                ConfigViewModel.Instance.colorMode.Value = new ConfigViewModel.ImageMode(640, 480);
                ConfigViewModel.Instance.colorMirror.Value = true;
            }
            ConfigViewModel.Instance.skeletonFeatures.Value = Astra.BodyTrackingFeatures.HandPose;
            ConfigViewModel.Instance.skeletonProfile.Value = Astra.SkeletonProfile.Full;
            ConfigViewModel.Instance.skeletonOptimization.Value = Astra.SkeletonOptimization.Balanced;
        });
    }
}
