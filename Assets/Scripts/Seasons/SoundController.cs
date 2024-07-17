using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private string _pathToSoundsFolder;

    private string _prevSoundName = "None";
    private Coroutine _cor;
    private float _initVolume = 1f;

    private void Start()
    {
        _initVolume = _audioSource.volume;
        SeasonsController.SeasonsUpdated += OnSeasonUpdated;
    }

    private void OnDestroy()
    {
        SeasonsController.SeasonsUpdated -= OnSeasonUpdated;
    }

    private void OnSeasonUpdated(SeasonsJson json)
    {
        if (json.voice == _prevSoundName)
            return;
        _prevSoundName = json.voice;

        if(_cor != null)
        {
            StopCoroutine(_cor);
        }
        _cor = StartCoroutine(UpdateSoundIE(json));
    }

    private IEnumerator UpdateSoundIE(SeasonsJson json)
    {
        _audioSource.DOFade(0f, SeasonsController.TransitionTime / 2);
        yield return new WaitForSeconds(SeasonsController.TransitionTime / 2);
        _audioSource.Stop();

        try
        {
            var clip = Resources.Load<AudioClip>($"{_pathToSoundsFolder}/{json.voice}");
            _audioSource.clip = clip;
        }
        catch
        {
            _audioSource.clip = null;
        }

        if(_audioSource.clip != null)
        {
            _audioSource.Play();
            _audioSource.DOFade(_initVolume, SeasonsController.TransitionTime / 2);
        }
    }
}
