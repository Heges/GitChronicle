using System.Diagnostics;
using System.Text;
using Utils.Core.Interfaces;

namespace Utils.Services
{
    public class Shell : IShell
    {
        public async Task<int> Run(string workingDir, IReadOnlyList<string> args)
        {
            var (rc, _, _) = await RunInfo(workingDir, args);
            return rc;
        }

        public async Task<(int rc, string stdout, string stderr)> RunInfo(string workingDir, IReadOnlyList<string> args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = false,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };

            foreach (var a in args) psi.ArgumentList.Add(a);

            using var p = new Process { StartInfo = psi, EnableRaisingEvents = false };

            p.Start();

            Task<string> outTask = p.StandardOutput.ReadToEndAsync();
            Task<string> errTask = p.StandardError.ReadToEndAsync();
            Task waitTask = p.WaitForExitAsync();

            await Task.WhenAll(outTask, errTask, waitTask);

            return (p.ExitCode, outTask.Result, errTask.Result);
        }
    }
}
