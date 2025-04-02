using UnityEngine;

[ExecuteInEditMode]
public class PathVizualizer : MonoBehaviour
{
    [SerializeField] private CameraMover _cameraMover;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Material _defaultMaterial, _stopMaterial;

    private void LateUpdate()
    {
        if (_cameraMover == null || _lineRenderer == null)
        {
            Debug.LogWarning("Please, add a CameraMover and LineRenderer to the component in the inspector");
            return;
        }

        Vector3[] arr = new Vector3[_cameraMover.Points.Length];
        _lineRenderer.positionCount = arr.Length;

        var points = _cameraMover.Points;

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null || points[i].IsNull)
            {
                _cameraMover.ReFindAllPoints();
                return;
            }

            PaintPoint(points[i]);
            arr[i] = points[i].Position;
        }

        _lineRenderer.SetPositions(arr);
    }

    private void PaintPoint(CameraMover.CameraPoint point)
    {
        if (_defaultMaterial == null || _stopMaterial == null)
            return;

        if(point.isStop)
        {
            point.Renderer.sharedMaterial = _stopMaterial;
        }
        else
        {
            point.Renderer.sharedMaterial = _defaultMaterial;
        }
    }
}
