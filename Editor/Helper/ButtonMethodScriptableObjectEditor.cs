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
	public class ButtonMethodScriptableObjectEditor : Editor
	{
		private List<MethodInfo> _methods;
		private ScriptableObject _target;
		private AddressablesImporterOdinInspectorDrawer _drawer;

		private void OnEnable()
		{
			_target = target as ScriptableObject;
			_drawer = _drawer ?? new AddressablesImporterOdinInspectorDrawer();
			if (_target == null) return;
			
			_drawer.Initialize(target);
			_methods = AddressablesImportMethodHandler.CollectValidMembers(_target.GetType());
		}

		private void OnDisable()
		{
			_drawer.Dispose();
		}

		public override void OnInspectorGUI()
		{
			DrawBaseEditor();
			
			if (_methods == null) return;

			AddressablesImportMethodHandler.OnInspectorGUI(_target, _methods);
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