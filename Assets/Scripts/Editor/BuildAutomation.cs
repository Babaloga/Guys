using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using Unity.SharpZipLib.Utils;

class BuildAutomation : EditorWindow
{
    int majorVersionNumber;
    int minorVersionNumber;

    [MenuItem("Window/Build Automation")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BuildAutomation));
    }

    void CreateGUI()
    {
        string rawVersionNumber = File.ReadAllText("Assets/VersionNumber.txt");
        string[] rawSplit = rawVersionNumber.Split("\n");
        majorVersionNumber = int.Parse(rawSplit[0]);
        minorVersionNumber = int.Parse(rawSplit[1]);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField(string.Format("Current Version: {0}.{1}", majorVersionNumber, minorVersionNumber));
        
        if(GUILayout.Button("Build Major Version"))
        {
            majorVersionNumber++;
            minorVersionNumber = 0;
            File.WriteAllText("Assets/VersionNumber.txt", majorVersionNumber + "\n" + minorVersionNumber);
            BuildWindowsAndLinux();
        }
        if (GUILayout.Button("Build Minor Version"))
        {
            minorVersionNumber++;
            File.WriteAllText("Assets/VersionNumber.txt", majorVersionNumber + "\n" + minorVersionNumber);
            BuildWindowsAndLinux();
        }
        if (GUILayout.Button("Build Without Incrementing Version"))
        {
            BuildWindowsAndLinux();
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("WebGL Client"))
        {
            BuildWebGLClient();
        }
        if (GUILayout.Button("Windows Client"))
        {
            BuildWindowsClient();
        }
        if (GUILayout.Button("Linux Client"))
        {
            BuildLinuxClient();
        }
        if (GUILayout.Button("Mac Client"))
        {
            BuildMacClient();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Windows Server"))
        {
            BuildWindowsServer();
        }

        if (GUILayout.Button("Linux Server"))
        {
            BuildLinuxServer();
        }

        GUILayout.EndHorizontal();
    }

    private void BuildWindowsAndLinux()
    {
        BuildWebGLClient();
        BuildWindowsServer();
        BuildWindowsClient();
        BuildLinuxClient();
        BuildLinuxServer();
    }

    private void BuildWindowsServer()
    {
        if(EditorSceneManager.GetActiveScene().name != "SampleScene")
        {
            EditorSceneManager.LoadScene("Scenes/SampleScene");
        }
        HelloWorldManager netUI = GameObject.Find("Network UI").GetComponent<HelloWorldManager>();
        netUI.server = true;
        netUI.host = false;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        string name = string.Format("WindowsServer{0}.{1}", majorVersionNumber, minorVersionNumber);
        string path = "Builds/" + name;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = path + "/" + name + ".exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Windows Server Build succeeded: " + summary.totalSize + " bytes");
            ZipBuild(name, path);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Windows Server Build failed");
        }
    }

    private void BuildLinuxServer()
    {
        if (EditorSceneManager.GetActiveScene().name != "SampleScene")
        {
            EditorSceneManager.LoadScene("Scenes/SampleScene");
        }
        HelloWorldManager netUI = GameObject.Find("Network UI").GetComponent<HelloWorldManager>();
        netUI.server = true;
        netUI.host = false;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        string name = string.Format("LinuxServer{0}.{1}", majorVersionNumber, minorVersionNumber);
        string path = "Builds/" + name;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = path + "/" + name + ".x86_64";
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Linux Server Build succeeded: " + summary.totalSize + " bytes");
            ZipBuild(name, path);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Linux Server Build failed");
        }
    }

    private void BuildWindowsClient()
    {
        if (EditorSceneManager.GetActiveScene().name != "SampleScene")
        {
            EditorSceneManager.LoadScene("Scenes/SampleScene");
        }
        HelloWorldManager netUI = GameObject.Find("Network UI").GetComponent<HelloWorldManager>();
        netUI.server = false;
        netUI.host = false;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        string name = string.Format("WindowsClient{0}.{1}", majorVersionNumber, minorVersionNumber);
        string path = "Builds/" + name;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = path + "/Guys.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Windows Client Build succeeded: " + summary.totalSize + " bytes");
            ZipBuild(name, path);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Windows Client Build failed");
        }
    }

    private void BuildWebGLClient()
    {
        if (EditorSceneManager.GetActiveScene().name != "SampleScene")
        {
            EditorSceneManager.LoadScene("Scenes/SampleScene");
        }
        HelloWorldManager netUI = GameObject.Find("Network UI").GetComponent<HelloWorldManager>();
        netUI.server = false;
        netUI.host = false;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        string name = string.Format("WebGLClient{0}.{1}", majorVersionNumber, minorVersionNumber);
        string path = "Builds/" + name;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = path + "/Guys.exe";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("WebGL Client Build succeeded: " + summary.totalSize + " bytes");
            ZipBuild(name, path);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("WebGL Client Build failed");
        }
    }

    private void BuildLinuxClient()
    {
        if (EditorSceneManager.GetActiveScene().name != "SampleScene")
        {
            EditorSceneManager.LoadScene("Scenes/SampleScene");
        }
        HelloWorldManager netUI = GameObject.Find("Network UI").GetComponent<HelloWorldManager>();
        netUI.server = false;
        netUI.host = false;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        string name = string.Format("LinuxClient{0}.{1}", majorVersionNumber, minorVersionNumber);
        string path = "Builds/" + name;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = path + "/Guys.x86_64";
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Linux Client Build succeeded: " + summary.totalSize + " bytes");
            ZipBuild(name, path);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Linux Client Build failed");
        }
    }

    private void BuildMacClient()
    {
        if (EditorSceneManager.GetActiveScene().name != "SampleScene")
        {
            EditorSceneManager.LoadScene("Scenes/SampleScene");
        }
        HelloWorldManager netUI = GameObject.Find("Network UI").GetComponent<HelloWorldManager>();
        netUI.server = false;
        netUI.host = false;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        string name = string.Format("MacClient{0}.{1}", majorVersionNumber, minorVersionNumber);
        string path = "Builds/" + name;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = path + "/Guys.app";
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Mac Client Build succeeded: " + summary.totalSize + " bytes");
            ZipBuild(name, path);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Mac Client Build failed");
        }
    }

    private void ZipBuild(string name, string path)
    {
        Directory.CreateDirectory("Builds/Zips");
        ZipUtility.CompressFolderToZip(string.Format("Builds/Zips/{0}.zip", name), null, path);
    }
}
