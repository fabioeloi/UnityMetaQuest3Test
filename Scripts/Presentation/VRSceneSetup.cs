using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

namespace MetaQuestTest.Presentation
{
    /// <summary>
    /// Scene setup for VR environments
    /// Handles initialization of XR components and environment setup
    /// </summary>
    public class VRSceneSetup : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _xrRigPrefab;
        
        [SerializeField] 
        private GameObject _interactionControllerPrefab;
        
        [SerializeField]
        private Material _floorMaterial;
        
        [SerializeField]
        private Material _skyboxMaterial;
        
        void Awake()
        {
            SetupScene();
        }
        
        private void SetupScene()
        {
            // Setup basic environment
            CreateFloor();
            SetSkybox();
            
            // Setup XR components if needed
            if (FindObjectOfType<XRRig>() == null && _xrRigPrefab != null)
            {
                Instantiate(_xrRigPrefab, Vector3.zero, Quaternion.identity);
            }
            
            // Setup interaction controller if needed
            if (FindObjectOfType<VRInteractionController>() == null && _interactionControllerPrefab != null)
            {
                Instantiate(_interactionControllerPrefab, Vector3.zero, Quaternion.identity);
            }
            else if (_interactionControllerPrefab == null)
            {
                // Create a default interaction controller
                GameObject controllerObj = new GameObject("VR Interaction Controller");
                controllerObj.AddComponent<VRInteractionController>();
            }
            
            Debug.Log($"VR Scene setup complete for scene: {SceneManager.GetActiveScene().name}");
        }
        
        private void CreateFloor()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.position = new Vector3(0, 0, 0);
            floor.transform.localScale = new Vector3(10, 1, 10); // 10x10 meter floor
            
            if (_floorMaterial != null)
            {
                floor.GetComponent<Renderer>().material = _floorMaterial;
            }
        }
        
        private void SetSkybox()
        {
            if (_skyboxMaterial != null)
            {
                RenderSettings.skybox = _skyboxMaterial;
            }
        }
        
        // This ensures the scene setup is performed even when entering play mode
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            Debug.Log("Initializing VR environment...");
        }
    }
}