using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NuGet;

namespace BuildNuGetPackage
{
    class Program
    {
        class PropertyProvider : Dictionary<string, dynamic>, IPropertyProvider
        {
            dynamic IPropertyProvider.GetPropertyValue(string propertyName)
            {
                dynamic value;
                if (TryGetValue(propertyName, out value))
                    return value;

                return null;
            }
        }

        static void Main(string[] args)
        {
            var path = "";

            if (args.Length == 1)
            {
                path = Path.GetFullPath(args[0]);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            Environment.CurrentDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            var propertyProvider = new PropertyProvider()
            {
                { "version", GitVersionInformation.NuGetVersionV2 }
            };

            var packageBuilder = new PackageBuilder();

            using (var spec = File.OpenRead(Path.Combine("Assets", "ObservablePropertyChanged.nuspec")))
            {
                var manifest = Manifest.ReadFrom(spec, propertyProvider, false);
                packageBuilder.Populate(manifest.Metadata);
            }

            packageBuilder.PopulateFiles("", new[] {
                new ManifestFile { Source = "ObservablePropertyChanged.dll", Target = "lib" },
                new ManifestFile { Source = "ObservablePropertyChanged.pdb", Target = "lib" }
            });

            var packagesConfig = @"..\..\..\ObservablePropertyChanged\packages.config";

            if (File.Exists(packagesConfig))
            {
                var dependencies = new List<PackageDependency>();

                var doc = XDocument.Load(packagesConfig);

                var packages = doc.Descendants()
                    .Where(x => x.Name == "package" && x.Attribute("developmentDependency")?.Value != "true")
                    .Select(p => new { id = p.Attribute("id").Value, version = SemanticVersion.Parse(p.Attribute("version").Value) })
                    .Select(p => new PackageDependency(p.id, new VersionSpec() { IsMinInclusive = true, MinVersion = p.version }));

                dependencies.AddRange(packages);

                packageBuilder.DependencySets.Add(new PackageDependencySet(null, dependencies));
            }

            var packagePath = Path.Combine(path, packageBuilder.GetFullName() + ".nupkg");

            using (var file = new FileStream(packagePath, FileMode.Create))
            {
                Console.WriteLine($"Saving file {packagePath}");

                packageBuilder.Save(file);
            }
        }
    }
}