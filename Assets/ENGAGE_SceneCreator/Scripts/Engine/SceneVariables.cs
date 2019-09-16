using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
/// <summary>
/// For manually defining a scene's unique variables
/// On an initial scene load (not additive), this script will create
/// a TheaterVariables instance and transfer these variables to it as 
/// static variables, and rules for the scene. Also sets up the scene
/// by instantiating the core GameObjects on MasterClient 
/// </summary>
public class SceneVariables : MonoBehaviour
{

    public enum LegacyWhiteboardType
    {
        none,
        blackboard,
        whiteboard
    }
    /// <summary>
    /// Actual scene name (ID)
    /// </summary>
	public string sceneFilename;

    /// <summary>The scale the scene was created at (Initial Engage scenes were 1.3)</summary>
    /// <remarks>Affects player height and vr scale settings</remarks>
	public float theSceneScale = 1.3f;

    /// <summary>
    /// The gravity of the scene
    /// </summary>
	public Vector3 gravity = new Vector3(0, -9.8f, 0);

    /// <summary>
    /// Disable local player shadows
    /// </summary>
    public bool localPlayerShadowsOff;

    /// <summary>
    /// Disable remote player shadows
    /// </summary>
    public bool remotePlayerShadowsOff;

    /// <summary>
    /// Velocity of the teleport function
    /// </summary>
    public float teleportVelocity = 12;

    //If there is a legacy whiteboard in this location, what type is it
    public LegacyWhiteboardType legacyWhiteboardType;

    /// <summary>
    /// Outfit override for this scene (Or -1 for normal clothes)
    /// </summary>
    [Header("")]
    [Header("Only if override is necessary, otherwise -1")]
    [Header("")]
    public int outfitOverrideOnStart = -1;


    [Header("")]
    [Header("Camera Clipping - set to false unless absolutely needed")]
    [Header("")]
    /// <summary>
    /// Override camera's near clip plane?
    /// </summary>
	public bool overrideMinDrawDistance = false;

    /// <summary>
    /// Override camera's far clip plane?
    /// </summary>
	public bool overrideMaxDrawDistance = false;

    /// <summary>
    /// Override amount for camera's near clip plane
    /// </summary>
	public float minDrawDistanceAmount = 0;

    /// <summary>
    /// Override amount for camera's far clip plane
    /// </summary>
	public float maxDrawDistanceAmount = 0;

    [Header("")]
    [Header("-----If this is not a locked, lesson-only scene, set the below to false!-----")]
    [Header("-----Be Very Careful with the below settings-----")]
    [Header("")]

    /// <summary>
    /// Keep headbox on, must manually remove via script or effect (careful with this)
    /// </summary>
    public bool keepHeadboxOnAfterStart = false;

    /// <summary>
    /// A safer way to delay headbox removal (specified seconds)
    /// </summary>
	public bool delayHeadBoxRemoval = false;

    /// <summary>
    /// Actual seconds to delay headbox removal
    /// </summary>
	public float headboxDelaySeconds = 0f;

    /// <summary>
    /// Disable IFX & recordings
    /// </summary>
    /// <remarks>Typically used for scenes that aren't free roam / part of an experience</remarks>
	public bool disableTablet = false;


    GameObject theaterVariablesObject = null;
    GameObject engineObject;

#if UNITY_ENGAGE
    /// <summary>
    /// Instantiate TheaterVariables if necessary and set scene rules
    /// </summary>
    void Awake()
    {
        DestroyEventSystems();
		if (GameObject.Find ("TheaterVariables")) {
			theaterVariablesObject = GameObject.Find ("TheaterVariables");
			TheaterVariables varScript = theaterVariablesObject.GetComponent<TheaterVariables> ();
			TheaterVariables.roomOverrideMinDrawDistance = overrideMinDrawDistance;
			TheaterVariables.roomOverrideMaxDrawDistance = overrideMaxDrawDistance;
			TheaterVariables.roomMinDrawDistanceAmount = minDrawDistanceAmount;
			TheaterVariables.roomMaxDrawDistanceAmount = maxDrawDistanceAmount;
		} else {
			theaterVariablesObject = Instantiate (Resources.Load ("TheaterVariables") as GameObject);
			theaterVariablesObject.name = "TheaterVariables";
			TheaterVariables.sceneScale = theSceneScale;
            TheaterVariables.originalSceneScale = theSceneScale;
			TheaterVariables.myIfxPoint = null;
			
			TheaterVariables.roomOutfitOverrideOnStart = outfitOverrideOnStart;
			TheaterVariables.roomKeepHeadboxOnAfterStart = keepHeadboxOnAfterStart;
			TheaterVariables.roomDelayHeadBoxRemoval = delayHeadBoxRemoval;
			TheaterVariables.roomHeadboxDelaySeconds = headboxDelaySeconds;
			TheaterVariables.roomDisableTablet = disableTablet;
            TheaterVariables.roomLocalPlayerShadowsOff = localPlayerShadowsOff;
            TheaterVariables.roomRemotePlayerShadowsOff = remotePlayerShadowsOff;
			TheaterVariables.roomOverrideMinDrawDistance = overrideMinDrawDistance;
			TheaterVariables.roomOverrideMaxDrawDistance = overrideMaxDrawDistance;
			TheaterVariables.roomMinDrawDistanceAmount = minDrawDistanceAmount;
			TheaterVariables.roomMaxDrawDistanceAmount = maxDrawDistanceAmount;
			TheaterVariables.roomGravity = gravity;
            TheaterVariables.roomTeleportVelocity = teleportVelocity;
			Physics.gravity = gravity;
		}
		
	}

    /// <summary>
    /// Instantiate Engage engine objects, destroy temp cameras
    /// </summary>
    /// <returns></returns>
    void Start()
    {
        if (Component.FindObjectOfType<ENG_IGM_PlayerManager>())
        {
            //Engine is already available. (additive scene)
        }
        else
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "LoadingRoomAdditive")
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        engineObject = PhotonNetwork.Instantiate("Engage_Engine_Object", transform.position, transform.rotation, 0) as GameObject;
                    }
                    else {
#if !ENGAGE_FOCUS
                        Instantiate(Resources.Load("TempCamera") as GameObject);
#endif
                    }
                }
            }
            else {
                if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 2)
                {
                    engineObject = Instantiate(Resources.Load("Engage_Engine_Object"), transform.position, transform.rotation) as GameObject;
                }
            }
        }
    }

    /// <summary>
    /// Destroy rogue event systems
    /// </summary>
    void DestroyEventSystems() {
        if (Component.FindObjectOfType<EventSystem>() != null)
            foreach (EventSystem evSysObject in Component.FindObjectsOfType<EventSystem>())
                if(!evSysObject.gameObject.GetComponent<Wacki.LaserPointerInputModule>())Destroy(evSysObject.gameObject);
    }
#endif
}
