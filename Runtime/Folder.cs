using UnityEngine;


#if UNITY_EDITOR

using UnityEngine.SceneManagement;
using System.Linq;

using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEditor;

#endif


namespace Folders
{
#if UNITY_EDITOR
	[DisallowMultipleComponent, ExecuteAlways]
#else
	[DisallowMultipleComponent]
#endif
	public class Folder : MonoBehaviour
	{
#if UNITY_EDITOR

		private void Awake()
		{
			ResetTransform();
			CheckDependencies();
		}

		private void Update()
		{
			ResetTransform();
			CheckDependencies();
		}

		private void OnDisable()
		{
			enabled = true;
		}

		private void OnDestroy()
		{
			EditorGUIUtility.SetIconForObject(gameObject, null);
		}

		private void ResetTransform()
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		private void CheckDependencies()
		{
			if (Application.isPlaying || this == null)
			{
				return;
			}
			else
			{
				Component[] components = GetComponents<Component>().Where(component => component != this && !typeof(Transform).IsAssignableFrom(component.GetType())).ToArray();

				if (components == null || components.Length <= 0)
				{
					return;
				}
				else
				{
					EditorUtility.DisplayDialog("Can't add a \"Folder\" script", "Folder can't be used with other components. Please go remove them first", "ok");
					DestroyImmediate(this);
				}
			}
		}
#endif
	}

#if UNITY_EDITOR
	public class FolderCleanup : IProcessSceneWithReport
	{
		public int callbackOrder { get { return 0; } }
		public void OnProcessScene(Scene Scene, BuildReport Report)
		{
			foreach (Folder folder in Object.FindObjectsOfType<Folder>())
			{
				Object.DestroyImmediate(folder);
			}
		}
	}
#endif

}