using System;
using System.Collections;
using UnityEngine;

public class SeasonsController : MonoBehaviour
{
    [SerializeField] private float _updateTime;
    [SerializeField] private float _transitionTime;

    public static event Action<SeasonsJson> SeasonsUpdated;
    public static float TransitionTime { get; private set; } = 1f;
    public const string SERVER_MESSAGE = "GetWeather";

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
            yield return ServerConnection.SendRequest(SERVER_MESSAGE, seasons);
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
