using System.Diagnostics;

public class PythonProcessService
{
    private Process process;

    public void StartPython(string exe, string script)
    {
        process = new Process();
        process.StartInfo.FileName = exe;
        process.StartInfo.Arguments = string.IsNullOrEmpty(script) ? "" : $"\"{script}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
    }

    public void StopPython()
    {
        if (process == null || process.HasExited)
            return;

        try
        {
            // Killt den kompletten Prozessbaum (Bootloader + Python-Kindprozess)
            var killer = new Process();
            killer.StartInfo.FileName = "taskkill";
            killer.StartInfo.Arguments = $"/PID {process.Id} /T /F";
            killer.StartInfo.UseShellExecute = false;
            killer.StartInfo.CreateNoWindow = true;
            killer.Start();
            killer.WaitForExit(2000);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogWarning($"Fehler beim Beenden des Python-Prozesses: {e.Message}");
        }
    }
}