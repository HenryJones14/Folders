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
    public class Folder : MonoBehaviour, IProcessSceneWithReport
#else
	[DisallowMultipleComponent]
	public class Folder : MonoBehaviour
#endif
	{
#if UNITY_EDITOR
		public int callbackOrder { get { return 0; } }
		public void OnProcessScene(Scene Scene, BuildReport Report)
		{
			foreach (Folder folder in FindObjectsOfType<Folder>())
			{
				DestroyImmediate(folder);
			}
		}

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

		/*private List<Type> GetDependencies(Type Root)        // Dependency graveyard
		{
			List<Type> types = new List<Type>();

			foreach (RequireComponent required in Attribute.GetCustomAttributes(Root, typeof(RequireComponent), true))
			{
				if (required.m_Type0 != null && required.m_Type0 != typeof(Transform) && required.m_Type0 != typeof(RectTransform))
				{
					GetDependencies(required.m_Type0, ref types);
					types.Add(required.m_Type0);
				}
				if (required.m_Type1 != null && required.m_Type1 != typeof(Transform) && required.m_Type1 != typeof(RectTransform))
				{
					GetDependencies(required.m_Type1, ref types);
					types.Add(required.m_Type1);
				}
				if (required.m_Type2 != null && required.m_Type2 != typeof(Transform) && required.m_Type2 != typeof(RectTransform))
				{
					GetDependencies(required.m_Type2, ref types);
					types.Add(required.m_Type2);
				}
			}

			return types;
		}

		private void GetDependencies(Type Root, ref List<Type> Types)
		{
			foreach (RequireComponent required in Attribute.GetCustomAttributes(Root, typeof(RequireComponent), true))
			{
				if (required.m_Type0 != null)
				{
					GetDependencies(required.m_Type0, ref Types);
					Types.Add(required.m_Type0);
				}
				if (required.m_Type1 != null)
				{
					GetDependencies(required.m_Type1, ref Types);
					Types.Add(required.m_Type1);
				}
				if (required.m_Type2 != null)
				{
					GetDependencies(required.m_Type2, ref Types);
					Types.Add(required.m_Type2);
				}
			}
		}*/
#endif
	}
}
