using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [System.Serializable]
    public class CameraPoint
    {
        [SerializeField] private GameObject _point;
        [Range(.1f, 10)] public float speed = 1;
        public bool isStop = false;
        [Range(0, 60)]
        [SerializeField] private float _stopTime = 0;

        public Vector3 Position => _point.transform.position;
        public Quaternion Rotation { get; private set; }
        public bool IsNull => _point == null;
        public float StopTime => _stopTime;

        public CameraPoint(GameObject obj, float speed)
        {
            _point = obj;
            this.speed = speed;
        }

        public void Activate(bool value, Vector3 offset)
        {
            _point.SetActive(value);
            _point.transform.position -= offset;
            //var localEulers = _point.transform.localRotation.eulerAngles;
            //localEulers = new Vector3(localEulers.z, localEulers.y, localEulers.x);
            //_point.transform.localRotation = Quaternion.Euler(localEulers);
            Rotation = Quaternion.Euler(_point.transform.rotation.eulerAngles * -1);
            Rotation = Quaternion.Inverse(_point.transform.rotation);
        }
    }

    [SerializeField] private Transform _toMove;
    [SerializeField] private CameraPoint[] _points;
    [SerializeField] private bool _hidePointsOnStart;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Transform _world;
    [SerializeField] private Transform _worldCameraPoint;

    private int _currentPosIndex = 0;
    private Vector3 _nextPos = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;
    private Quaternion _prevRotation = Quaternion.identity;
    private float _maxDiff = -1;
    private float _speed = 0;
    private float _minDiff = -1;

    private Coroutine _waitTimeCor = null;

    public CameraPoint[] Points => _points;

    PIDController xPosController = new PIDController(.05f, 0f, 0f, 0f);
    PIDController yPosController = new PIDController(.05f, 0f, 0f, 0f);
    PIDController zPosController = new PIDController(.05f, 0f, 0f, 0f);

    private void Start()
    {
        foreach(var point in _points)
        {
            point.Activate(!_hidePointsOnStart, _positionOffset);
        }
        StartWay();
    }

    public void StartWay()
    {
        _currentPosIndex = -1;
        _nextPos = _toMove.position;
        _rotation = _toMove.rotation;
        ToNextPath();
    }

    public void ToNextPath()
    {
        _currentPosIndex = (_currentPosIndex + 1) % _points.Length;
        _nextPos = _points[_currentPosIndex].Position;
        _speed = _points[_currentPosIndex].speed;

        _prevRotation = _rotation;
        _rotation = _points[_currentPosIndex].Rotation;
        _rotation = new Quaternion(-_rotation.x, -_rotation.y, -_rotation.z, -_rotation.w);

        _maxDiff = (_worldCameraPoint.position - _nextPos).magnitude;
        _minDiff = _maxDiff + 1;
    }

    private void TryToNextPath()
    {
        if (_points[_currentPosIndex].IsNull)
        {
            ReFindAllPoints();
            _currentPosIndex = 0;
        }

        if (!_points[_currentPosIndex].isStop)
        {
            ToNextPath();
        }
        else
        {
            _speed = 0;
            _worldCameraPoint.position = _nextPos;
            _world.rotation = _rotation;

            if (_points[_currentPosIndex].StopTime > 0)
            {
                if(_waitTimeCor != null)
                {
                    StopCoroutine(_waitTimeCor);
                }
                _waitTimeCor = StartCoroutine(WaitTimeIE(_points[_currentPosIndex].StopTime));
            }
        }
    }

    private IEnumerator WaitTimeIE(float time)
    {
        yield return new WaitForSeconds(time);
        ToNextPath();
    }

    public void AddPointToPath(GameObject point)
    {
        CameraPoint[] newPoints = new CameraPoint[_points.Length + 1];
        for(int i = 0; i < _points.Length; i++)
        {
            newPoints[i] = _points[i];
        }
        newPoints[^1] = new CameraPoint(point, newPoints[^2].speed);
        _points = newPoints;
    }

    public void ReFindAllPoints()
    {
        List<CameraPoint> points = new();
        for(int i = 0; i < _points.Length; i++)
        {
            if (_points[i] != null && !_points[i].IsNull)
            {
                points.Add(_points[i]);
            }
        }
        _points = points.ToArray();
    }

    private float RotationFunc(float x)
    {
        return Mathf.Sin(x * Mathf.PI - Mathf.PI / 2) / 2 + .5f;
    }

    private void Update()
    {
        if (_speed > 0)
        {
            var diff = _nextPos - _worldCameraPoint.transform.position;
            if (diff.magnitude < .1f || diff.magnitude > _minDiff + 1)
            {
                TryToNextPath();
            }
            else
            {
                _minDiff = diff.magnitude;
                _worldCameraPoint.position += _velocity * Time.deltaTime;
                float mult = (_maxDiff - _minDiff) / _maxDiff;
                var oldPos = _worldCameraPoint.position;
                _world.rotation = Quaternion.Lerp(_prevRotation, _rotation, RotationFunc(mult));
                var deltaVec = _worldCameraPoint.position - oldPos;
                _toMove.position += deltaVec;

                xPosController.updateSetpoint(_worldCameraPoint.position.x);
                yPosController.updateSetpoint(_worldCameraPoint.position.y);
                zPosController.updateSetpoint(_worldCameraPoint.position.z);

                Vector3 newPos = _toMove.position;
                newPos.x += xPosController.compute(newPos.x);
                newPos.y += yPosController.compute(newPos.y);
                newPos.z += zPosController.compute(newPos.z);

                _toMove.position = newPos;

                //_toMove.position = _worldCameraPoint.position;
                _nextPos = _points[_currentPosIndex].Position;
                _velocity = _nextPos - _worldCameraPoint.position;
                _velocity = _velocity.normalized * _speed;
            }
        }
    }
}
