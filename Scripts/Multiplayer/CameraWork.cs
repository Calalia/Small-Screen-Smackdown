using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;



public class CameraWork : MonoBehaviour
{
    #region Private Fields


    [Tooltip("The distance in the local x-z plane to the target")]
    [SerializeField]
    private float distance = 7.0f;


    [Tooltip("The height we want the camera to be above the target")]
    [SerializeField]
    private float height = 3.0f;


    [Tooltip("The Smooth time lag for the height of the camera.")]
    [SerializeField]
    private float heightSmoothLag = 0.3f;


    [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;


    [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
    [SerializeField]
    private bool followOnStart = false;


    // cached transform of the target
    Transform cameraTransform;
    private PhotonView photonView;


    // maintain a flag internally to reconnect if target is lost or camera is switched
    bool isFollowing;


    // Represents the current velocity, this value is modified by SmoothDamp() every time you call it.
    private float heightVelocity;


    // Represents the position we are trying to reach using SmoothDamp()
    private float targetHeight = 100000.0f;


    #endregion
    
    #region MonoBehaviour Callbacks
    
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            this.enabled = false;
            return;
        }
        if (!PhotonNetwork.IsMasterClient) centerOffset = new Vector3(-4,0,-4);
        // Start following the target if wanted.
        if (followOnStart)
        {
            OnStartFollowing();
        }

    }
    
    void LateUpdate()
    {
        // The transform target may not destroy on level load,
        // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
        if (cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }
        // only follow is explicitly declared
        if (isFollowing)
        {
            Apply();
        }
    }


    #endregion
    
    #region Public Methods


    /// <summary>
    /// Raises the start following event.
    /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
    /// </summary>
    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        // we don't smooth anything, we go straight to the right camera shot
        Cut();
    }


    #endregion
    
    #region Private Methods


    /// <summary>
    /// Follow the target smoothly
    /// </summary>
    void Apply()
    {
        Vector3 targetCenter = transform.position + centerOffset;
        // Calculate the current & target rotation angles
        float originalTargetAngle = transform.eulerAngles.y;
        float currentAngle = cameraTransform.eulerAngles.y;
        // Adjust real target angle when camera is locked
        float targetAngle = originalTargetAngle;
        currentAngle = targetAngle;
        targetHeight = targetCenter.y + height;


        // Damp the height
        float currentHeight = cameraTransform.position.y;
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightVelocity, heightSmoothLag);
        // Convert the angle into a rotation, by which we then reposition the camera
        Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);
        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        cameraTransform.position = targetCenter;
        cameraTransform.position += currentRotation * Vector3.back * distance;
        // Set the height of the camera
        cameraTransform.position = new Vector3(cameraTransform.position.x, currentHeight, cameraTransform.position.z);
    }
    

    /// <summary>
    /// Directly position the camera to a the specified Target and center.
    /// </summary>
    void Cut()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            cameraTransform.rotation = Quaternion.LookRotation(new Vector3(-1, -2, -1), Vector3.up);
        }
        else
        {
            cameraTransform.rotation = Quaternion.LookRotation(new Vector3(1, -2, 1), Vector3.up);
        }
        float oldHeightSmooth = heightSmoothLag;
        heightSmoothLag = 0.001f;
        Apply();
        heightSmoothLag = oldHeightSmooth;
    }
    
    #endregion
}