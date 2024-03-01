using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[ExecuteInEditMode]
public class PathVizualizer : MonoBehaviour
{
    [SerializeField] private CameraMover _cameraMover;
    [SerializeField] private LineRenderer _lineRenderer;

    private void LateUpdate()
    {
        if (_cameraMover == null || _lineRenderer == null)
        {
            Debug.LogWarning("Please, add a CameraMover and LineRenderer to the component in the inspector");
            return;
        }

        Vector3[] arr = new Vector3[_cameraMover.Points.Length];
        _lineRenderer.positionCount = arr.Length;

        for (int i = 0; i < _cameraMover.Points.Length; i++)
        {
            if (_cameraMover.Points[i] == null || _cameraMover.Points[i].IsNull)
            {
                _cameraMover.ReFindAllPoints();
                return;
            }
            arr[i] = _cameraMover.Points[i].Position;
        }

        _lineRenderer.SetPositions(arr);
    }
}
