using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using FyberPlugin;
using FyberPlugin.LitJson;

namespace FyberEditor
{

    public class PlatformChecker
    {
        public static bool isAndroid()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
        }
    }

    [InitializeOnLoad]
    public class FyberEditorSettings : ScriptableObject
    {

        const string fyberSettingsAssetName = "FyberEditorSettings";
        const string fyberSettingsPath = "OfferWallEdge/Editor/Resources";
        const string fyberSettingsAssetExtension = ".asset";

        [SerializeField]
        internal AndroidManifestChecker manifestChecker;

        [HideInInspector]
        public bool shouldAutogenerateAssets = true;

        private static FyberEditorSettings instance;

        static FyberEditorSettings()
        {
            EditorApplication.update += Initialize;
        }

        public static void Initialize()
        {
            EditorApplication.update -= Initialize;

            if (PlatformChecker.isAndroid())
                InitializeAndroid();
        }

        private static void InitializeAndroid()
        {
            var manifestChecker = Instance.manifestChecker;
            if (manifestChecker != null)
                manifestChecker.CheckOnStart();
        }

        [MenuItem("Fyber/Edit SDK Settings")]
        public static void Edit()
        {
            Selection.activeObject = Instance;
        }

        public static FyberEditorSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(fyberSettingsAssetName) as FyberEditorSettings;
                    if (instance == null)
                    {
                        // If not found, autocreate the asset object.
                        instance = CreateInstance<FyberEditorSettings>();

                        string properPath = Path.Combine(Application.dataPath, fyberSettingsPath);
                        if (!Directory.Exists(properPath))
                        {
                            AssetDatabase.CreateFolder("Assets/OfferWallEdge/Editor", "Resources");
                        }

                        string fullPath = Path.Combine(Path.Combine("Assets", fyberSettingsPath),
                                                    fyberSettingsAssetName + fyberSettingsAssetExtension
                                                    );
                        AssetDatabase.CreateAsset(instance, fullPath);

                        AddManifestChecker(instance);
                    }
                }

                return instance;
            }
        }

        internal static void AddManifestChecker(FyberEditorSettings settings = null)
        {
            if (settings == null)
                settings = Instance;
            settings.manifestChecker = ScriptableObject.CreateInstance<AndroidManifestChecker>();
            settings.manifestChecker.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(settings.manifestChecker, settings);
        }
    }

    [CustomEditor(typeof(FyberEditorSettings))]
    public class SettingsEditor : Editor
    {

        List<Editor> editors = new List<Editor>();

        Editor manifestEditor;

        void OnEnable()
        {
            if (PlatformChecker.isAndroid())
            {

                var manifestChecker = ((FyberEditorSettings)target).manifestChecker;
                if (manifestChecker == null)
                {
                    FyberEditorSettings.AddManifestChecker((FyberEditorSettings)target);
                    manifestChecker = ((FyberEditorSettings)target).manifestChecker;
                }
                manifestEditor = CreateEditor(manifestChecker);
            }
        }

        public override void OnInspectorGUI()
        {
            if (PlatformChecker.isAndroid())
                DrawAndroidSettings();
            else
                DrawNoSettings();
        }

        private void DrawAndroidSettings()
        {
            if (manifestEditor != null)
                manifestEditor.OnInspectorGUI();

            GUI.changed = false;

            foreach (Editor ed in editors)
            {
                ed.OnInspectorGUI();
                EditorGUILayout.Space();
            }

        }

        private void DrawNoSettings()
        {
            EditorGUILayout.HelpBox("There are no settings to be configured for the selected platform", MessageType.Info);
        }
    }

}
