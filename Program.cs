using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LogTail
{
    class Program
    {
        private static readonly string DefaultLogPath = @"C:\logs";
        private static CancellationTokenSource cancellationTokenSource = new();
        
        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
                Console.WriteLine("\nStopping tail-log...");
            };

            try
            {
                var options = ParseArguments(args);
                var filePath = await ResolveFilePath(options.Path, options.IsDirectory);
                
                Console.WriteLine($"Monitoring: {filePath}");
                Console.WriteLine("Press Ctrl+C to stop...\n");
                
                await TailFile(filePath, cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        private static (string Path, bool IsDirectory) ParseArguments(string[] args)
        {
            if (args.Length == 0)
            {
                return (DefaultLogPath, true);
            }
            
            var path = args[0];
            var isDirectory = Directory.Exists(path);
            
            if (!File.Exists(path) && !isDirectory)
            {
                throw new FileNotFoundException($"Path not found: {path}");
            }
            
            return (path, isDirectory);
        }

        private static Task<string> ResolveFilePath(string path, bool isDirectory)
        {
            if (!isDirectory)
            {
                return Task.FromResult(path);
            }
            
            var files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                throw new FileNotFoundException($"No files found in directory: {path}");
            }
            
            string? latestFile = null;
            DateTime latestTime = DateTime.MinValue;
            
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var timeFromName = ExtractTimeFromFileName(fileInfo.Name);
                var compareTime = timeFromName ?? fileInfo.LastWriteTime;
                
                if (compareTime > latestTime)
                {
                    latestTime = compareTime;
                    latestFile = file;
                }
            }
            
            return Task.FromResult(latestFile ?? throw new FileNotFoundException("Could not determine latest file"));
        }

        private static DateTime? ExtractTimeFromFileName(string fileName)
        {
            var patterns = new[]
            {
                @"(\d{4}[-_]\d{2}[-_]\d{2}[-_]\d{2}[-_]\d{2}[-_]\d{2})",
                @"(\d{4}[-_]\d{2}[-_]\d{2})",
                @"(\d{8}[-_]\d{6})",
                @"(\d{8})"
            };
            
            foreach (var pattern in patterns)
            {
                var match = Regex.Match(fileName, pattern);
                if (match.Success)
                {
                    var timeStr = match.Groups[1].Value.Replace("_", "").Replace("-", "");
                    if (DateTime.TryParseExact(timeStr, new[] { "yyyyMMddHHmmss", "yyyyMMdd" }, null, 
                        System.Globalization.DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }
            }
            
            return null;
        }

        private static async Task TailFile(string filePath, CancellationToken cancellationToken)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.Seek(0, SeekOrigin.End);
            
            using var reader = new StreamReader(fileStream);
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (line != null)
                {
                    PrintHighlightedLine(line);
                }
                else
                {
                    await Task.Delay(100, cancellationToken);
                }
            }
        }

        private static void PrintHighlightedLine(string line)
        {
            Console.ResetColor();
            
            if (line.Contains("ERROR", StringComparison.OrdinalIgnoreCase) || 
                line.Contains("FATAL", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (line.Contains("WARN", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (line.Contains("INFO", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (line.Contains("DEBUG", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {line}");
            Console.ResetColor();
        }
    }
}