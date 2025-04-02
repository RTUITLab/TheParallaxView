using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretItem : MonoBehaviour
{
    [field: SerializeField] public Vector3 BoxSize { get; private set; }

    private SecretItemsController _controller;
    private Transform _camera;
    private float _lookingTime = 0f;

    private Vector3[] _origins;

    public bool Finded { get; private set; } = false;

    private void Start()
    {
        _controller = SecretItemsController.Instance;
        _camera = _controller.Camera;
        _controller.AddSecretItem(this);

        _origins = CreateOrigins();
    }

    private Vector3[] CreateOrigins()
    {
        var list = new List<Vector3>
        {
            Vector3.zero
        };

        for (float x = -BoxSize.x / 2f; x <= BoxSize.x / 2f; x += 1f)
        {
            for (float y = -BoxSize.y / 2f; y <= BoxSize.y / 2f; y += 1f)
            {
                for (float z = -BoxSize.z / 2f; z <= BoxSize.z / 2f; z += 1f)
                {
                    list.Add(new(x, y, z));
                }
            }
        }

        return list.ToArray();
    }

    private void FixedUpdate()
    {
        if(IsCameraRotatedToItem() && IsCameraLookingOnItem() && !Finded)
        {
            _lookingTime += Time.fixedDeltaTime;
            if(_lookingTime >= _controller.SearchTime)
            {
                _lookingTime = 0f;
                Find();
            }
        }
        else
        {
            _lookingTime = 0f;
        }
    }

    public bool IsCameraLookingOnItem()
    {
        foreach (var originDelta in _origins)
        {
            var origin = transform.position + originDelta;
            var direction = _camera.position - origin;
            var ray = new Ray(origin, direction);
            var distance = Vector3.Distance(origin, _camera.position);
            distance = Mathf.Min(distance, _controller.SearchDistance);

            if (Physics.Raycast(ray, out _, distance, _controller.SearchRayMask))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsCameraRotatedToItem()
    {
        var delta = transform.position - _camera.position;
        var angle = Vector3.Angle(_camera.forward, delta);
        return Mathf.Abs(angle) <= _controller.SearchAngle;
    }

    public void Find()
    {
        if (Finded)
            return;

        Finded = true;
        StartCoroutine(FindAnimIE());
    }

    private IEnumerator FindAnimIE()
    {
        var pos = transform.position;
        var rot = transform.rotation;
        var scale = transform.localScale;

        transform.DOMoveY(pos.y + BoxSize.y / 2f, .5f);
        transform.DORotate((rot * Quaternion.Euler(0, 180, 0)).eulerAngles, .5f);
        transform.DOScale(scale * 1.3f, .5f);

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);

        transform.position = pos;
        transform.rotation = rot;
        transform.localScale = scale;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        Finded = false;
        _lookingTime = 0f;
    }
}
