// #define ENABLE_HIERARCHY_OPTIMIZATIONS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Folders
{
    public static class FolderEditor
    {
		private const BindingFlags _FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

#if ENABLE_HIERARCHY_OPTIMIZATIONS
		private static bool _hasProcessedFrame;
#endif

		private static bool _isInitialized;
		private static bool _folderSelected;
		private static Tool _lastTool;

		// Reflected members
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special naming scheme")]
		private static PropertyInfo property_sceneHierarchy;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special naming scheme")]
		private static PropertyInfo property_treeView;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special naming scheme")]
		private static PropertyInfo property_data;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special naming scheme")]
		private static PropertyInfo property_selectedIcon;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special naming scheme")]
		private static PropertyInfo property_objectPPTR;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special naming scheme")]
		private static MethodInfo method_getRows;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special naming scheme")]
		private static MethodInfo method_isExpanded;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special naming scheme")]
		private static MethodInfo method_getAllSceneHierarchyWindows;


		[MenuItem("GameObject/Normal Folder", false, -2)]
		private static void CreateNormalFolder()
		{
			GameObject obj = EditorUtility.CreateGameObjectWithHideFlags("Normal Folder", HideFlags.None, typeof(Folder));
			obj.tag = "Untagged";

			if (Selection.activeTransform != null)
			{
				obj.transform.parent = Selection.activeTransform;
			}

			Selection.activeObject = obj;
		}

		[MenuItem("GameObject/Editor Folder", false, -1)]
		private static void CreateEditorFolder()
		{
			GameObject obj = EditorUtility.CreateGameObjectWithHideFlags("Editor Folder", HideFlags.None, typeof(Folder));
			obj.tag = "EditorOnly";

			if (Selection.activeTransform != null)
			{
				obj.transform.parent = Selection.activeTransform;
			}

			Selection.activeObject = obj;
		}

		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			EditorApplication.hierarchyWindowItemOnGUI += RefreshFolderIcons;
			EditorApplication.update += RefreshFolderReflection;
			EditorApplication.update += UpdateSelectedTool;
			Selection.selectionChanged += CheckToolChange;

			CheckToolChange();
			UpdateSelectedTool();
		}

		private static void RefreshFolderReflection()
		{
#if ENABLE_HIERARCHY_OPTIMIZATIONS
			_hasProcessedFrame = false;
#endif

			if (!_isInitialized)
			{
				try
				{
					var assembly = typeof(SceneView).Assembly;

					var type_sceneHierarchyWindow = assembly.GetType("UnityEditor.SceneHierarchyWindow");
					method_getAllSceneHierarchyWindows = type_sceneHierarchyWindow.GetMethod("GetAllSceneHierarchyWindows", _FLAGS);
					property_sceneHierarchy = type_sceneHierarchyWindow.GetProperty("sceneHierarchy");

					var type_sceneHierarchy = assembly.GetType("UnityEditor.SceneHierarchy");
					property_treeView = type_sceneHierarchy.GetProperty("treeView", _FLAGS);

					var type_treeViewController = assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewController");
					property_data = type_treeViewController.GetProperty("data", _FLAGS);

					var type_iTreeViewDataSource = assembly.GetType("UnityEditor.IMGUI.Controls.ITreeViewDataSource");
					method_getRows = type_iTreeViewDataSource.GetMethod("GetRows");
					method_isExpanded = type_iTreeViewDataSource.GetMethod("IsExpanded", new Type[] { typeof(TreeViewItem) });

					var type_gameObjectTreeViewItem = assembly.GetType("UnityEditor.GameObjectTreeViewItem");
					property_selectedIcon = type_gameObjectTreeViewItem.GetProperty("selectedIcon", _FLAGS);
					property_objectPPTR = type_gameObjectTreeViewItem.GetProperty("objectPPTR", _FLAGS);

					_isInitialized = true;
				}
				catch (Exception exception)
				{
					Debug.LogError("Reflection failed:");
					Debug.LogException(exception);
				}
			}
		}

		private static void RefreshFolderIcons(int InstanceID, Rect SelectionRect)
		{
#if ENABLE_HIERARCHY_OPTIMIZATIONS
			if (_hasProcessedFrame || !_isInitialized)
			{
				return;
			}
			else
			{
				_hasProcessedFrame = true;
			}
#else
			if (!_isInitialized)
			{
				return;
			}
#endif

			foreach (EditorWindow hierarchy in ((IEnumerable)method_getAllSceneHierarchyWindows.Invoke(null, null)).Cast<EditorWindow>())
			{
				object sceneHierarchy = property_sceneHierarchy.GetValue(hierarchy);
				object treeView = property_treeView.GetValue(sceneHierarchy);
				object data = property_data.GetValue(treeView);

				foreach (TreeViewItem item in (IList<TreeViewItem>)method_getRows.Invoke(data, null))
				{
					GameObject itemObject = (Object)property_objectPPTR.GetValue(item) as GameObject;
					bool isExpanded = (bool)method_isExpanded.Invoke(data, new object[] { item });

					if (itemObject != null && itemObject.TryGetComponent(out Folder folder))
					{
						if (itemObject.CompareTag("EditorOnly"))
						{
							item.icon = isExpanded ? Icon.EditorOpen : Icon.EditorClosed;
							property_selectedIcon.SetValue(item, isExpanded ? Icon.EditorOpen : Icon.EditorClosed);
							EditorGUIUtility.SetIconForObject(itemObject, Icon.EditorClosed);
						}
						else
						{
							item.icon = isExpanded ? Icon.NormalOpen : Icon.NormalClosed;
							property_selectedIcon.SetValue(item, isExpanded ? Icon.NormalOpen : Icon.NormalClosed);
							EditorGUIUtility.SetIconForObject(itemObject, Icon.NormalClosed);
						}
					}
				}
			}
		}

		private static void CheckToolChange()
		{
			_folderSelected = false;
			foreach (GameObject obj in Selection.gameObjects)
			{
				if (obj != null && obj.TryGetComponent(out Folder folder))
				{
					_folderSelected = true;
					break;
				}
			}
		}

		public static void UpdateSelectedTool()
		{
			if (_folderSelected)
			{
				if (_lastTool == Tool.None)
				{
					_lastTool = Tools.current;
					Tools.current = Tool.None;
				}
				else
				{
					Tools.current = Tool.None;
				}
			}
			else
			{
				if (_lastTool != Tool.None)
				{
					Tools.current = _lastTool;
					_lastTool = Tool.None;
				}
				/*else
				{
				}*/
			}
		}
	}
}
