#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Folders
{
	public static class Icon
	{
		private static Texture2D _open;
		public static Texture2D NormalOpen
		{
			get
			{
				if (_open == null)
				{
					_open = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gameboxinteractive.folders/Assets/FolderOpened.png");
				}

				return _open;
			}
		}

		private static Texture2D _closed;
		public static Texture2D NormalClosed
		{
			get
			{
				if (_closed == null)
				{
					_closed = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gameboxinteractive.folders/Assets/Folder.png");
				}

				return _closed;
			}
		}

		private static Texture2D _editorOpen;
		public static Texture2D EditorOpen
		{
			get
			{
				if (_editorOpen == null)
				{
					_editorOpen = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gameboxinteractive.folders/Assets/EditorFolderOpened.png");
				}

				return _editorOpen;
			}
		}

		private static Texture2D _editorClosed;
		public static Texture2D EditorClosed
		{
			get
			{
				if (_editorClosed == null)
				{
					_editorClosed = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gameboxinteractive.folders/Assets/EditorFolder.png");
				}

				return _editorClosed;
			}
		}
	}
}

#endif