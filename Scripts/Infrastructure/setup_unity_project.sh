#!/bin/bash
# Script to set up a Unity project for Meta Quest 3 development
# This script should be run after the development environment is set up

echo "Setting up Unity project for Meta Quest 3 development..."

# Define project directory
UNITY_DIR="./Unity"
PACKAGES_FILE="$UNITY_DIR/Packages/manifest.json"

# Create Unity project structure if it doesn't exist
if [ ! -d "$UNITY_DIR" ]; then
    echo "Creating Unity project directory structure..."
    mkdir -p "$UNITY_DIR/Assets/MetaQuestTest/Scenes"
    mkdir -p "$UNITY_DIR/Assets/MetaQuestTest/Prefabs"
    mkdir -p "$UNITY_DIR/Assets/MetaQuestTest/Materials"
    mkdir -p "$UNITY_DIR/Assets/MetaQuestTest/Scripts"
    mkdir -p "$UNITY_DIR/Assets/MetaQuestTest/Tests"
    mkdir -p "$UNITY_DIR/Assets/Editor"
    mkdir -p "$UNITY_DIR/Packages"
    mkdir -p "$UNITY_DIR/ProjectSettings"

    # Create a basic manifest.json if it doesn't exist
    if [ ! -f "$PACKAGES_FILE" ]; then
        echo "Creating default package manifest..."
        cat > "$PACKAGES_FILE" << EOL
{
  "dependencies": {
    "com.unity.collab-proxy": "2.0.5",
    "com.unity.ide.rider": "3.0.24",
    "com.unity.ide.visualstudio": "2.0.18",
    "com.unity.ide.vscode": "1.2.5",
    "com.unity.test-framework": "1.1.33",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.timeline": "1.7.5",
    "com.unity.ugui": "1.0.0",
    "com.unity.visualscripting": "1.8.0",
    "com.unity.xr.interaction.toolkit": "2.3.2",
    "com.unity.xr.management": "4.3.3",
    "com.unity.xr.oculus": "4.0.0",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.cloth": "1.0.0",
    "com.unity.modules.director": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.terrain": "1.0.0",
    "com.unity.modules.terrainphysics": "1.0.0",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.umbra": "1.0.0",
    "com.unity.modules.unityanalytics": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestassetbundle": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.unitywebrequestwww": "1.0.0",
    "com.unity.modules.vehicles": "1.0.0",
    "com.unity.modules.video": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.wind": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
  }
}
EOL
    fi

    # Create a basic project settings file for Android/Quest
    echo "Creating basic project settings..."
    mkdir -p "$UNITY_DIR/ProjectSettings"
    cat > "$UNITY_DIR/ProjectSettings/ProjectSettings.asset" << EOL
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!129 &1
PlayerSettings:
  m_ObjectHideFlags: 0
  productGUID: 00000000000000000000000000000000
  AndroidProfiler: 0
  AndroidFilterTouchesWhenObscured: 0
  AndroidEnableSustainedPerformanceMode: 0
  defaultScreenOrientation: 3
  targetDevice: 2
  useOnDemandResources: 0
  accelerometerFrequency: 60
  companyName: fabioeloi
  productName: UnityMetaQuest3Test
  defaultCursor: {fileID: 0}
  cursorHotspot: {x: 0, y: 0}
  m_SplashScreenBackgroundColor: {r: 0.13725491, g: 0.12156863, b: 0.1254902, a: 1}
  m_ShowUnitySplashScreen: 1
  m_ShowUnitySplashLogo: 1
  m_SplashScreenOverlayOpacity: 1
  m_SplashScreenAnimation: 1
  m_SplashScreenLogoStyle: 1
  m_SplashScreenDrawMode: 0
  m_SplashScreenBackgroundAnimationZoom: 1
  m_SplashScreenLogoAnimationZoom: 1
  m_SplashScreenBackgroundLandscapeAspect: 1
  m_SplashScreenBackgroundPortraitAspect: 1
  m_SplashScreenBackgroundLandscapeUvs:
    serializedVersion: 2
    x: 0
    y: 0
    width: 1
    height: 1
  m_SplashScreenBackgroundPortraitUvs:
    serializedVersion: 2
    x: 0
    y: 0
    width: 1
    height: 1
  m_SplashScreenLogos: []
  m_VirtualRealitySplashScreen: {fileID: 0}
  m_HolographicTrackingLossScreen: {fileID: 0}
  defaultScreenWidth: 1920
  defaultScreenHeight: 1080
  defaultScreenWidthWeb: 960
  defaultScreenHeightWeb: 600
  m_StereoRenderingPath: 1
  m_ActiveColorSpace: 1
  m_MTRendering: 1
  mipStripping: 0
  numberOfMipsStripped: 0
  m_StackTraceTypes: 010000000100000001000000010000000100000001000000
  iosShowActivityIndicatorOnLoading: -1
  androidShowActivityIndicatorOnLoading: -1
  iosUseCustomAppBackgroundBehavior: 0
  iosAllowHTTPDownload: 1
  allowedAutorotateToPortrait: 1
  allowedAutorotateToPortraitUpsideDown: 1
  allowedAutorotateToLandscapeRight: 1
  allowedAutorotateToLandscapeLeft: 1
  useOSAutorotation: 1
  use32BitDisplayBuffer: 1
  preserveFramebufferAlpha: 0
  disableDepthAndStencilBuffers: 0
  androidStartInFullscreen: 1
  androidRenderOutsideSafeArea: 1
  androidUseSwappy: 1
  androidBlitType: 0
  androidResizableWindow: 0
  androidDefaultWindowWidth: 1920
  androidDefaultWindowHeight: 1080
  androidMinimumWindowWidth: 400
  androidMinimumWindowHeight: 300
  androidFullscreenMode: 1
  androidSupportedAspectRatio: 1
  androidMaxAspectRatio: 2.1
  applicationIdentifier:
    Android: com.fabioeloi.unitymetaquest3test
  buildNumber:
    Standalone: 0
    iPhone: 0
    tvOS: 0
  overrideDefaultApplicationIdentifier: 0
  AndroidBundleVersionCode: 1
  AndroidMinSdkVersion: 29
  AndroidTargetSdkVersion: 33
  AndroidPreferredInstallLocation: 1
  XRSettings:
    4:
      VRSettings:
        enabledDevices:
        - Oculus
      enabledDevices: 0a000000
    7:
      VRSettings:
        enabledDevices:
        - Oculus
      enabledDevices: 0a000000
    14:
      VRSettings:
        enabledDevices:
        - Oculus
      enabledDevices: 0a000000
EOL

    # Create a basic XR plugin management settings
    echo "Setting up XR Plugin Management..."
    mkdir -p "$UNITY_DIR/ProjectSettings/XR"
    cat > "$UNITY_DIR/ProjectSettings/XR/Loaders.asset" << EOL
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: d2dc886499c26824283350fa532d087d, type: 3}
  m_Name: XRGeneralSettingsPerBuildTarget
  m_EditorClassIdentifier: 
  Keys: 07000000
  Values:
  - {fileID: 1329871453023471399}
--- !u!114 &1329871453023471399
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: d236b7d11115f2143951f1e14045df39, type: 3}
  m_Name: Android Settings
  m_EditorClassIdentifier: 
  m_LoaderManagerInstance: {fileID: 8885861488494745212}
  m_InitManagerOnStart: 1
--- !u!114 &8885861488494745212
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: f4c3631f5e58749a59194e0cf6baf6d5, type: 3}
  m_Name: Android Providers
  m_EditorClassIdentifier: 
  m_RequiresSettingsUpdate: 0
  m_AutomaticLoading: 0
  m_AutomaticRunning: 0
  m_Loaders:
  - {fileID: 11400000, guid: 9b09f0106dcd341a980413bd4ca80c1a, type: 2}
EOL
fi

# Copy our DDD-structured scripts to the Unity project
echo "Copying scripts to Unity project..."
SCRIPT_DIR="../Scripts"
UNITY_SCRIPT_DIR="$UNITY_DIR/Assets/MetaQuestTest/Scripts"

if [ -d "$SCRIPT_DIR" ]; then
    # Ensure target directory exists
    mkdir -p "$UNITY_SCRIPT_DIR/Domain"
    mkdir -p "$UNITY_SCRIPT_DIR/Application"
    mkdir -p "$UNITY_SCRIPT_DIR/Infrastructure"
    mkdir -p "$UNITY_SCRIPT_DIR/Presentation"
    
    # Copy scripts while maintaining directory structure
    cp -R "$SCRIPT_DIR/Domain/" "$UNITY_SCRIPT_DIR/Domain/"
    cp -R "$SCRIPT_DIR/Application/" "$UNITY_SCRIPT_DIR/Application/"
    cp -R "$SCRIPT_DIR/Infrastructure/" "$UNITY_SCRIPT_DIR/Infrastructure/"
    cp -R "$SCRIPT_DIR/Presentation/" "$UNITY_SCRIPT_DIR/Presentation/"
    
    echo "Scripts copied successfully!"
else
    echo "Warning: Script directory not found!"
fi

# Copy tests to Unity test directory
echo "Setting up test framework..."
TEST_DIR="../Test"
UNITY_TEST_DIR="$UNITY_DIR/Assets/MetaQuestTest/Tests"

if [ -d "$TEST_DIR" ]; then
    # Ensure target directory exists
    mkdir -p "$UNITY_TEST_DIR"
    
    # Copy tests
    cp -R "$TEST_DIR/" "$UNITY_TEST_DIR/"
    
    echo "Tests copied successfully!"
else
    echo "Warning: Test directory not found!"
fi

# Create a basic scene file
echo "Creating basic scene file..."
mkdir -p "$UNITY_DIR/Assets/MetaQuestTest/Scenes"
cat > "$UNITY_DIR/Assets/MetaQuestTest/Scenes/MainScene.unity" << EOL
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {fileID: 0}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 0
  m_FogColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {r: 0.212, g: 0.227, b: 0.259, a: 1}
  m_AmbientEquatorColor: {r: 0.114, g: 0.125, b: 0.133, a: 1}
  m_AmbientGroundColor: {r: 0.047, g: 0.043, b: 0.035, a: 1}
  m_AmbientIntensity: 1
  m_AmbientMode: 0
  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}
  m_SkyboxMaterial: {fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {fileID: 0}
  m_SpotCookie: {fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {fileID: 0}
  m_Sun: {fileID: 0}
  m_IndirectSpecularColor: {r: 0, g: 0, b: 0, a: 1}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 1
    m_EnableRealtimeLightmaps: 0
  m_LightmapEditorSettings:
    serializedVersion: 12
    m_Resolution: 2
    m_BakeResolution: 40
    m_AtlasSize: 1024
    m_AO: 0
    m_AOMaxDistance: 1
    m_CompAOExponent: 1
    m_CompAOExponentDirect: 0
    m_ExtractAmbientOcclusion: 0
    m_Padding: 2
    m_LightmapParameters: {fileID: 0}
    m_LightmapsBakeMode: 1
    m_TextureCompression: 1
    m_FinalGather: 0
    m_FinalGatherFiltering: 1
    m_FinalGatherRayCount: 256
    m_ReflectionCompression: 2
    m_MixedBakeMode: 2
    m_BakeBackend: 1
    m_PVRSampling: 1
    m_PVRDirectSampleCount: 32
    m_PVRSampleCount: 512
    m_PVRBounces: 2
    m_PVREnvironmentSampleCount: 256
    m_PVREnvironmentReferencePointCount: 2048
    m_PVRFilteringMode: 1
    m_PVRDenoiserTypeDirect: 1
    m_PVRDenoiserTypeIndirect: 1
    m_PVRDenoiserTypeAO: 1
    m_PVRFilterTypeDirect: 0
    m_PVRFilterTypeIndirect: 0
    m_PVRFilterTypeAO: 0
    m_PVREnvironmentMIS: 1
    m_PVRCulling: 1
    m_PVRFilteringGaussRadiusDirect: 1
    m_PVRFilteringGaussRadiusIndirect: 5
    m_PVRFilteringGaussRadiusAO: 2
    m_PVRFilteringAtrousPositionSigmaDirect: 0.5
    m_PVRFilteringAtrousPositionSigmaIndirect: 2
    m_PVRFilteringAtrousPositionSigmaAO: 1
    m_ExportTrainingData: 0
    m_TrainingDataDestination: TrainingData
    m_LightProbeSampleCountMultiplier: 4
  m_LightingDataAsset: {fileID: 0}
  m_LightingSettings: {fileID: 0}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 2
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    accuratePlacement: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {fileID: 0}
--- !u!1 &705507993
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 705507995}
  - component: {fileID: 705507994}
  m_Layer: 0
  m_Name: Directional Light
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!108 &705507994
Light:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 705507993}
  m_Enabled: 1
  serializedVersion: 10
  m_Type: 1
  m_Shape: 0
  m_Color: {r: 1, g: 0.95686275, b: 0.8392157, a: 1}
  m_Intensity: 1
  m_Range: 10
  m_SpotAngle: 30
  m_InnerSpotAngle: 21.80208
  m_CookieSize: 10
  m_Shadows:
    m_Type: 2
    m_Resolution: -1
    m_CustomResolution: -1
    m_Strength: 1
    m_Bias: 0.05
    m_NormalBias: 0.4
    m_NearPlane: 0.2
    m_CullingMatrixOverride:
      e00: 1
      e01: 0
      e02: 0
      e03: 0
      e10: 0
      e11: 1
      e12: 0
      e13: 0
      e20: 0
      e21: 0
      e22: 1
      e23: 0
      e30: 0
      e31: 0
      e32: 0
      e33: 1
    m_UseCullingMatrixOverride: 0
  m_Cookie: {fileID: 0}
  m_DrawHalo: 0
  m_Flare: {fileID: 0}
  m_RenderMode: 0
  m_CullingMask:
    serializedVersion: 2
    m_Bits: 4294967295
  m_RenderingLayerMask: 1
  m_Lightmapping: 1
  m_LightShadowCasterMode: 0
  m_AreaSize: {x: 1, y: 1}
  m_BounceIntensity: 1
  m_ColorTemperature: 6570
  m_UseColorTemperature: 0
  m_BoundingSphereOverride: {x: 0, y: 0, z: 0, w: 0}
  m_UseBoundingSphereOverride: 0
  m_UseViewFrustumForShadowCasterCull: 1
  m_ShadowRadius: 0
  m_ShadowAngle: 0
--- !u!4 &705507995
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 705507993}
  m_LocalRotation: {x: 0.40821788, y: -0.23456968, z: 0.10938163, w: 0.8754261}
  m_LocalPosition: {x: 0, y: 3, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_Children: []
  m_Father: {fileID: 0}
  m_RootOrder: 0
  m_LocalEulerAnglesHint: {x: 50, y: -30, z: 0}
--- !u!1 &1234567890
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 1234567891}
  - component: {fileID: 1234567892}
  m_Layer: 0
  m_Name: VR Scene Setup
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &1234567891
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1234567890}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_Children: []
  m_Father: {fileID: 0}
  m_RootOrder: 1
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!114 &1234567892
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1234567890}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: YOUR_GUID_HERE, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
EOL

echo "Unity project setup complete!"
echo ""
echo "Next steps:"
echo "1. Open Unity Hub"
echo "2. Add project from disk (point to the Unity directory)"
echo "3. Open project and set up the XR Interaction Toolkit packages"
echo "4. Build and deploy using deploy_to_quest.sh script"