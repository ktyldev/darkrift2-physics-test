using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildWindow : EditorWindow
{
    private const string _serverBuildPath = "Build/Server";
    private const string _clientBuildPath = "Build/Client";
    private const string _serverScene = "Server";
    private const string _clientScene = "Client";
    private const string _buildName = "DR2PT";

    public enum BuildMode
    {
        Server,
        Client,
        All
    }

    private BuildMode _buildMode;

    [MenuItem("DR2PT/Build Player")]
    private static void Init()
    {
        var window = GetWindow<BuildWindow>();
        window.titleContent = new GUIContent("Build Player");

        window.Show();
    }

    private void OnGUI()
    {
        _buildMode = (BuildMode)EditorGUILayout.EnumPopup(_buildMode);

        bool build = GUILayout.Button("Build");

        if (build)
        {
            switch (_buildMode)
            {
                case BuildMode.Server:
                    BuildServer();
                    break;
                case BuildMode.Client:
                    BuildClient();
                    break;
                case BuildMode.All:
                    BuildAll();
                    break;
                default:
                    break;
            }
        }
    }

    private void BuildAll()
    {
        Debug.Log(":: Building all");
        BuildServer();
        BuildClient();
    }

    private void BuildServer()
    {
        Debug.Log(":: Building server");

        PerformBuild(BuildMode.Server, BuildTarget.StandaloneWindows, BuildOptions.EnableHeadlessMode);
    }

    private void BuildClient()
    {
        Debug.Log(":: Building client");

        PerformBuild(BuildMode.Client, BuildTarget.StandaloneWindows);
    }

    private static string[] GetEnabledScenes(BuildMode mode)
    {
        string omitScene = null;
        switch (mode)
        {
            case BuildMode.Server:
                omitScene = _clientScene;
                break;
            case BuildMode.Client:
                omitScene = _serverScene;
                break;
            default:
                throw new System.Exception();
        }

        return EditorBuildSettings.scenes
            // get all enabled scenes...
            .Where(s => s.enabled)
            .Select(s => s.path)
            // ...that have been saved at least once...
            .Where(s => !string.IsNullOrEmpty(s))
            // ...omitting the relevant master scene
            .Where(s => !s.Contains(omitScene))
            .ToArray();
    }

    private static string GetBuildName(BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Server:
                return _buildName + "_Server";

            case BuildMode.Client:
                return _buildName + "_Client";

            default:
                throw new System.Exception();
        }
    }

    private static string GetBuildPath(BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Server:
                return _serverBuildPath;

            case BuildMode.Client:
                return _clientBuildPath;

            case BuildMode.All:
                throw new System.Exception("Can't provide path for build mode \"All\"");

            default:
                throw new System.Exception("😬");
        }
    }

    private static string GetFixedBuildPath(BuildTarget buildTarget, string buildPath, string buildName)
    {
        if (buildTarget.ToString().ToLower().Contains("windows"))
        {
            buildName += ".exe";
        }

        return Path.Combine(buildPath, buildName);
    }

    private static void PerformBuild(
        BuildMode buildMode,
        BuildTarget buildTarget,
        BuildOptions buildOptions = BuildOptions.None)
    {
        Debug.Log(":: Performing build");

        var scenes = GetEnabledScenes(buildMode);
        Debug.Log(":: Scenes in build: " + string.Join(", ", scenes));
        var buildPath = GetBuildPath(buildMode);
        var buildName = GetBuildName(buildMode);

        string fixedBuildPath = GetFixedBuildPath(buildTarget, buildPath, buildName);

        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            locationPathName = fixedBuildPath,
            options = buildOptions,
            scenes = scenes,
            target = buildTarget
        });

        Debug.Log(":: Build completed");
    }
}
