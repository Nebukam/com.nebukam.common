using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Nebukam.Editor
{
    public static class EditorExtensions
    {

        #region bool

        public static int Checkbox(ref bool value, string label)
        {
            bool input = EditorGUILayout.Toggle(label, value);
            if (input == value) { return 0; }
            value = input;
            return 1;
        }

        #endregion

        #region string

        public static int TextInput(ref string value, string label = "")
        {
            string input;
            if (label != "")
                input = EditorGUILayout.TextField(label, value);
            else
                input = EditorGUILayout.TextField(value);

            if (input == value) { return 0; }
            value = input;
            return 1;
        }

        public static int PathInput(ref string value, string label = "", bool folder = false)
        {

            GUILayout.BeginHorizontal();

            int result = TextInput(ref value, label);

            if (folder)
            {
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    string input = EditorUtility.OpenFolderPanel("Pick a folder...", "...", "");
                    value = input;
                }
            }
            else if (result == 0)
            {
                GUILayout.EndHorizontal();
                return 0;
            }

            value = value.Replace("\\", "/");

            if (folder && value.Length > 0 && value.Substring(value.Length - 1) != "/")
                value += "/";

            GUILayout.EndHorizontal();

            return 1;

        }

        #endregion

        #region int

        public static int Slider(ref int value, int min, int max, string label = "")
        {
            int input;
            if (label != "")
                input = EditorGUILayout.IntSlider(label, value, min, max);
            else
                input = EditorGUILayout.IntSlider(value, min, max);

            if (input == value) { return 0; }
            value = input;
            return 1;
        }

        #endregion

    }
}
