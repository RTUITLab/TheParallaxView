using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AstraInputController : MonoBehaviour{

    private long _lastFrameIndex = -1;
    public System.Action<bool, Vector3> onDetectBody;//this callback will be called each time the body stream update.
    private Vector3 lastChestPost;
    private Astra.Body[] _bodies = new Astra.Body[Astra.BodyFrame.MaxBodies];
    public float Horizontal
    {
        get { return lastChestPost.x; }
    }
    //This function will receive the skeleton data from the AstraController.cs 
    public void OnNewFrame(Astra.BodyStream bodyStream, Astra.BodyFrame frame)
    {
        if (frame.Width == 0 || frame.Height == 0) return;
        if (_lastFrameIndex == frame.FrameIndex) return;
        _lastFrameIndex = frame.FrameIndex;
        frame.CopyBodyData(ref _bodies);
        if (_bodies != null && _bodies.Length > 0 && _bodies[0] != null && _bodies[0].Joints != null)
        {
            lastChestPost = GetJointWorldPos(_bodies[0].Joints[(int)Astra.JointType.ShoulderSpine]);
            if (onDetectBody != null)
                onDetectBody(true, lastChestPost);
        }
        else
        {
            lastChestPost = new Vector3(0, lastChestPost.y, lastChestPost.z);
            if (onDetectBody != null)
                onDetectBody(false, lastChestPost);
        }
        Debug.Log("Last : "+lastChestPost);
    }
    //convert from millimeters to meters
    private Vector3 GetJointWorldPos(Astra.Joint joint)
    {
        return new Vector3(joint.WorldPosition.X / 1000f, joint.WorldPosition.Y / 1000f, joint.WorldPosition.Z / 1000);
    }
}
