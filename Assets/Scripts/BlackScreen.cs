using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
    [SerializeField] private Image _image;

    private Coroutine _animCor;
    private Action _callback;

    public void StartAnimation(float transitionTime, Action fullBlackCallback)
    {
        if(_animCor != null)
        {
            StopCoroutine(_animCor);
        }
        _callback += fullBlackCallback;
        _animCor = StartCoroutine(AnimationIE(transitionTime));
    }

    private IEnumerator AnimationIE(float transitionTime)
    {
        var clr = _image.color;
        clr.a = 1;
        _image.DOColor(clr, transitionTime / 2f);

        yield return new WaitForSeconds(transitionTime / 2f);

        _callback?.Invoke();
        _callback = null;
        clr.a = 0;
        _image.DOColor(clr, transitionTime / 2f);
    }
}
