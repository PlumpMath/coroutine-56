using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class AutoBuilder {

	static string GetProjectName()
	{
		string[] s = Application.dataPath.Split('/');
		return s[s.Length - 2];
	}

	static string[] GetScenePaths()
	{
		/*
		string[] scenes = new string[EditorBuildSettings.scenes.Length];

		for(int i = 0; i < scenes.Length; i++)
		{
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}

		return scenes;
		*/
		var scenes = new List<string>();

		foreach (var scene in EditorBuildSettings.scenes)
		{
			if (scene == null)
				continue;
			if (scene.enabled)
				scenes.Add(scene.path);
		}
		return scenes.ToArray();
	}

	[MenuItem("File/AutoBuilder/iOS")]
	static void PerformiOSBuild ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
		string targetDir = "Builds/"+GetProjectName();
		BuildPipeline.BuildPlayer(GetScenePaths(), targetDir,BuildTarget.iOS,BuildOptions.None);
	}

	[MenuItem("File/AutoBuilder/iOSDevelopment")]
	static void PerformiOSBuildDevelopment ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
		string targetDir = "Builds/"+GetProjectName();
		BuildPipeline.BuildPlayer(GetScenePaths(), targetDir,BuildTarget.iOS,BuildOptions.Development);
	}

	[MenuItem("File/AutoBuilder/Android")]
	static void PerformAndroidBuild ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		string targetDir = "Builds/"+GetProjectName() + ".apk";
		BuildPipeline.BuildPlayer(GetScenePaths(), targetDir,BuildTarget.Android,BuildOptions.None);
	}

	[MenuItem("File/AutoBuilder/AndroidDevelopment")]
	static void PerformAndroidBuildDevelopment ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		string targetDir = "Builds/"+GetProjectName() + ".apk";
		BuildPipeline.BuildPlayer(GetScenePaths(), targetDir,BuildTarget.Android,BuildOptions.Development);
	}
}