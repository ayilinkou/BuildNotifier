using System;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace BuildNotifier
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("Build Notifier", "Get notified whenever your build succeeds or fails.", "1.0")]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(BuildNotifierPackage.PackageGuidString)]
    public sealed class BuildNotifierPackage : AsyncPackage
    {
        public const string PackageGuidString = "B72174EB-16DB-4E76-8705-A15D57F04CA8";

        private DTE2 _dte;
        private BuildEvents _buildEvents;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            _dte = await GetServiceAsync(typeof(DTE)) as DTE2;
            if (_dte == null) return;

            _buildEvents = _dte.Events.BuildEvents;
            _buildEvents.OnBuildDone += OnBuildDone;
        }

        private void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            bool succeeded = _dte.Solution.SolutionBuild.LastBuildInfo == 0;
            string solutionName = System.IO.Path.GetFileNameWithoutExtension(_dte.Solution.FullName);

            ToastNotifier.Show(
                succeeded ? "Build Succeeded!" : "Build Failed!",
                solutionName,
                succeeded
            );
        }
    }
}