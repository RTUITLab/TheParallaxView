using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AstraSDK;

public class StreamView : MonoBehaviour
{
    public ObButton depthButton;
    public ObButton colorButton;
    public ObButton bodyButton;
    public ObButton maskedColorButton;
    public ObButton colorizedBodyButton;
    public ObButton ldpButton;
    public ObButton correctButton;

    void Awake()
    {
        StreamViewModel viewModel = StreamViewModel.Instance;

        depthButton.onClick.AddListener(() =>
        {
            viewModel.depthStream.Value = !viewModel.depthStream.Value;
            depthButton.OnOff(viewModel.depthStream.Value);
        });
        colorButton.onClick.AddListener(() =>
        {
            viewModel.colorStream.Value = !viewModel.colorStream.Value;
            colorButton.OnOff(viewModel.colorStream.Value);
        });
        bodyButton.onClick.AddListener(() =>
        {
            viewModel.bodyStream.Value = !viewModel.bodyStream.Value;
            bodyButton.OnOff(viewModel.bodyStream.Value);
        });
        maskedColorButton.onClick.AddListener(() =>
        {
            viewModel.maskedColorStream.Value = !viewModel.maskedColorStream.Value;
            maskedColorButton.OnOff(viewModel.maskedColorStream.Value);
        });
        colorizedBodyButton.onClick.AddListener(() =>
        {
            viewModel.colorizedBodyStream.Value = !viewModel.colorizedBodyStream.Value;
            colorizedBodyButton.OnOff(viewModel.colorizedBodyStream.Value);
        });
        ldpButton.onClick.AddListener(()=>{
            viewModel.ldpEnable.Value = !viewModel.ldpEnable.Value;
            ldpButton.OnOff(viewModel.ldpEnable.Value);
        });
        correctButton.onClick.AddListener(()=>{
            AstraSDKManager.Instance.CorrectBody = !AstraSDKManager.Instance.CorrectBody;
            correctButton.OnOff(AstraSDKManager.Instance.CorrectBody);
        });

        AstraSDKManager.Instance.OnInitializeFailed.AddListener(() =>
        {
            depthButton.interactable = false;
            colorButton.interactable = false;
            bodyButton.interactable = false;
            maskedColorButton.interactable = false;
            colorizedBodyButton.interactable = false;
        });

        AstraSDKManager.Instance.OnInitializeSuccess.AddListener(() =>
        {
            viewModel.depthStream.Value = true;
            depthButton.OnOff(viewModel.depthStream.Value);

            var pid = AstraSDKManager.Instance.UsbInfo.Pid;
            if (pid == Constant.BUS_CL_PID)
            {
                colorButton.interactable = false;
                maskedColorButton.interactable = false;
                colorizedBodyButton.interactable = false;
            }
            else
            {
                viewModel.colorStream.Value = true;
                colorButton.OnOff(viewModel.colorStream.Value);
            }
        });
    }
}
