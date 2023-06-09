using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace PackForIP
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: PackForIP [Build Folder] [Platform] {Plugin Binary} {OutputFile}");
                return 1;
            }

            Console.WriteLine("Packing plugin for ImperialPlugins...");

            var BuildFolder = args[0];
            var platform = args[1];
            if (BuildFolder == "%" || BuildFolder == ".")
            {
                BuildFolder = Environment.CurrentDirectory;
            }

			if (platform.Equals("rm", StringComparison.InvariantCultureIgnoreCase))
				platform = "rm4";

			if (platform.Equals("rocketmod", StringComparison.InvariantCultureIgnoreCase))
				platform = "rm4";

			if (platform.Equals("om", StringComparison.InvariantCultureIgnoreCase))
                platform = "openmod";

            if (!platform.Equals("rm4", StringComparison.InvariantCultureIgnoreCase) && !platform.Equals("openmod", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"Invalid Platform '{platform}', Allowed Platforms: rm4, openmod");
                Thread.Sleep(2000);
                return 1;
            }

            if (!Directory.Exists(BuildFolder))
            {
                Console.WriteLine("Specified build folder doesn't exist");
                Console.WriteLine(BuildFolder);
                Thread.Sleep(2000);
                return 1;
            }

            string binaryFile;
            if (args.Length < 3)
            {
                var binFolder = Directory.GetParent(BuildFolder);

                if (binFolder == null || !binFolder.Exists || !binFolder.Name.Equals("bin", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Failed to determine plugin binary name!");
                    Thread.Sleep(2000);
                    return 1;
                }

                var projectFolder = binFolder.Parent;

                if (projectFolder == null || !projectFolder.Exists)
                {
                    Console.WriteLine("Failed to determine plugin binary name!");
                    Thread.Sleep(2000);
                    return 1;
                }

                binaryFile = Path.Combine(BuildFolder, projectFolder.Name + ".dll");

                if (!File.Exists(binaryFile))
                {
                    Console.WriteLine("Failed to determine plugin binary name!");
                    Thread.Sleep(2000);
                    return 1;
                }
                Console.WriteLine($"Packing Plugin Binary: {Path.GetFileName(binaryFile)}");
            }
            else
            {
                binaryFile = args[2];
                if (!File.Exists(binaryFile))
                {
                    binaryFile = Path.Combine(BuildFolder, binaryFile);
                }

                if (!File.Exists(binaryFile))
                {
                    Console.WriteLine($"Specified plugin binary '{binaryFile}' doesnt' exist!");
                    Thread.Sleep(2000);
                    return 1;
                }
            }

            Console.WriteLine($"Packing Plugin Binary: {Path.GetFileName(binaryFile)}");
            var projectName = Path.GetFileNameWithoutExtension(binaryFile);


            string platformStr = platform == "openmod" ? "om" : "rm";

            var binVer = FileVersionInfo.GetVersionInfo(binaryFile);
            Console.WriteLine($"Packing project version: {binVer.FileVersion}");

            var outputFile = Path.Combine(BuildFolder, $"{projectName}_{platformStr}_{binVer.FileVersion}.zip");

            if (args.Length >= 4)
            {
                outputFile = args[3];
                if (!Path.IsPathRooted(outputFile))
                {
                    outputFile = Path.Combine(BuildFolder, outputFile);
                }

                var dird = new FileInfo(outputFile);

                if (!dird.Directory.Exists)
                {
                    dird.Directory.Create();
                }
            }

            Console.WriteLine($"Packing to: {Path.GetFileName(outputFile)}");


            outputFile = outputFile.Replace("$version$", binVer.FileVersion);

            var assemblies = Directory.GetFiles(BuildFolder, "*.dll");

            var regices = platform == "opemod" ? ExcludeAssemblies.BuildOpenmodRegices() : ExcludeAssemblies.BuildRocketmodRegices();

            var validAsms = new List<string>();

            foreach (var f in assemblies)
            {
                bool include = true;
                foreach (var pattern in regices)
                {
                    if (pattern.IsMatch(f))
                    {
                        include = false;
                        break;
                    }
                }

                if (Path.GetFileName(f).Equals(Path.GetFileName(binaryFile), StringComparison.InvariantCultureIgnoreCase))
                {
                    include = false;
                }

                if (include)
                {
                    validAsms.Add(f);
                }
            }

            if (File.Exists(outputFile))
            {
                var oldFile = outputFile + ".old";

                if (File.Exists(oldFile))
                {
                    File.Delete(oldFile);
                }

                File.Move(outputFile, oldFile);
                Console.WriteLine("Removed existing output file.");
            }




            using (var outFile = new FileStream(outputFile, FileMode.CreateNew))
            using (var archive = new ZipArchive(outFile, ZipArchiveMode.Create))
            {
                archive.CreateEntry("Libraries/");
                archive.CreateEntry("Plugins/");

                var productFile = archive.CreateEntry("product.json");
                using (var prodFl = productFile.Open())
                {
                    var dat = Product.GetProductString(platform);
                    using (var writer = new StreamWriter(prodFl))
                    {
                        writer.Write(dat);
                        writer.Flush();
                    }
                }

                var i = 0;
                foreach (var asm in validAsms)
                {
                    var asmName = Path.GetFileName(asm);
                    i++;
                    var asmEntry = archive.CreateEntry("Libraries/" + asmName);
                    using (var zipStream = asmEntry.Open())
                    using (var asmStream = new FileStream(asm, FileMode.Open, FileAccess.Read))
                    {
                        Console.WriteLine($"Packing Library {Path.GetFileName(asm)} ({i}/{validAsms.Count})...");
                        asmStream.CopyTo(zipStream);
                        zipStream.Flush();
                    }
                }
                Console.WriteLine($"Packed {validAsms.Count} Libraries.");
                var pluginEntry = archive.CreateEntry("Plugins/" + Path.GetFileName(binaryFile));
                using (var pluginZip = pluginEntry.Open())
                using (var pluginStream = new FileStream(binaryFile, FileMode.Open, FileAccess.Read))
                {
                    Console.WriteLine($"Packing plugin assembly {Path.GetFileName(binaryFile)}...");
                    pluginStream.CopyTo(pluginZip);
                    pluginZip.Flush();
                }
            }

            Console.WriteLine("Finished packing plugin for ImperialPlugins release.");
            Console.WriteLine();
            Console.WriteLine($"Output file for upload: {outputFile}");

            return 0;
        }
    }
}