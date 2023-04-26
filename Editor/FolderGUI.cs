using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Folders
{
	[CustomEditor(typeof(Folder))]
    public class FolderGUI : Editor
    {
		GameObject gameObject;

		public void OnEnable()
		{
			gameObject = ((MonoBehaviour)target).gameObject;
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Label("You can define \"ENABLE_HIERARCHY_OPTIMIZATIONS\" to make hierarchy icons update only once per frame.\nThis can cause some issues with icons not updating!\n");

			GUILayout.BeginHorizontal();
			string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			Color col = GUI.contentColor;

			if (symbols.Contains("ENABLE_HIERARCHY_OPTIMIZATIONS"))
			{
				GUI.contentColor = new Color32(100, 255, 100, 255);
				GUILayout.Label("\"ENABLE_HIERARCHY_OPTIMIZATIONS\" is defined", GUILayout.Width(300));
			}
			else
			{
				GUI.contentColor = new Color32(255, 100, 100, 255);
				GUILayout.Label("\"ENABLE_HIERARCHY_OPTIMIZATIONS\" is undefined", GUILayout.Width(310));
			}

			GUI.contentColor = col;

			if (gameObject.CompareTag("EditorOnly"))
			{
				if (GUILayout.Button("Switch to Normal folder"))
				{
					gameObject.tag = "Untagged";
				}
			}
			else if (gameObject.CompareTag("Untagged"))
			{
				if (GUILayout.Button("Switch to Editor folder"))
				{
					gameObject.tag = "EditorOnly";
				}
			}
			else
			{
				gameObject.tag = "EditorOnly";
			}

			if (GUILayout.Button("Remove Folder"))
			{
				DestroyImmediate(target);
			}

			GUILayout.EndHorizontal();
		}
	}
}
