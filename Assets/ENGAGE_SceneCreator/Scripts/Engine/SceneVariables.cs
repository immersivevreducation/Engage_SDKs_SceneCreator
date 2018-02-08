using UnityEngine;
using System.Collections;
//using UnityEngine.VR;

using UnityEngine;
using System.Collections;

public class SceneVariables : MonoBehaviour {

	public string sceneFilename;
	public float theSceneScale = 1.3f; // affects player height and vr scale settings
	public Transform sceneRootObject;
    public GameObject copyCameraImageEffectsObject;
    public bool replaceSkybox;
	public Material skyboxMaterial;
	public Transform ifxInstantiationLocation;
	public Vector3 gravity = new Vector3 (0, -9.8f, 0);
	public int outsideTheaterVoiceMode = 1;//0 = 2d, 1 = 3d  (Set 0 if you want voices 2d always, 3d  and if you have a specific area that sets the playermanager.playerintheater bool to true will it will be 2d)
	public bool keepHeadboxOnAfterStart = false;
	public bool delayHeadBoxRemoval = false;
	public float headboxDelaySeconds = 0f;
	public bool disableTablet = false;
    public bool localPlayerShadowsOff;
    public bool remotePlayerShadowsOff;
	public int outfitOverrideOnStart = -1;
	public bool overrideMinDrawDistance = false;
	public bool overrideMaxDrawDistance = false;
	public float minDrawDistanceAmount = 0;
	public float maxDrawDistanceAmount = 0;
	public bool layerCameraOverride = false;
	public float layerCameraOverrideNearClip;
	public float layerCameraOverrideFarClip;
	public bool useLightProbes = false;
	public bool noAA = false;
    public float teleportVelocity = 15;
    public Transform teleportXMin;
	public Transform teleportXMax;
	public Transform teleportZMin;
	public Transform teleportZMax;
	public bool hasWhiteboard = false;
	public bool hasBlackboard = false;
	public bool whiteboardXFacing = false;
	public bool whiteboardYFacing = false;
	public bool whiteboardZFacing = false;
	public float whiteboardPosition = 0;
	public float whiteboardRestingPosition = 0;
	public GameObject whiteboardRestingObject;
	public bool whiteboardHasParent = false;
	public GameObject whiteboardParent;
	public bool hideBodies = false;
	GameObject theaterVariablesObject = null;

}
