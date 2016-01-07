using System;
using System.Text.RegularExpressions;
using GitLink;
using GitLink.Providers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CustomBuildTasks
{
    public class GitLinkTask : Task
    {
        [Required]
        public ITaskItem SolutionDirectory { get; set; }

        [Required]
        public string TargetUrl { get; set; }

        [Required]
        public string TargetBranch { get; set; }

        public override bool Execute()
        {
            try
            {
                InnerExecute();
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException vex)
            {
                foreach (var line in Regex.Split(vex.Message, Environment.NewLine))
                    Log.LogError(line);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true, true, null);
            }

            return !Log.HasLoggedErrors;
        }

        private void InnerExecute()
        {
            var context = new Context(new ProviderManager());
            context.SolutionDirectory = SolutionDirectory.FullPath();
            context.TargetUrl = TargetUrl;
            context.TargetBranch = TargetBranch;

            Linker.Link(context);
        }
    }
}