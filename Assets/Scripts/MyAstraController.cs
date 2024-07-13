/* Created by: Alex Wang
 * 06/21/2019
 * MyAstraController is inherited from the AstraController in the Orbbec SDK.
 * It is responsible for initializing and configuring the Orbbec camera.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAstraController : AstraController {

    private void Awake()
    {
        //Initilialize the camera
        AstraUnityContext.Instance.Initialize();

        //Set the new license key 
        //Key received on 06/12/2019
        string licenseString = "X5pihnpPfAnvjAurgN0p3Ruuq0xBehzrvjEqtM5AKL6lZA1HQZ7hZhA3WkKd/2YRO3auQ25G3UQ1DfEr22lqB05hbWU9QmVubmV0dCBMYW5kbWFufE9yZz1WYW5kZXJiaWx0IFVuaXZlcnNpdHl8Q29tbWVudD18RXhwaXJhdGlvbj05OTk5OTk5OTk5";
        Astra.BodyTracking.SetLicense(licenseString);
        StartCoroutine(AwakeIE());
    }

    private IEnumerator AwakeIE()
    {
        yield return new WaitForSeconds(1);
        Screen.SetResolution(1920, 1080, true);
    }

}
