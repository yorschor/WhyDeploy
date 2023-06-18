using System.Diagnostics;

namespace WDUtility;

public static class ProcessUtility
{
	public static Task<int> RunProcessAsync(string fileName, string arguments)
	{
		var tcs = new TaskCompletionSource<int>();

		var process = new Process
		{
			StartInfo =
			{
				FileName = fileName,
				Arguments = arguments,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			},
			EnableRaisingEvents = true
		};

		process.Exited += (sender, args) =>
		{
			tcs.SetResult(process.ExitCode);
			process.Dispose();
		};

		process.Start();

		return tcs.Task;
	}
}