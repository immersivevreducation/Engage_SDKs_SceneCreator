
using UnityEngine;
using UnityEditor;
using System.IO;

namespace IFXTools{
    public class IFXThumbnailTool
    {
        public float lightBrightness { get; set; } = 1.5f;
        public float lightRotation  { get; set; } = 0;
        public float cameraZoom { get; set; } = 50;

        public GameObject ifxObject { get; set; }
        public float objRotationX { get; set; } = 0;
        public float objRotationY { get; set; } = 0;

        public float objPosY { get; set; } = 0;
        public float objPosX { get; set; } = 0;
        public float objScale { get; set; } = 1;
        public Texture2D thumbnailImage { get; set; }
        public Texture2D previewImage {get; set;}
        public Light[] lights { get; set; }

        public Camera camera {get; set;}
        public Transform lightMover {get; set;}
        public GameObject ifxThumbnailRig {get; set;}
        public GameObject cameraObject {get; set;}
        public Rect thumbnailPreviewRect {get; set;}
        ///////////////////////Internal/////////////////////////
        RenderTexture activeRenderTexture = RenderTexture.active;         
        
        
        public void ThumbnailToolControlsUI()
        {
            
            
            //Lights Comtrols
            GUILayout.Label("Brightness");
            lightBrightness = EditorGUILayout.Slider(lightBrightness, 0.1f, 10);
            LightsBrightness(lightBrightness);
            GUILayout.Label("Rotate Lights");
            lightRotation = EditorGUILayout.Slider(lightRotation, 180, -180);
            RotateLights(new Vector3(0, lightRotation, 0));

            //Camera_Controls
            GUILayout.Label("Camera Zoom");
            cameraZoom = EditorGUILayout.Slider(cameraZoom, 100, 1);
            camera.fieldOfView = cameraZoom;

            //Object Thumbnail Controls
            //Scale
            GUILayout.Label("IFX Scale");
            objScale = EditorGUILayout.Slider(objScale, 0.01f, 2);
            ScaleIFX(new Vector3(objScale, objScale, objScale));

            //Rotate
            GUILayout.Label("IFX Rotation - Up Down");
            objRotationX = EditorGUILayout.Slider(objRotationX, 180, -180);
            GUILayout.Label("IFX Rotation - Left Right");
            objRotationY = EditorGUILayout.Slider(objRotationY, 180, -180);
            RotateIFX(new Vector3(objRotationX, objRotationY, 0));

            //move
            GUILayout.Label("IFX Move - Up Down");
            objPosY = EditorGUILayout.Slider(objPosY, 10, -10);
            GUILayout.Label("IFX Move - Left Right");
            objPosX = EditorGUILayout.Slider(objPosX, -10, 10);
            MoveIFX(new Vector3(objPosX, objPosY, 0));
            
        }
        public void ResetCameraSettings()
        {
            cameraZoom = 50;
            objScale = 1;
            objRotationX = 0;
            objRotationY = 0;

            objPosY = 0;
            objPosX = 0;
        }

        public void ThumbnailSetup(GameObject ifxObjectIN)
        {
            ifxObject = ifxObjectIN;
            try
            {
                ifxThumbnailRig = GameObject.Find("IFX_Thumbnail_Rig");
                cameraObject = GameObject.Find("IFX_Thumbnail_Camera");
                lightMover = ifxThumbnailRig.transform.Find("Light_Mover") ;
            }
            catch (FileNotFoundException e)
            {
                Debug.Log($"IFX_Thumbnail_Rig part was not found: '{e}'");
            }
            

            lights = ifxThumbnailRig.GetComponentsInChildren<Light>();

            camera = cameraObject.GetComponent(typeof(Camera)) as Camera;
            RenderTexture rt = new RenderTexture(750, 500, 8, RenderTextureFormat.ARGB32);
            camera.targetTexture = rt;
            RenderTexture activeRenderTexture = RenderTexture.active;          
            previewImage = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
            thumbnailImage = previewImage;
        }
        public void UpdatePreviewImage()
        {
            RenderTexture.active = camera.targetTexture;
            camera.Render();
            previewImage.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            previewImage.Apply();
            RenderTexture.active = activeRenderTexture;
            
        }
        public void SaveThumbnail(string path)
        {
            if (path !="" && Directory.Exists(path))
            {
                //Save Image to file
                byte[] bytes = thumbnailImage.EncodeToPNG();
                File.WriteAllBytes(path +"/"+ ifxObject.name + ".png", bytes);
            }
            else
            {
                 EditorUtility.DisplayDialog("No Folder Found at: "+path,
                 "Please Choose a thumbnail save location in the settings menu of this tool", "OK");
            }
            
        }

        public void AutoCamera(GameObject obj, Transform transform )//obj is the asset, and transform in this case was the camera itself. not working 100%
        {
            Camera camera = transform.GetComponent(typeof(Camera)) as Camera;
            //auto adjust camera
            Mesh mesh = obj.GetComponentInChildren<MeshFilter>().sharedMesh;
            Bounds bounds = mesh.bounds;
            float adjac = Vector3.Distance(transform.TransformPoint(bounds.center), transform.TransformPoint(bounds.extents));
            float theta = 90 - (camera.fieldOfView/2);
            float hypot = adjac/Mathf.Cos(theta);
            if(hypot < 0)
                hypot *= -1;
                Debug.Log(hypot);
            float distance = hypot*1.2f;
            transform.position = transform.rotation * new Vector3(0, 0, -distance) + obj.transform.position;
        }
    //this needs more work and dosn't work currently
        public void RotateLights(Vector3 rotateIN)
        {
            lightMover.localRotation = Quaternion.Euler(rotateIN);
        }
        public void LightsBrightness(float BrightnessIN)
        {
            foreach (Light light in lights)
            {
                light.intensity = BrightnessIN;
            }
        }
        public void MoveIFX(Vector3 positionIN)
        {
            ifxObject.transform.localPosition = positionIN;
        }
        public void RotateIFX(Vector3 rotateIN)
        {
            ifxObject.transform.localRotation = Quaternion.Euler(rotateIN);
        }
        public void ScaleIFX(Vector3 scaleIN)
            {
                ifxObject.transform.localScale = scaleIN;
            }
            
            
            
    }
}
        
    