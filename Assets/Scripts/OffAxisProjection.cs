﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script is for setting up the non-symmetric camera frustom and compute the off-axis projection matrix used for the eye camera

[ExecuteInEditMode]
public class OffAxisProjection : MonoBehaviour
{
	public Camera deviceCamera;
	public Camera eyeCamera;
	public Transform lookPoint;
    public Transform cameraParent;

    private Camera copyeye;
	private Vector3 eyeye = Vector3.zero;

	public LineRenderer lineRenderer;

	public float horizontal, vertical, near, far;

	public CameraManager camManager;

	public float nearDist;


void LateUpdate()
	{
       
		//Debug.Log("X1: "+eyeCamera.transform.position.x);

        Quaternion q = deviceCamera.transform.rotation * Quaternion.Euler(Vector3.up * 180);
		eyeCamera.transform.rotation = q;

		Vector3 fwd = eyeCamera.transform.forward;
		//Plane device_plane = new Plane(fwd, -lookPoint);
        Plane device_plane = new Plane(fwd, lookPoint.localPosition);

        //Vector3 deviceCamPos = eyeCamera.transform.worldToLocalMatrix.MultiplyPoint( deviceCamera.transform.position ); // find device camera in rendering camera's view space
        //Vector3 fwd = eyeCamera.transform.worldToLocalMatrix.MultiplyVector (deviceCamera.transform.forward); // normal of plane defined by device camera
        //Plane device_plane = new Plane( fwd, deviceCamPos);

        var localEyeCamera = eyeCamera.transform.position - cameraParent.position;
        Vector3 close = device_plane.ClosestPointOnPlane (localEyeCamera);
		near = (close - localEyeCamera).magnitude;

		// couldn't get device orientation to work properly in all cases, so just landscape for now (it's just the UI that is locked to landscape, everyting else works just fine)
		/*if (Screen.orientation == ScreenOrientation.Portrait) { 
			left = trackedCamPos.x - 0.040f; // portrait iphone X
			right = trackedCamPos.x + 0.022f;
			top = trackedCamPos.y + 0.000f;
			bottom = trackedCamPos.y - 0.135f;
		} else {*/

		var pos = eyeCamera.transform.position;
		var hor = eyeCamera.transform.right * horizontal;
		var ver = eyeCamera.transform.up * vertical;

		Vector3 topLeft = pos - hor + ver;
		Vector3 bottomLeft = pos - hor - ver;
		Vector3 topRight = pos + hor + ver;
		Vector3 bottomRight = pos + hor - ver;

		if (lineRenderer != null && camManager != null) {
			if (camManager.EyeCamUsed) {
				lineRenderer.enabled = false;
			} else {

				// visualise frustum. or more exactly, the 4 sided pyramid in front of the actual frustum
				lineRenderer.enabled = true;

				Vector3 w_topLeft = eyeCamera.transform.localToWorldMatrix.MultiplyPoint (topLeft);
				Vector3 w_topRight = eyeCamera.transform.localToWorldMatrix.MultiplyPoint (topRight);
				Vector3 w_bottomLeft = eyeCamera.transform.localToWorldMatrix.MultiplyPoint (bottomLeft);
				Vector3 w_bottomRight = eyeCamera.transform.localToWorldMatrix.MultiplyPoint (bottomRight);

				if (camManager.DeviceCamUsed) {
					// flip on x. still a bit unsure what is correct here (in device cam mirrored view) but I like this visualisation.
					w_topLeft = eyeCamera.transform.localToWorldMatrix.MultiplyPoint (new Vector3 (topLeft.x, topLeft.y, topLeft.z));
					w_topRight = eyeCamera.transform.localToWorldMatrix.MultiplyPoint (new Vector3 (topRight.x, topRight.y, topRight.z));
					w_bottomLeft = eyeCamera.transform.localToWorldMatrix.MultiplyPoint (new Vector3 (bottomLeft.x, bottomLeft.y, bottomLeft.z));
					w_bottomRight = eyeCamera.transform.localToWorldMatrix.MultiplyPoint (new Vector3 (bottomRight.x, bottomRight.y, bottomRight.z));
				}

				lineRenderer.SetPosition (0, eyeCamera.transform.position);
				lineRenderer.SetPosition (1, w_topLeft);
				lineRenderer.SetPosition (2, w_topRight);
				lineRenderer.SetPosition (3, eyeCamera.transform.position);
				lineRenderer.SetPosition (4, w_bottomLeft);
				lineRenderer.SetPosition (5, w_bottomRight);
				lineRenderer.SetPosition (6, eyeCamera.transform.position);
			} 
		}

		nearDist = near;

		// move near to 0.01 (1 cm from eye)
		float scale_factor = 0.01f / near;
		near *= scale_factor;

		var forward = eyeCamera.transform.forward;
		var rightVec = eyeCamera.transform.right;
		var up = eyeCamera.transform.up;
		var delta = lookPoint.position - eyeCamera.transform.position;

        var basedVec = new Vector3(rightVec.x * delta.x + up.x * delta.y + forward.x * delta.z,
                                   rightVec.y * delta.x + up.y * delta.y + forward.y * delta.z,
                                   rightVec.z * delta.x + up.z * delta.y + forward.z * delta.z);
		float left = basedVec.x - horizontal;
		float right = basedVec.x + horizontal;
		float top = basedVec.y + vertical;
		float bottom = basedVec.y - vertical;
		left *= scale_factor;
		right *= scale_factor;
		top *= scale_factor;
		bottom *= scale_factor;

		Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, near, far);
		eyeCamera.projectionMatrix = m;
	}

	static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
	{
		float x = 2.0F * near / (right - left);
		float y = 2.0F * near / (top - bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0F * far * near) / (far - near);
		float e = -1.0F;
		Matrix4x4 m = new Matrix4x4();
		m[0, 0] = x;
		m[0, 1] = 0;
		m[0, 2] = a;
		m[0, 3] = 0;
		m[1, 0] = 0;
		m[1, 1] = y;
		m[1, 2] = b;
		m[1, 3] = 0;
		m[2, 0] = 0;
		m[2, 1] = 0;
		m[2, 2] = c;
		m[2, 3] = d;
		m[3, 0] = 0;
		m[3, 1] = 0;
		m[3, 2] = e;
		m[3, 3] = 0;
		return m;
	}
}