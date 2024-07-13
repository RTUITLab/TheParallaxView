using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonsTimeController : MonoBehaviour
{
    private string _previousTime = "None";

    private void Start()
    {
        SeasonsController.SeasonsUpdated += UpdateTime;
    }

    private void OnDestroy()
    {
        SeasonsController.SeasonsUpdated -= UpdateTime;
    }

    private void UpdateTime(SeasonsJson json)
    {
        if (json.time == _previousTime)
            return;

        _previousTime = json.time;
        //TODO: Later
    }
}
