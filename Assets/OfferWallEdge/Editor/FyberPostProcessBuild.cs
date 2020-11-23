using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

#if UNITY_IPHONE || UNITY_IOS

using UnityEditor.iOS.Xcode;

public class FyberPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        PBXProject project = new PBXProject();
        string sPath = PBXProject.GetPBXProjectPath(path);
        project.ReadFromFile(sPath);

        string targetGUID;
#if UNITY_2019_3_OR_NEWER
        targetGUID = project.GetUnityFrameworkTargetGuid();
#else
        string targetName = PBXProject.GetUnityTargetName();
        targetGUID = project.TargetGuidByName(targetName);
#endif

        project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");
        File.WriteAllText(sPath, project.WriteToString());
    }
}

#endif
