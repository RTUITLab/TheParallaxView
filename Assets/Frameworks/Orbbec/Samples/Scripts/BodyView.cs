using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using AstraSDK;

public class BodyView : MonoBehaviour
{
    public int bodyIndex;
    private Dictionary<Astra.JointType, GameObject> jointGOs;
    private LineRenderer[] jointLines;

    // Use this for initialization
    void Start()
    {
        StreamViewModel viewModel = StreamViewModel.Instance;
        viewModel.bodyStream.onValueChanged += OnBodyStreamChanged;

        jointGOs = new Dictionary<Astra.JointType, GameObject>();
        for (int i = 0; i < 19; ++i)
        {
            var jointGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            jointGO.name = ((Astra.JointType)i).ToString();
            jointGO.transform.localScale = new Vector3(100f, 100f, 100f);
            jointGO.transform.SetParent(transform, false);
            jointGO.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
            jointGO.SetActive(false);
            jointGOs.Add((Astra.JointType)i, jointGO);
        }

        jointLines = new LineRenderer[15];
        for (int i = 0; i < jointLines.Length; ++i)
        {
            var jointLineGO = new GameObject("Line");
            jointLineGO.transform.SetParent(transform, false);
            var jointLine = jointLineGO.AddComponent<LineRenderer>();
            jointLine.material = new Material(Shader.Find("Diffuse"));
            jointLine.SetWidth(0.1f, 0.1f);
            jointLineGO.SetActive(false);
            jointLines[i] = jointLine;
        }
    }

    private void OnBodyStreamChanged(bool value)
    {
        gameObject.SetActive(value);
    }

    // Update is called once per frame
    void Update()
    {
        if (!AstraSDKManager.Instance.Initialized || !AstraSDKManager.Instance.IsBodyOn)
        {
            return;
        }
        var bodies = AstraSDKManager.Instance.Bodies;
        if (bodies == null)
        {
            return;
        }
        int i = 0;
        foreach (var body in bodies)
        {
            if(body != null) i++;
            if(i != bodyIndex) continue;
            var joints = body.Joints;
            if (joints != null)
            {
                foreach (var joint in joints)
                {
                    if (joint.Status == Astra.JointStatus.Tracked)
                    {
                        var jointPos = joint.WorldPosition;
                        jointGOs[joint.Type].transform.localPosition = new Vector3(jointPos.X, jointPos.Y, jointPos.Z);
                        jointGOs[joint.Type].SetActive(true);

                        if (joint.Type == Astra.JointType.LeftHand)
                        {
                            if (body.HandPoseInfo.LeftHand == Astra.HandPose.Grip)
                            {
                                jointGOs[joint.Type].transform.localScale = new Vector3(100f, 100f, 100f) * 0.7f;
                                jointGOs[joint.Type].GetComponent<MeshRenderer>().material.color = Color.blue;
                            }
                            else
                            {
                                jointGOs[joint.Type].transform.localScale = new Vector3(100f, 100f, 100f);
                                jointGOs[joint.Type].GetComponent<MeshRenderer>().material.color = Color.white;
                            }
                        }
                        if (joint.Type == Astra.JointType.RightHand)
                        {
                            if (body.HandPoseInfo.RightHand == Astra.HandPose.Grip)
                            {
                                jointGOs[joint.Type].transform.localScale = new Vector3(100f, 100f, 100f) * 0.7f;
                                jointGOs[joint.Type].GetComponent<MeshRenderer>().material.color = Color.blue;
                            }
                            else
                            {
                                jointGOs[joint.Type].transform.localScale = new Vector3(100f, 100f, 100f);
                                jointGOs[joint.Type].GetComponent<MeshRenderer>().material.color = Color.white;
                            }
                        }
                    }
                    else
                    {
                        jointGOs[joint.Type].SetActive(false);
                    }
                }

                DrawLine(joints[0], joints[1], 0);
                DrawLine(joints[1], joints[2], 1);
                DrawLine(joints[1], joints[5], 2);
                DrawLine(joints[2], joints[3], 3);
                DrawLine(joints[3], joints[4], 4);
                DrawLine(joints[5], joints[6], 5);
                DrawLine(joints[6], joints[7], 6);
                DrawLine(joints[1], joints[8], 7);
                DrawLine(joints[8], joints[9], 8);
                DrawLine(joints[9], joints[10], 9);
                DrawLine(joints[9], joints[13], 10);
                DrawLine(joints[10], joints[11], 11);
                DrawLine(joints[11], joints[12], 12);
                DrawLine(joints[13], joints[14], 13);
                DrawLine(joints[14], joints[15], 14);

                break;
            }
        }
    }

    private void DrawLine(Astra.Joint startJoint, Astra.Joint endJoint, int index)
    {
        if (startJoint.Status == Astra.JointStatus.Tracked && endJoint.Status == Astra.JointStatus.Tracked)
        {
            var jointPos = startJoint.WorldPosition;
            var startPos = transform.TransformVector(jointPos.X, jointPos.Y, jointPos.Z);
            jointPos = endJoint.WorldPosition;
            var endPos = transform.TransformVector(jointPos.X, jointPos.Y, jointPos.Z);
            jointLines[index].SetPositions(new Vector3[] { startPos, endPos });
            jointLines[index].gameObject.SetActive(true);
        }
        else
        {
            jointLines[index].gameObject.SetActive(false);
        }
    }
}
