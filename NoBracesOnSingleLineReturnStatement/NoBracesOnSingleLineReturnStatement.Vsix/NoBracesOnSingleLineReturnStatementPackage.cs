using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NoBracesOnSingleLineReturnStatement.Vsix
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("95eb0ac7-0e3c-463f-830e-d00e2b8e78bc")]
    public sealed class NoBracesOnSingleLineReturnStatementPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
            => await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
    }
}
