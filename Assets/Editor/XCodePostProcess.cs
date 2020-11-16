using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
//using UnityEditor.XCodeEditor;
//using System.Xml;
#endif
using System.IO;

public static class XCodePostProcess
{
#if UNITY_EDITOR
    [PostProcessBuild(100)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
        {
            Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
            return;
        }

        //得到xcode工程的路径
        string path = Path.GetFullPath(pathToBuiltProject);
        
        string projPath = PBXProject.GetPBXProjectPath (pathToBuiltProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));
        string targetGUID = proj.TargetGuidByName("Unity-iPhone");
        
        proj.SetBuildProperty(targetGUID, "ENABLE_BITCODE", "NO");
        proj.SetBuildProperty(targetGUID,"ARCHS", "arm64");
        proj.SetBuildProperty(targetGUID,"VALID_ARCHS", "arm64");
        proj.SetBuildProperty(targetGUID,"GCC_OPTIMIZATION_LEVEL", "s");
        proj.SetBuildProperty(targetGUID,"DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");
        

        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        
        plist.root.SetString("GADApplicationIdentifier", "ca-app-pub-0000000000000000~0000000000");
        plist.root.SetBoolean("Application has localized display name", true);
        plist.root.SetString("AppLovinSdkKey", "Fs-cUqJfRU6DI-3nHAtCUubM2g2mHMT4kl_2_v9IyohMfXicNfA0eEwvSJ6gvrtpXtmu2TpTdL-QrLAMqwaXPS");
        plist.WriteToFile(plistPath);
        
        string unityappControllerPath = pathToBuiltProject + "/Classes/UnityAppController.mm";
        XClass UnityAppController = new XClass(unityappControllerPath);
        UnityAppController.WriteBelow("#import \"UnityAppController.h\"", "#import <AdSupport/AdSupport.h>");
       //afid获取添加
        UnityAppController.WriteBelow("@end\n", "extern \"C\" const char * Getidfa(){\n NSString *idfa = [[[ASIdentifierManager sharedManager]advertisingIdentifier]UUIDString];\nreturn strdup([idfa UTF8String]);\n}\n");
    
       File.WriteAllText(projPath, proj.WriteToString());

    }
#endif
}

public partial class XClass : System.IDisposable
{

    private string filePath;

    public XClass(string fPath)
    {
        filePath = fPath;
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError(filePath + "路径下文件不存在");
            return;
        }
    }


    public void WriteBelow(string below, string text)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();

        int beginIndex = text_all.IndexOf(below);
        if (beginIndex == -1)
        {
            Debug.LogError(filePath + "中没有找到标致" + below);
            return;
        }

        int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);

        text_all = text_all.Substring(0, endIndex) + "\n" + text + "\n" + text_all.Substring(endIndex);

        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(text_all);
        streamWriter.Close();
    }

    public void WriteAbove(string above,string text)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();

        int beginIndex = text_all.IndexOf(above);
        if (beginIndex == -1)
        {
            Debug.LogError(filePath + "中没有找到标致" + above);
            return;
        }

        //int endIndex = text_all.LastIndexOf("\n", beginIndex + above.Length);

        text_all = text_all.Substring(0, beginIndex) + "\n" + text + "\n" + text_all.Substring(beginIndex);

        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(text_all);
        streamWriter.Close();
    }

    public void Replace(string below, string newText)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();

        int beginIndex = text_all.IndexOf(below);
        if (beginIndex == -1)
        {
            Debug.LogError(filePath + "中没有找到标致" + below);
            return;
        }

        text_all = text_all.Replace(below, newText);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(text_all);
        streamWriter.Close();

    }

    public void Dispose()
    {

    }
}