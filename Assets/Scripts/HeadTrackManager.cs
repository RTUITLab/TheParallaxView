using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Video;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

// script for setting up ARKit for 3D head tracking purposes

public class HeadTrackManager : MonoBehaviour {

	private Vector3 posabs = Vector3.zero;
    private Quaternion qabs = new Quaternion(0, 0, 0, 0);
    private Vector3 eyeye = Vector3.zero;
    private Vector3 plusdif = Vector3.zero;

    private Vector3 laneTargetPos = Vector3.zero;
    [SerializeField] private float speed, laneSpeed;


[SerializeField]
	public GameObject headCenter;

    public GameObject CameraDetectPosition;
    public GameObject ScenePosition;
    public VideoPlayer VideoPlayer;

	//public Light keylight;
	public CameraManager camManager;

	Dictionary<string, float> currentBlendShapes;


	public string eyeInfoText; // little status, which eye is being tracked, auto or not
	public float IPD = 64f; // inter pupil distance (mm)
	public float EyeHeight = 32f; // eye height from head anchor (mm)

    
    private float screenWidth;
    private float screenHeight;
    public GameObject ball00, ballUp, ballRight;
    private float proportionW;
    private float proportionH;
    private float proportion;

    private float virtualScreenWidth;
    private float virtualScreenHeight;

    private float virtualctosX;
    private float virtualctosZ;
    private float virtualctosY;

    public string ARError;

	public void SetIPD( float value ) {
		IPD = value;
	}
		
	public void SetEyeHeight( float value ) {
		EyeHeight = value;
	}


	// Use this for initialization
	public void Start () {

		// first try to get camera acess
		//yield return RequestCamera ();

		ARError = null;


		Application.targetFrameRate = 60;
        //config.alignment = UnityARAlignment.UnityARAlignmentGravity; // using gravity alignment enables orientation (3DOF) tracking of device camera. we don't need it


        FindObjectOfType<AstraInputController>().onDetectBody += OnDetectBody;//register the callback to track the player position


       
        screenWidth =( Screen.width / Screen.dpi * 0.0254f) * 0.882f;
        screenHeight =( Screen.height / Screen.dpi * 0.0254f) * 0.887f;

        virtualScreenWidth = Math.Abs(ballRight.transform.position.x - ball00.transform.position.x);
        virtualScreenHeight = Math.Abs(ballUp.transform.position.y - ball00.transform.position.y);

        proportionW = virtualScreenWidth / screenWidth;
        proportionH = virtualScreenHeight / screenHeight;
        proportion = (proportionH + proportionW) / 2;

        Vector3 campos = CameraDetectPosition.transform.position;
        Vector3 scenepos = ScenePosition.transform.position;
        virtualctosX = (campos.x * proportion - virtualScreenWidth/2 - scenepos.x);
        virtualctosY = (campos.y * proportion - virtualScreenHeight/2 -  scenepos.y);
        virtualctosZ = (campos.z * proportion - scenepos.z);


    }

    void CatchARSessionFailed (string error) {
		//Debug.Log ("AR session failed. Error: " + error);
		ARError = error;
	}


	/* // this doesn't help at all
	IEnumerator RequestCamera() {
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
		if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {
			Debug.Log ("Camera granted");
		} else {
			Debug.Log ("Camera denied");
		}
	}
*/




	void FaceAdded()//ARFaceAnchor anchorData)
    {
        Vector3 pos = new Vector3(0, 0, 0);//UnityARMatrixOps.GetPosition (anchorData.transform));
        Quaternion rot = new Quaternion(0, 0, 0, 0);// UnityARMatrixOps.GetRotation (anchorData.transform);

if (camManager.DeviceCamUsed) {
			headCenter.transform.position = pos; // in device cam viewing mode, don't invert on x because this view is mirrored
			headCenter.transform.rotation = rot;
		} else {
			// invert on x because ARfaceAnchors are inverted on x (to mirror in display)
			headCenter.transform.position = new Vector3 (-pos.x, pos.y, pos.z); 
			headCenter.transform.rotation = new Quaternion( -rot.x, rot.y, rot.z, -rot.w); 
		}

		headCenter.SetActive (true);

		//currentBlendShapes = anchorData.blendShapes;
	}

	void FaceUpdated ()//ARFaceAnchor anchorData)
    {
        

        Vector3 pos = new Vector3(0, 0, 0);//UnityARMatrixOps.GetPosition (anchorData.transform));
        Quaternion rot = new Quaternion(0, 0, 0, 0);// UnityARMatrixOps.GetRotation (anchorData.transform);

		if (camManager.DeviceCamUsed) {
			headCenter.transform.position = pos; // in device cam viewing mode, don't invert on x because this view is mirrored
			headCenter.transform.rotation = rot;
		} else {
			// invert on x because ARfaceAnchors are inverted on x (to mirror in display)
			headCenter.transform.position = new Vector3 (-pos.x, pos.y, pos.z);
			headCenter.transform.rotation = new Quaternion( -rot.x, rot.y, rot.z, -rot.w);
		}
        if (Input.GetKeyDown(KeyCode.A))
        {
            eyeye = new Vector3(0.01f, 0, 0);
            plusdif += eyeye;


            //deviceCamera.transform.eulerAngles += new Vector3(0, -10, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            eyeye = new Vector3(-0.01f, 0, 0);
            plusdif += eyeye;

            //deviceCamera.transform.eulerAngles += new Vector3(0, -10, 0);
        }
		
        headCenter.transform.position += plusdif;
//currentBlendShapes = anchorData.blendShapes;

    }

	void FaceRemoved ()//ARFaceAnchor anchorData)
	{
		headCenter.SetActive (false);
		string str = "Lost Eye Tracking";
	}

	void FrameUpdate()//UnityARCamera cam)
	{
		//can't get the light direction estimate to work for some reason, it freezes the app
		//keylight.transform.rotation = Quaternion.FromToRotation(Vector3.back, cam.lightData.arDirectonalLightEstimate.primaryLightDirection); // <- probably incorrect way to do it
		//keylight.transform.rotation = Quaternion.LookRotation(cam.lightData.arDirectonalLightEstimate.primaryLightDirection); // <- probably correct way to do it
	}


	// Update is called once per frame
	void Update () {

        Vector3 pos = new Vector3(-0.034f, -0.041f, 0.042f);//UnityARMatrixOps.GetPosition (anchorData.transform));
        Quaternion rot = new Quaternion(0, 0, 0, 0);// UnityARMatrixOps.GetRotation (anchorData.transform);

        if (camManager.DeviceCamUsed)
        {
            headCenter.transform.position = pos; // in device cam viewing mode, don't invert on x because this view is mirrored
            headCenter.transform.rotation = rot;
        }
        else
        {
            // invert on x because ARfaceAnchors are inverted on x (to mirror in display)
            headCenter.transform.position = new Vector3(-pos.x, pos.y, pos.z);
            headCenter.transform.rotation = new Quaternion(-rot.x, rot.y, rot.z, -rot.w);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            eyeye = new Vector3(0.01f, 0, 0);
            plusdif += eyeye;


            //deviceCamera.transform.eulerAngles += new Vector3(0, -10, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            eyeye = new Vector3(-0.0347f, 0, 0);
            plusdif += eyeye;

            //deviceCamera.transform.eulerAngles += new Vector3(0, -10, 0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            eyeye = new Vector3(0f, 0.017f, 0);
            plusdif += eyeye;


            //deviceCamera.transform.eulerAngles += new Vector3(0, -10, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            eyeye = new Vector3(0, -0.01f, 0);
            plusdif += eyeye;

            //deviceCamera.transform.eulerAngles += new Vector3(0, -10, 0);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            eyeye = new Vector3(0f, 0, 0.6f);
            plusdif += eyeye;


            //deviceCamera.transform.eulerAngles += new Vector3(0, -10, 0);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            eyeye = new Vector3(0, 0, -0.1f);
            plusdif += eyeye;

            //deviceCamera.transform.eulerAngles += new Vector3(0, -10, 0);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            eyeye = new Vector3(0, 0, -0.01f);
            plusdif += eyeye;

            //deviceCamera.transform.eulerAngles += new Vector3(0, -10, 0);
        }

        //  float moveVal = Input.GetAxis("Horizontal");
        //  if (moveVal != 0)
        //      laneTargetPos = new Vector3(Mathf.Clamp(laneTargetPos.x + moveVal / 5, -0.75f, 0.75f), 0, 0);
        //  else
        //      laneTargetPos = headCenter.transform.localPosition;
        //  transform.Translate(transform.forward * Time.deltaTime * speed);
        //  headCenter.transform.localPosition = Vector3.MoveTowards(headCenter.transform.localPosition, laneTargetPos, Time.deltaTime * laneSpeed);
        //  headCenter.transform.Rotate(new Vector3(10, 0, 0));


        
        headCenter.transform.position += plusdif;
        //currentBlendShapes = anchorData.blendShapes;
}

    void OnDestroy()
	{
		
	}

    private static float xReg = 0f;
    private static float yReg = 0f;
    private static float zReg = 0f;

    // Пример использования
    private float kpX = 0.1f;  // Коэффициент пропорциональной части
    private float kiX = 0.00f;  // Коэффициент интегральной части
    private float kdX = 0.00f;  // Коэффициент дифференциальной части
     
    private float kpY = 0.1f;  // Коэффициент пропорциональной части
    private float kiY = 0.00f;  // Коэффициент интегральной части
    private float kdY = 0.00f;  // Коэффициент дифференциальной части
     
    private float kpSquare = 0.1f;  // Коэффициент пропорциональной части
    private float kiSquare = 0.00f;  // Коэффициент интегральной части
    private float kdSquare = 0.00f;  // Коэффициент дифференциальной части
     


    //process the player position
    public void OnDetectBody(bool status, Vector3 bodyPos)
    {
        
        float xPos = Mathf.Clamp(bodyPos.x * 1f , -15f, 15f);//clamp the ball position
        float yPos = Mathf.Clamp(bodyPos.y * 1f , -1.5f, 1.5f);
        float zPos = Mathf.Clamp(bodyPos.z * 1f , -15f, 15f);


        PIDController controllerX = new PIDController(kpX, kiX, kdX, 0);
        PIDController controllerY = new PIDController(kpY, kiY, kdY, 0);
        PIDController controllerZ = new PIDController(kpSquare, kiSquare, kdSquare, 0);

        Debug.Log(controllerX.compute(xReg) + " " + controllerX.compute(yReg) + " " + controllerX.compute(zReg));
        controllerX.updateSetpoint((xPos));
        xReg = xReg + (float)controllerX.compute(xReg);
        controllerY.updateSetpoint((yPos));
        yReg = yReg + (float)controllerY.compute(yReg);
        controllerZ.updateSetpoint((zPos));
        zReg = zReg + (float)controllerZ.compute(zReg);
        

        if (status)
            plusdif = new Vector3(xReg * proportion + virtualctosX, yReg * proportion - virtualctosY, -zReg*proportion + virtualctosZ);
        Debug.Log("REG: " + xReg + " "+ yReg + " "+ zReg);
        Debug.Log("POSPOSPOS: " + xPos + " " + yPos + " " + zPos);
        Debug.Log("Screen2: "+screenWidth + " " + screenHeight);
        Debug.Log("Proportion: " + proportion);
        Debug.Log("CtoS: " + virtualctosX + " "+ virtualctosY + " "+ virtualctosZ);
    }

}
