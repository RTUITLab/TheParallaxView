using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeasonsTimeController : BaseSeasonElementController
{
    [Serializable]
    private class SeasonTime
    {
        public string TimeName;
        public Color SunColor;
        public Vector3 SunRotation;
        public float SunIntensity;
        public float SkyboxBrightness;
    }

    [SerializeField] private Light _sun;
    [SerializeField] private SunController _sunController;
    [SerializeField] private SeasonTime[] Times;

    private string _previousTime = "None";
    private Dictionary<string, SeasonTime> _times;
    private float _exposureValue = 1f, _exposureTarget = 1f;
    private bool _exposureAnimation = false;

    private const string MODIFIER_NAME = "SsnTime";

    protected override void Start()
    {
        base.Start();
        _times = new();
        foreach(var time in Times)
        {
            _times.Add(time.TimeName, time);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        RenderSettings.skybox.SetFloat("_Exposure", 1f);
    }

    private void Update()
    {
        if(_exposureAnimation)
        {
            _exposureValue = Mathf.Lerp(_exposureValue, _exposureTarget, Time.deltaTime);

            if(Mathf.Abs(_exposureTarget - _exposureValue) <= .03f)
            {
                _exposureAnimation = false;
                _exposureValue = _exposureTarget;
            }

            RenderSettings.skybox.SetFloat("_Exposure", _exposureValue);
        }
    }

    protected override void OnSeasonUpdated(SeasonsJson json)
    {
        if (json.time == _previousTime)
            return;

        _previousTime = json.time;

        if(!_times.ContainsKey(json.time))
        {
            Debug.LogError($"Time controller hasn't this name: {json.time}");
            return;
        }

        _times.TryGetValue(json.time, out var time);
        _sunController.ColorsAdd.Set(MODIFIER_NAME, time.SunColor);
        _sunController.Intensities.Set(MODIFIER_NAME, time.SunIntensity);
        _sunController.UpdateSun(SeasonsController.TransitionTime);
        _sun.transform.DORotate(time.SunRotation, SeasonsController.TransitionTime);
        _exposureTarget = time.SkyboxBrightness;
        _exposureAnimation = true;
    }
}
