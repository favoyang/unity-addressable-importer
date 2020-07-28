/// <summary>
/// ButtonMethodAttribute,
/// modified from https://github.com/Deadcows/MyBox/blob/master/Attributes/ButtonMethodAttribute.cs
/// </summary>
using UnityEngine;

namespace UnityAddressableImporter.Helper.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Editor.Helper;
	using UnityEditor;


	[CustomEditor(typeof(AddressableImportSettings), true), CanEditMultipleObjects]
	public class AddressableImportSettingsEditor : Editor
	{
		private List<MethodInfo> _methods;
		private AddressableImportSettings _target;
		private AddressableImporterOdinHandler _drawer;
		

		private void OnEnable()
		{
			_target = target as AddressableImportSettings;
			_drawer = _drawer ?? new AddressableImporterOdinHandler();
			if (_target == null) return;
			
			_drawer.Initialize(_target);
			_methods = AddressableImporterMethodHandler.CollectValidMembers(_target.GetType());

		}

		private void OnDisable()
		{
			_drawer.Dispose();
		}

		public override void OnInspectorGUI()
		{
			DrawBaseEditor();

#if !ODIN_INSPECTOR
			if (_methods == null) return;

			AddressableImporterMethodHandler.OnInspectorGUI(_target, _methods);
#endif

			serializedObject.ApplyModifiedProperties();

		}

		private void DrawBaseEditor()
		{
#if ODIN_INSPECTOR
			_drawer.Draw();
#else
			base.OnInspectorGUI();
#endif
		}
	}
}