using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MicrophoneInput))]
public class MicrophoneInputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MicrophoneInput mic = (MicrophoneInput)target;

        string[] devices = Microphone.devices;

        if (devices.Length == 0)
        {
            EditorGUILayout.HelpBox("Kein Mikrofon gefunden!", MessageType.Warning);
            return;
        }

        int currentIndex = Mathf.Max(0, System.Array.IndexOf(devices, mic.DeviceName));

        int newIndex = EditorGUILayout.Popup("Mikrofon", currentIndex, devices);

        if (newIndex != currentIndex)
        {
            Undo.RecordObject(mic, "Change Microphone");
            typeof(MicrophoneInput)
                .GetField("deviceName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(mic, devices[newIndex]);
        }

        DrawDefaultInspector();
    }
}
