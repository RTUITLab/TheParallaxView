using System;
using System.Collections;
using UnityEngine;

public class SeasonsController : MonoBehaviour
{
    [SerializeField] private float _updateTime;
    [SerializeField] private string _weather;
    [SerializeField] private float _transitionTime;

    public static event Action<SeasonsJson> SeasonsUpdated;
    public static float TransitionTime { get; private set; } = 1f;

    private Coroutine _startCor;

    private void Start()
    {
        TransitionTime = _transitionTime;
        _startCor = StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while(true)
        {
            SeasonsJson seasons = new();
            //yield return ServerConnection.SendRequest("Get seasons", seasons);
            //For test
            yield return ServerConnection.SendRequest(_weather, seasons);
            SeasonsUpdated?.Invoke(seasons);
            yield return new WaitForSeconds(_updateTime);
        }
    }

    private void OnDestroy()
    {
        if(_startCor != null)
        {
            StopCoroutine(_startCor);
        }
    }
}
