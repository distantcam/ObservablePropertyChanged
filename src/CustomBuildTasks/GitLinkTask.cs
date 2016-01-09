using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public class GitLinkTask : ToolTask
{
    [Required]
    public ITaskItem SolutionDirectory { get; set; }

    protected override string ToolName => "GitLink.exe";

    protected override string GenerateFullPathToTool() => Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "GitLink.exe");

    protected override string GenerateCommandLineCommands() => SolutionDirectory.FullPath();
}