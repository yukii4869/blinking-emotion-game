using System.Diagnostics;

public class PythonProcessService
{
    private Process process;

    public void StartPython(string exe, string script)
    {
        process = new Process();
        process.StartInfo.FileName = exe;
        process.StartInfo.Arguments = $"\"{script}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
    }

    public void StopPython()
    {
        if (process != null && !process.HasExited)
            process.Kill();
    }
}