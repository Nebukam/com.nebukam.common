using System.Collections.Generic;
using UnityEngine;

namespace Nebukam
{

    public class Static : MonoBehaviour
    {

        public delegate void Callback();

        private static List<Callback> m_OnUpdate = new List<Callback>();
        private static List<Callback> m_OnLateUpdate = new List<Callback>();
        private static List<Callback> m_OnFixedUpdate = new List<Callback>();
        private static List<Callback> m_OnQuit = new List<Callback>();

        public static bool quitting = false;

        [RuntimeInitializeOnLoadMethod]
        static void RunOnStart()
        {
            GameObject global = new GameObject();
            DontDestroyOnLoad(global);

            global.hideFlags = HideFlags.HideInHierarchy;
            global.AddComponent<Static>();
        }

        public static void onUpdate(Callback func) { ON(func, m_OnUpdate); }
        public static void offUpdate(Callback func) { OFF(func, m_OnUpdate); }

        public static void onLateUpdate(Callback func) { ON(func, m_OnLateUpdate); }
        public static void offLateUpdate(Callback func) { OFF(func, m_OnLateUpdate); }

        public static void onFixedUpdate(Callback func) { ON(func, m_OnFixedUpdate); }
        public static void offFixedUpdate(Callback func) { OFF(func, m_OnFixedUpdate); }

        public static void onQuit(Callback func) { if (quitting) { return; } ON(func, m_OnQuit); }
        public static void offQuit(Callback func) { if (quitting) { return; } OFF(func, m_OnQuit); }

        private static void ON(Callback func, List<Callback> list) { if (!list.Contains(func)) { list.Add(func); } }
        private static void OFF(Callback func, List<Callback> list) {  int i = list.IndexOf(func); if (i != -1) { list.RemoveAt(i); } }

        private void Update() { Call(m_OnUpdate); }
        private void LateUpdate() { Call(m_OnLateUpdate); }
        private void FixedUpdate() { Call(m_OnFixedUpdate); }
        private void OnApplicationQuit()
        {
            quitting = true;
            Call(m_OnQuit);
        }

        private void Call(List<Callback> list) { for (int i = 0, count = list.Count; i < count; i++) { list[i](); } }

    }

}
