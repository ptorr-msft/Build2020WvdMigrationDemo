using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.Management.Deployment;

namespace StageAppxPackage
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine($@"
Stage a package in-place for MSIX AppAttach. Usage: {Process.GetCurrentProcess().ProcessName} <path-to-package>.
Use the path inside the VHD file, not the junction path, and do not include the 'AppxManifest.xml' filename.
Once the package is staged, it can be registered with Add-AppxPackage in PowerShell.");
                return 1;
            }

            if (!Uri.TryCreate(args[0], UriKind.Absolute, out var uri) || uri.Scheme != "file")
            {
                Console.Error.WriteLine($"'{args[0]}' is not a valid name for a package; must be convertible to the 'file' URI scheme.");
                return 2;
            }

            var result = RealMain(uri).Result;
            if (result == 0)
            {
                Console.WriteLine("Success!");
            }
            else
            {
                Console.WriteLine("Error.");
            }

            return result;
        }

        // Not in the SDK yet
        const DeploymentOptions StageInPlace = (DeploymentOptions)0x400000;

        static async Task<int> RealMain(Uri packageUri)
        {
            try
            {
                var pm = new PackageManager();
                await pm.StagePackageAsync(packageUri, null, StageInPlace);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 3;
            }

            return 0;
        }
    }
}
