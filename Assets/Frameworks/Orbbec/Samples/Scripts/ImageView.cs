using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using AstraSDK;

public class ImageView : MonoBehaviour
{
	public RawImage depthImage;
	public RawImage colorImage;
	public RawImage maskedColorImage;
	public RawImage colorizedBodyImage;

    // Use this for initialization
    void Awake()
    {
		StreamViewModel viewModel = StreamViewModel.Instance;
		viewModel.depthStream.onValueChanged += OnDepthStreamChanged;
		viewModel.colorStream.onValueChanged += OnColorStreamChanged;
		viewModel.colorizedBodyStream.onValueChanged += OnColorizedBodyStreamChanged;
		viewModel.maskedColorStream.onValueChanged += OnMaskedColorStreamChanged;

		depthImage.gameObject.SetActive(false);
		colorImage.gameObject.SetActive(false);
		colorizedBodyImage.gameObject.SetActive(false);
		maskedColorImage.gameObject.SetActive(false);
    }

	void Update()
	{
		depthImage.texture = AstraSDKManager.Instance.DepthTexture;
		colorImage.texture = AstraSDKManager.Instance.ColorTexture;
		colorizedBodyImage.texture = AstraSDKManager.Instance.ColorizedBodyTexture;
		maskedColorImage.texture = AstraSDKManager.Instance.MaskedColorTexture;
	}

    private void OnDepthStreamChanged(bool value)
    {
        if(value)
		{
			depthImage.gameObject.SetActive(true);
		}
		else
		{
			depthImage.gameObject.SetActive(false);
		}
    }

    private void OnColorStreamChanged(bool value)
    {
        if(value)
		{
			colorImage.gameObject.SetActive(true);
		}
		else
		{
			colorImage.gameObject.SetActive(false);
		}
    }

    private void OnColorizedBodyStreamChanged(bool value)
    {
        if(value)
		{
			colorizedBodyImage.gameObject.SetActive(true);
		}
		else
		{
			colorizedBodyImage.gameObject.SetActive(false);
		}
    }

    private void OnMaskedColorStreamChanged(bool value)
    {
        if(value)
		{
			maskedColorImage.gameObject.SetActive(true);
		}
		else
		{
			maskedColorImage.gameObject.SetActive(false);
		}
    }
}
