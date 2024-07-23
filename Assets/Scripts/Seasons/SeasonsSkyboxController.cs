using System;
using UnityEngine;
using UnityEngine.Video;

public class SeasonsSkyboxController : BaseSeasonElementController
{
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private BlackScreen _blackScreen;
    [SerializeField] private string _resourcesPathToVideos;

    private string _prevVideo = "none";

    protected override void Start()
    {
        base.Start();
        _player.prepareCompleted += VideoPrepareCompleted;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _player.prepareCompleted -= VideoPrepareCompleted;
    }

    private void VideoPrepareCompleted(VideoPlayer source)
    {
        source.Play();
    }

    protected override void OnSeasonUpdated(SeasonsJson json)
    {
        if (json.VideoOnBack == _prevVideo)
            return;
        _prevVideo = json.VideoOnBack;

        VideoClip clip = null;

        try
        {
            clip = Resources.Load<VideoClip>($"{_resourcesPathToVideos}/{json.VideoOnBack}");
        }
        catch
        {
            Debug.LogError($"Can't find {json.VideoOnBack} video clip");
        }

        if (clip == null)
            return;

        _blackScreen.StartAnimation(SeasonsController.TransitionTime, () =>
        {
            _player.Stop();
            _player.clip = clip;
        });
    }
}
