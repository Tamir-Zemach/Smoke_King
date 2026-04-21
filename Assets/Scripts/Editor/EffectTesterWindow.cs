using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EffectTesterWindow : EditorWindow
    {
        private GameObject _targetObject;

        private Component[] _components;
        private string[] _componentNames;
        private int _selectedComponentIndex = -1;
        private int _previousComponentIndex = -1;

        private List<MethodInfo> _publicMethods = new List<MethodInfo>();
        private string[] _methodNames;
        private int _selectedMethodIndex = -1;

        [MenuItem("Tools/Effect Tester")]
        public static void ShowWindow()
        {
            GetWindow<EffectTesterWindow>("Effect Tester");
        }

        private void OnGUI()
        {
            GUILayout.Label("Effect Tester", EditorStyles.boldLabel);

            // Drag & Drop field
            _targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", _targetObject, typeof(GameObject), true);

            if (_targetObject == null)
            {
                EditorGUILayout.HelpBox("Drag a GameObject here to begin.", MessageType.Info);
                return;
            }

            GUILayout.Space(10);

            // SCAN COMPONENTS
            if (GUILayout.Button("Scan Components"))
            {
                ScanComponents();
            }

            if (_components != null && _components.Length > 0)
            {
                _selectedComponentIndex = EditorGUILayout.Popup("Component", _selectedComponentIndex, _componentNames);

                // Auto-scan methods when component changes
                if (_selectedComponentIndex != _previousComponentIndex)
                {
                    _previousComponentIndex = _selectedComponentIndex;
                    ScanMethods();
                }
            }

            GUILayout.Space(10);

            if (_publicMethods.Count > 0)
            {
                _selectedMethodIndex = EditorGUILayout.Popup("Method", _selectedMethodIndex, _methodNames);

                GUILayout.Space(10);

                if (GUILayout.Button("Play Method"))
                {
                    InvokeSelectedMethod();
                }
            }
        }

        private void ScanComponents()
        {
            _components = _targetObject.GetComponents<Component>();

            _componentNames = new string[_components.Length];
            for (int i = 0; i < _components.Length; i++)
            {
                _componentNames[i] = _components[i].GetType().Name;
            }

            _selectedComponentIndex = 0;
            _previousComponentIndex = -1;

            _publicMethods.Clear();
            _selectedMethodIndex = -1;
        }

        private void ScanMethods()
        {
            _publicMethods.Clear();

            Component comp = _components[_selectedComponentIndex];
            System.Type type = comp.GetType();

            // Only methods declared in THIS script (not inherited)
            MethodInfo[] methods = type.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.DeclaredOnly
            );

            foreach (var method in methods)
            {
                if (method.GetParameters().Length == 0)
                {
                    _publicMethods.Add(method);
                }
            }

            _methodNames = new string[_publicMethods.Count];
            for (int i = 0; i < _publicMethods.Count; i++)
            {
                _methodNames[i] = _publicMethods[i].Name;
            }

            _selectedMethodIndex = _publicMethods.Count > 0 ? 0 : -1;
        }

        private void InvokeSelectedMethod()
        {
            if (_selectedMethodIndex < 0) return;

            Component comp = _components[_selectedComponentIndex];
            MethodInfo method = _publicMethods[_selectedMethodIndex];

            method.Invoke(comp, null);

            Debug.Log($"Invoked: {comp.GetType().Name}.{method.Name}");
        }
    }
}
