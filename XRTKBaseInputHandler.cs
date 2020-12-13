using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class InputBaseHandler : MonoBehaviour
{
    #region Variable Declarations
    public float triggerSensitivity = 0.3f;
    #region Variables Necessary To Get XR Input

    //XRRig based declarations for internal usage
    XRRig xrRig;
    XRRayInteractor rightController, leftController;
    GameObject rightControllerMR, leftControllerMR;

    


    //List for devices with Primary2dAxis (Right Thumb Stick) and Secondary2dAxis (Left Thumb Stick) - Just in case if there is more than one same type controller
    private List<InputDevice> devicesWithPrimary2DAxis;
    #endregion

    #region Public variables for other classes to catch input

    // Raw Values (for more control on input)
    // 2d Axis float values ranges from -1 to +1
    private Vector2 rightAxis = Vector2.zero;
    private Vector2 leftAxis = Vector2.zero;

    // XRRig Based Input Parameter Flags

    private bool rightTriggerFlag;
    private bool LeftTriggerFlag;

    private bool rightGripFlag;
    private bool LeftGripFlag;

    private bool rightPrimaryButtonFlag;
    private bool leftPrimaryButtonFlag;

    private bool rightSecondaryButtonFlag;
    private bool leftSecondaryButtonFlag;


    #endregion

    #endregion

    private void Awake()
    {
        // Finding, Assigning and Reading the Controller Interactions
        devicesWithPrimary2DAxis = new List<InputDevice>();
    }


    // Start is called before the first frame update
    void OnEnable()
    {
        if (FindObjectOfType<XRRig>() != null)
        {
            // Execution only starts if the scene has a XR Rig
            xrRig = FindObjectOfType<XRRig>();

            // Finding, Assigning and Reading the Controller Interactions
            devicesWithPrimary2DAxis = new List<InputDevice>();

            // List to get all devices and then all right and left devices are assigned respectively
            List<InputDevice> allDevices = new List<InputDevice>();

            // Asks XR Interaction subsystems and fetches all available devices - after that they are separated
            InputDevices.GetDevices(allDevices);

            // All detected devices are to be registed for monitoring their connection status and for use (to avoid null ref errors incase they are disconnected)
            foreach (InputDevice device in allDevices)
            {
                InputDevices_deviceConnected(device);
            }

            // Unity Action Delegates
            InputDevices.deviceConnected += InputDevices_deviceConnected;
            InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;

            rightController = GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>();
            leftController = GameObject.Find("LeftHand Controller").GetComponent<XRRayInteractor>();
//#if MRTK
            rightControllerMR = GameObject.Find("RightControllerAnchor");
            leftControllerMR = GameObject.Find("LeftControllerAnchor");
//#endif

            //LeftControllerAnchor

        }

    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= InputDevices_deviceConnected;
        InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
    }



    // Update is called once per frame
    void Update()
    {
        Vector2 localRightPrimaryAxis = Vector2.zero;
        Vector2 localLeftPrimaryAxis = Vector2.zero;
        float localRightTriggerAxis = 0;
        float localLeftTriggerAxis = 0;
        bool localRightTriggerFlag = false;
        bool localRightGripFlag = false;
        bool localLeftTriggerFlag = false;
        bool localLeftGripFlag = false;
        bool localRightPrimaryButtonFlag = false;
        bool localRightSecondaryButtonFlag = false;
        bool localLeftPrimaryButtonFlag = false;
        bool localLeftSecondaryButtonFlag = false;

        if (xrRig != null)
        {
            // Monitoring input for each button - could be made efficiently, If you know how to - please do it.
            foreach (var device in devicesWithPrimary2DAxis)
            {
                //if (device.name.Contains("Right"))
                //{
                //    device.TryGetFeatureValue(CommonUsages.primary2DAxis, out localRightPrimaryAxis);
                //    device.TryGetFeatureValue(CommonUsages.triggerButton, out localRightTriggerFlag);
                //    device.TryGetFeatureValue(CommonUsages.gripButton, out localRightGripFlag);
                //    device.TryGetFeatureValue(CommonUsages.primaryButton, out localRightPrimaryButtonFlag);
                //    device.TryGetFeatureValue(CommonUsages.secondaryButton, out localRightSecondaryButtonFlag);

                //}

                //if (device.name.Contains("Left"))
                //{
                //    device.TryGetFeatureValue(CommonUsages.primary2DAxis, out localLeftPrimaryAxis);
                //    device.TryGetFeatureValue(CommonUsages.triggerButton, out localLeftTriggerFlag);
                //    device.TryGetFeatureValue(CommonUsages.gripButton, out localLeftGripFlag);
                //    device.TryGetFeatureValue(CommonUsages.primaryButton, out localLeftPrimaryButtonFlag);
                //    device.TryGetFeatureValue(CommonUsages.secondaryButton, out localLeftSecondaryButtonFlag);
                //}
            }


#region Setters for Input
            // Setters for each Input Type
#region Right Controller

            localRightPrimaryAxis = new Vector2(Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal"), Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical"));

            localRightTriggerAxis = Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger");
            //UnityEngine.Debug.Log(localRightTriggerAxis);
            if (localRightTriggerAxis > triggerSensitivity)
                localRightTriggerFlag = true;
            else
                localRightTriggerFlag = false;
            if (Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.3)
                localRightGripFlag = true;
            else
                localRightGripFlag = false;

            localRightSecondaryButtonFlag = Input.GetButton("Oculus_CrossPlatform_Button1");
            localRightPrimaryButtonFlag = Input.GetButton("Oculus_CrossPlatform_Button2");
#endregion

#region Left Controller
            localLeftPrimaryAxis = new Vector2(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal"), Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical"));

            localLeftTriggerAxis =  Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger");
            if (localLeftTriggerAxis > triggerSensitivity)
                localLeftTriggerFlag = true;
            else
                localLeftTriggerFlag = false;

            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.3)
                localLeftGripFlag = true;
            else
                localLeftGripFlag = false;

            localRightSecondaryButtonFlag = Input.GetButton("Oculus_CrossPlatform_Button3");
            localLeftPrimaryButtonFlag = Input.GetButton("Oculus_CrossPlatform_Button4");
#endregion

            valueSetter(localRightPrimaryAxis, localLeftPrimaryAxis, localRightTriggerFlag, localRightGripFlag, localLeftTriggerFlag, localLeftGripFlag, localRightPrimaryButtonFlag, localRightSecondaryButtonFlag, localLeftPrimaryButtonFlag, localLeftSecondaryButtonFlag);

#endregion

        }
#region Generic inputs
            // Monitoring input for each button - could be made efficiently, If you know how to - please do it.

#region Setters for Input
            // Setters for each Input Type

            valueSetter(localRightPrimaryAxis, localLeftPrimaryAxis, localRightTriggerFlag, localRightGripFlag, localLeftTriggerFlag, localLeftGripFlag, localRightPrimaryButtonFlag, localRightSecondaryButtonFlag, localLeftPrimaryButtonFlag, localLeftSecondaryButtonFlag);

#endregion
#endregion

    }


#region Events,Actions and Callbacks
    // Events, Actions and Callbacks
    // If the device is connected, it is added to respective lists - right or left controller
    private void InputDevices_deviceConnected(InputDevice device)
    {
        Vector2 discardedValue;

        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out discardedValue))
        {

            devicesWithPrimary2DAxis.Add(device); // Add any devices that have a 2D axis.
        }

    }

    // If the device is disconnected, it is removed from respective lists - right or left controller
    private void InputDevices_deviceDisconnected(InputDevice device)
    {
        if (devicesWithPrimary2DAxis.Contains(device))
            devicesWithPrimary2DAxis.Remove(device);
    }

#endregion

#region Getters
    // All Public Getter Functions are to be added here - Absolutely no public variable access usage.

    // Note for other developers who will use/work on this: 
    // You don't let strangers into your house. You pickup stuff at the door and drop stuff at the door. In the same way, all data exchange between classes must happen via getters and setters.
    // Giving access to strangers (people / other classes) can cause serious issues.

    void valueSetter(Vector2 rAxis, Vector2 lAxis, bool rT, bool rG, bool lT, bool lG, bool RbA, bool RbB, bool LbA, bool LbB)
    {
        rightAxis = rAxis;
        leftAxis = lAxis;
        rightTriggerFlag = rT;
        rightGripFlag = rG;
        LeftTriggerFlag = lT;
        LeftGripFlag = lG;
        rightPrimaryButtonFlag = RbA;
        rightSecondaryButtonFlag = RbB;
        leftPrimaryButtonFlag = LbA;
        leftSecondaryButtonFlag = LbB;


    }
    public Vector2 getRightAxis()
    {
        return rightAxis;
    }

    public Vector2 getLeftAxis()
    {
        return leftAxis;
    }

    public bool isRightTriggerPressed()
    {
        return rightTriggerFlag;
    }

    public bool isLeftTriggerPressed()
    {
        return LeftTriggerFlag;
    }

    public bool isRightGripPressed()
    {
        return rightGripFlag;
    }

    public bool isLeftGripPressed()
    {
        return LeftGripFlag;
    }

    public bool isRightPrimaryAPressed()
    {
        return rightPrimaryButtonFlag;
    }
    public bool isLeftPrimaryXPressed()
    {
        return leftPrimaryButtonFlag;
    }
    public bool isRightSecondaryBPressed()
    {
        return rightSecondaryButtonFlag;
    }
    public bool isLeftSecondaryYPressed()
    {
        return leftSecondaryButtonFlag;
    }

    public RaycastHit getRightControllerRaycast()
    {
        RaycastHit hit;
        rightController.GetCurrentRaycastHit(out hit);
        return hit;
    }
    public RaycastHit getLeftControllerRaycast()
    {
        RaycastHit hit;
        leftController.GetCurrentRaycastHit(out hit);
        return hit;
    }

    public GameObject getRightController()
    {
        return rightController.gameObject;
    }


#endregion
}
