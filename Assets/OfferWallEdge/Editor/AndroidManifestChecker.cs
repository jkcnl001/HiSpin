using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Collections.Generic;

namespace FyberEditor
{
	[Serializable]
	public class AndroidManifestChecker : ScriptableObject
	{
		private const string androidManifestFileName = "AndroidManifest.xml";
		private const string manifestPath = "Plugins/Android";
		// when something goes wrong copying the default the the 'Plugins' folder, we'll need to let publishers know they'll need to add it manually		
		private const string manuallyManifestSetupMsg = "You'll need to create the manifest manually and try again. Alternatively, you can add all necessary activities and permission manually.";

		// Different Unity version have the default android manifest located on different paths. Unity documentation is not clear about this so we try all
		// locations we know to date (4.7 > UNITY version < 2017.3)
		// note also that these paths can occurr within the package content or at the base dir (starting on unity 5.3, Mac)
		private string[] manifestPaths = { "PlaybackEngines/AndroidPlayer/Apk", "AndroidPlayer/Apk", "PlaybackEngines/AndroidPlayer", "Unity/PlaybackEngines" };

		private readonly object syncLock = new object();

		[SerializeField]
		private bool checkOnStart = true;

		[SerializeField]
		private bool firstRun = true;

		[SerializeField]
		private bool showConfirmationDialogs = true;

		internal void CheckOnStart()
		{
			// check if this is the first time it runs
			// if so, show a dialog message about the feature
			// and provide a way to disable it
			if (firstRun)
			{
				checkOnStart = EditorUtility.DisplayDialog("Fyber Unity Plugin", "We would like to check your AndroidManifest.xml for the required entries.\n" +
					"You can always change the behaviour in the Fyber Settings Panel", "Proceed", "Cancel");
				SerializedObject serializedObject = new SerializedObject(this);
				serializedObject.FindProperty("firstRun").boolValue =  false;
				serializedObject.FindProperty("checkOnStart").boolValue = checkOnStart;
				serializedObject.ApplyModifiedProperties();
			}
			// only run if this is compiling for android
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
			{
				// only do the actual check if the permission was granted
				// check if there is a default manifest file
				if (checkOnStart && CheckAndroidManifestExists())
				{
					// check Fyber content inside 
					CheckFyberEntries();
				}
			}
		}

		// check if the Assets/Plugins/Android/AndroidManifest.xml file exists
		// and if not, copy the default one
		internal bool CheckAndroidManifestExists()
		{
	        string destinationPath = GetPath(manifestPath, androidManifestFileName);
	        FileInfo manifestFileInfo = new FileInfo(destinationPath);
			
			if (!manifestFileInfo.Exists)
			{
				if (showConfirmationDialogs && !EditorUtility.DisplayDialog("AndroidManifes.xml is missing", "The default Unity AndroidManifest.xml will be copied to your Assets/Plugins/Android folder.", "OK", "Cancel"))
					return false;

				string defaultManifest = FindDefaultAndroidManifest();				
				
				if (String.IsNullOrEmpty(defaultManifest) || !File.Exists(defaultManifest))
				{
					Debug.LogWarning("No default manifest found. " + manuallyManifestSetupMsg);
					return false;
				}
				// move the default unity manifest to the 'Plugins' folder
				File.Copy(defaultManifest, manifestFileInfo.FullName);
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
				manifestFileInfo.Refresh();
	      		if (!manifestFileInfo.Exists) 
				{
					Debug.LogWarning("An error occured while trying to move the default AndroidManifest.xml file. " + manuallyManifestSetupMsg);
					return false;
				}
			}
			return true;
		}

		private void OpenManifest(out XmlDocument doc, out XmlNamespaceManager ns)
		{
			doc = null;
			ns = null;
			string fullPath = GetPath(manifestPath, androidManifestFileName);
			if (File.Exists(fullPath))
			{
				doc = new XmlDocument();
				doc.Load(fullPath);
				ns = new XmlNamespaceManager(doc.NameTable);
				ns.AddNamespace("android", "http://schemas.android.com/apk/res/android");
			}
		}

		private void SaveManifest(XmlDocument doc)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
				NewLineHandling = NewLineHandling.None,
				CloseOutput = true,
			};
			string fullPath = GetPath(manifestPath, androidManifestFileName);
			XmlWriter xmlwriter = XmlWriter.Create(fullPath, settings);
			doc.Save(xmlwriter);
			xmlwriter.Close();
		}

		internal bool IsGooglePlayServicesIntegrated()
		{
			XmlDocument doc;
			XmlNamespaceManager ns;
			OpenManifest(out doc, out ns);
			
			if (doc != null)
			{
				var applicationNode = doc.SelectSingleNode("/manifest/application");
				var entry = GetXmlDocumentFromString(playServices).FirstChild;
				var nodeName = entry.Name;
				var entryName = entry.Attributes["android:name"].Value;
				
				var node = applicationNode.SelectSingleNode(nodeName + "[@android:name='" + entryName +"']", ns);
				return node != null;
			}
			return false;
		}

		public void AddPlayServicesEntry()
		{
			XmlDocument doc;
			XmlNamespaceManager ns;
			OpenManifest(out doc, out ns);
			
			if (doc != null)
			{
				var applicationNode = doc.SelectSingleNode("/manifest/application");
				if (CheckXmlNodeEntry(doc, ns, applicationNode, "meta-data", new [] {playServices}, false))
					SaveManifest(doc);
			}

		}

		internal struct Manifest
		{
			internal string name;
			internal string[] permissions;
			internal string[] services;
			internal string[] activities;
		}

		internal bool CheckManifestEntries(Manifest manifest)
		{
			//TODO 
//			showConfirmationDialogs && 
			XmlDocument doc;
			XmlNamespaceManager ns;
			OpenManifest(out doc, out ns);
			bool changed = false;

			if (doc != null)
			{
				lock(syncLock)
				{

					if (manifest.permissions != null)
						changed |= CheckPermissions(doc, ns, manifest.permissions);
					if (manifest.activities != null)
						changed |= CheckActivities(doc, ns, manifest.activities);
					if (manifest.services != null)
						changed |= CheckServices(doc, ns, manifest.services);

					if (changed)
					{
						SaveManifest(doc);
						Debug.Log ("Android Manifest was successfuly merged!");
					}
					else
					{
						Debug.Log ("There was nothing to merge on Android Manifest.");
					}
				}
			}
			return changed;
		}


		internal void CheckFyberEntries()
		{
			CheckManifestEntries (fyberManifest);
	    }

	    private bool CheckPermissions(XmlDocument doc, XmlNamespaceManager ns, string[] permissions)
	    {
	        var manifestNode = doc.SelectSingleNode("/manifest");

			return CheckXmlNodeEntry(doc, ns, manifestNode, "uses-permission", permissions, false);
	    }
	    
	    private bool CheckActivities(XmlDocument doc, XmlNamespaceManager ns, string[] activities)
	    {
            bool updated = false;

            var applicationNode = doc.SelectSingleNode("/manifest/application");
            
            updated |= CheckXmlNodeEntry(doc, ns, applicationNode, "activity", activities);
            updated |= RemoveTranslucentTheme(doc, ns, applicationNode);

            return updated;
	    }
	    
	    private bool CheckServices(XmlDocument doc, XmlNamespaceManager ns, string[] services)
	    {
	        var applicationNode = doc.SelectSingleNode("/manifest/application");

			return CheckXmlNodeEntry(doc, ns, applicationNode, "service", services);
	    }

		private bool CheckXmlNodeEntry(XmlDocument doc, XmlNamespaceManager ns, XmlNode parentNode, string nodeName, string[] entries, bool nodeWithChildren = true)
		{
			bool changed = false;

			foreach (string entryString in entries)
			{
				var entry = GetXmlDocumentFromString(entryString).FirstChild;
				var entryName = entry.Attributes["android:name"].Value;
				
				var node = parentNode.SelectSingleNode(nodeName + "[@android:name='" + entryName +"']", ns);
				if (node == null)
				{
					XmlNode imported = doc.ImportNode(entry, true);
					parentNode.AppendChild(imported);
					changed = true;
				}
				else if (nodeWithChildren)
				{
					if (!DeepNodesCheck(entry, node, ns))
					{
						XmlNode imported = doc.ImportNode(entry, true);
						parentNode.ReplaceChild(imported, node);
						changed = true;
					}
				}
			}
			
			return changed;
		}

        private bool RemoveTranslucentTheme(XmlDocument doc, XmlNamespaceManager ns, XmlNode parentNode){
            int counter = 0;

            XmlNodeList themedActivitiesList = parentNode.SelectNodes("//activity[@android:theme]", ns);
            foreach (XmlNode activity in themedActivitiesList){
                XmlAttribute activityTheme = activity.Attributes["android:theme"];
                XmlAttribute activityName = activity.Attributes["android:name"];
                if (activityName != null && activityName.Value.ToLower().Contains("com.fyber") && activityTheme != null && activityTheme.Value.ToLower().Contains("translucent")){
                    activity.Attributes.Remove(activityTheme);
                    counter++;
				}
            }
            Debug.Log("Removed " + counter + " translucent theme attributes from Fyber entries in the AndroidManifest.xml file.");

            return counter > 0;
        }

	    //helpers
	    XmlDocument GetXmlDocumentFromString(string xml) {
            var doc = new XmlDocument();

            // Create the XmlNamespaceManager.
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
            nsmgr.AddNamespace("android", "http://schemas.android.com/apk/res/android");

            // Create the XmlParserContext.
            XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

            // Create the reader. 
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlReader reader = XmlReader.Create(new StringReader(xml), settings, context);

            doc.Load(reader);

            return doc;
	    }

	    private string GetPath(string path1, string path2)
	    {
	        return Path.Combine(Path.Combine(Application.dataPath, path1), path2);
	    }

		private string FindDefaultAndroidManifest()
		{
			string fullManifestPath = string.Empty;
			try 
			{
				string applicationContentsPath = EditorApplication.applicationContentsPath;
				// Starting on Unity version 5.3, on Mac, PlaybackEngines folder exists outside of the application package contents path.
				string unityAppBasePath = Path.Combine(EditorApplication.applicationPath, "../");			
				string[] basePaths = {unityAppBasePath, applicationContentsPath};				

				foreach (string basePath in basePaths)
				{
					foreach (string manifesPath in manifestPaths)
					{				
						fullManifestPath = getFileFromDirectory (Path.Combine(basePath, manifesPath), androidManifestFileName);
						if(!String.IsNullOrEmpty(fullManifestPath)) 
						{
							return fullManifestPath;
						}					
					}
				}		
			}
			catch(Exception e)//in case of unexpected errors due to changes in file structure or permissions
			{
				Debug.LogWarning("An exception ocurred while accessing the defaule manifest: " + e.Message);
			}			
			return fullManifestPath;
		}

		//prevents file not found exception
		private string getFileFromDirectory(string path, string file)
		{
			string fileFound = string.Empty;
			if (File.Exists (Path.Combine(path,file))) 
			{
				string[] filesFound = Directory.GetFiles (path, file, System.IO.SearchOption.AllDirectories);
				if (filesFound.Length > 0)
				{
					fileFound = filesFound[0];
				}
				
			}				
			return fileFound;
		}

		// naive approach
		// node2 should be the one created with the XmlNamespaceManager, otherwise XPath won't work correctly
		private bool DeepNodesCheck(XmlNode node1, XmlNode node2, XmlNamespaceManager ns)
		{
			if (node1.Attributes.Count == node2.Attributes.Count && node1.ChildNodes.Count == node2.ChildNodes.Count)
			{
				bool equals = true;
				var node1XmlString = node1.OuterXml;
				foreach (XmlAttribute attribute in node2.Attributes)
				{
					if (!node1XmlString.Contains(String.Format("{0}=\"{1}\"", attribute.Name, attribute.Value)))
					{
						equals = false;
						break;
					}
				}
				if (equals)
				{
					for (int i = 0; i < node1.ChildNodes.Count  ;i++)
					{
						var node1child = node1.ChildNodes[i];

						//check for attributes size and if > 0
						//assumption - use android name
						string name;
						XmlNode node2child;
						if (node1child.Attributes.Count > 0)
						{
							name = node1child.Attributes["android:name"].Value;
							node2child = node2.SelectSingleNode(node1child.Name+"[@android:name='" + name +"']", ns);
						}
						else
						{
							name = node1child.Name;
							node2child = node2.SelectSingleNode(name, ns);
						}
						if (node2child != null)
						{
							if (!DeepNodesCheck(node1child, node2child, ns))
							{
								equals = false;
								break;
							}
						}
						else
						{
							equals = false;
							break;
						}
					}
				}
				return equals;
			}
			return false;
		}

		//data structure

		readonly Manifest fyberManifest = new Manifest() {
			name = "Fyber",
			permissions = new []{
				@"<uses-permission android:name=""android.permission.INTERNET"" />",
				@"<uses-permission android:name=""android.permission.ACCESS_NETWORK_STATE"" />"},
			activities = new []{
				@"<activity
		            android:name=""com.fyber.unity.ads.OfferWallUnityActivity""
		            android:configChanges=""orientation|screenSize"" />"},
			services = new []{ @"<service android:name=""com.fyber.cache.CacheVideoDownloadService"" android:exported=""false"" />"}
		};

		readonly string playServices = @"<meta-data
				android:name=""com.google.android.gms.version""
				android:value=""@integer/google_play_services_version"" />";
	}

	[CustomEditor(typeof(AndroidManifestChecker))]
	public class AndroiManifestEditor : Editor
	{
		private bool foldout = true;

		readonly GUIContent titleAndroid = new GUIContent("Android Manifest settings", "Specific Android Manifest settings for this project");		

		readonly GUIContent checkOnStart = new GUIContent("Check on Unity start", "Perform an AndroidManifest.xml check on every Unity start");
		readonly GUIContent showDialog = new GUIContent("Show confirmation dialogs", "Always present a confirmation dialog before performing any change");
		readonly GUIContent playServiceMoreInfo = new GUIContent("Get more information about Google Play Services integration","You will be redirected to our developer portal for more information.");

		const string playServicesUrl = "http://developer.fyber.com/content/current/unity/integration/building-your-app";

		SerializedProperty checkOnStartProp;
		SerializedProperty showDialogProp;

		GUIStyle helpBox;
		GUIStyle foldoutStyle;

		AndroidManifestChecker checker;
		Rect rect;

		void OnEnable()
		{						
			checker = target as AndroidManifestChecker;
			if (checker != null)
			{
				checkOnStartProp = serializedObject.FindProperty("checkOnStart");
				showDialogProp = serializedObject.FindProperty("showConfirmationDialogs");
			}
		}

		public override void OnInspectorGUI()
		{
			if (checker == null)
				return;

			bool guiEnablement = GUI.enabled;

			if (helpBox == null)
			{
				//TODO it's fine for now, but we should do it ourselves
				// this is quite brittle as unity may change the names at anytime
				helpBox = GUI.skin.FindStyle("HelpBox");
			}

			if (foldoutStyle == null)
			{
				var style = GUI.skin.FindStyle ("ShurikenModuleTitle");
				if (style != null)
				{
					foldoutStyle = new GUIStyle (style);
					foldoutStyle.fontStyle = FontStyle.Bold;
					foldoutStyle.fontSize = 11;
					foldoutStyle.contentOffset = new Vector2(15f, -3f);
					foldoutStyle.fixedHeight = 24f;
					foldoutStyle.stretchWidth = true;
				}
			}
				
			rect = GUILayoutUtility.GetRect (GetPanelRect().width, 16f);
		
			if (foldoutStyle == null)
				foldout = EditorGUILayout.Foldout(foldout, titleAndroid);
			else
				foldout = GUI.Toggle (rect, foldout, titleAndroid, foldoutStyle);

			if (foldout)
			{
				if (helpBox == null)
					EditorGUILayout.BeginVertical();
				else
					EditorGUILayout.BeginVertical(helpBox);

				EditorGUILayout.PropertyField(checkOnStartProp, checkOnStart);
				EditorGUILayout.PropertyField(showDialogProp, showDialog);

				serializedObject.ApplyModifiedProperties();

				GUI.enabled = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;

				if (GUILayout.Button("Check AndroidManifest"))
				{
					if (checker.CheckAndroidManifestExists())
						checker.CheckFyberEntries();
				}

				if (GUI.enabled && !checker.IsGooglePlayServicesIntegrated())
				{
					EditorGUILayout.HelpBox("Google Play Services is currently not integrated in your Manifest.\n" +
					                        "We strongly recommend its integration.", MessageType.Warning);

					if (GUILayout.Button(playServiceMoreInfo, GUI.skin.label))
						Application.OpenURL(playServicesUrl);
			
					if (GUILayout.Button("Add Google Play Services entry"))
					{
						checker.AddPlayServicesEntry();
						Application.OpenURL(playServicesUrl);
					}
				}
			
				EditorGUILayout.EndVertical ();
			}
			GUILayout.Space(4f);

			GUILayout.Space(10f);
			GUI.enabled = guiEnablement;
		}

		private Rect GetPanelRect()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			return GUILayoutUtility.GetLastRect();
		}

	}	
}