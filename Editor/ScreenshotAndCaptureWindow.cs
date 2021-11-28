using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static Nebukam.Editor.EditorDrawer;

namespace Nebukam.Editor
{
    public class ScreenshotAndCaptureWindow : EditorWindow
    {

        public string ExportFolder = "...";
        public string ExportFileName = "capture";
        public int ExportScale = 1;
        public bool AppendDate = false;

        // Add menu item named "My Window" to the Window menu
        [MenuItem("N:Toolkit/Screenshot & Capture")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(ScreenshotAndCaptureWindow));
        }

        void OnGUI()
        {

            int changes = 0;

            GUILayout.Label("File Settings", EditorStyles.boldLabel);

            changes += PathInput(ref ExportFolder, "Folder :", true);
            changes += TextInput(ref ExportFileName, "Filename :");
            changes += Checkbox(ref AppendDate, "Append date :");

            GUILayout.Label("Export Settings", EditorStyles.boldLabel);
            changes += Slider(ref ExportScale, 1, 10, "Scale");

            if (GUILayout.Button("Take Screenshot")) { TakeScreenshot(); }

            if(changes > 0) { SaveChanges(); }

        }

        protected void TakeScreenshot()
        {
            string d = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            string path = String.Format("{0}/{1}-{2}.png", ExportFolder, ExportFileName, d);
            ScreenCapture.CaptureScreenshot(path, ExportScale);
            SaveChanges();
        }

    }
}
