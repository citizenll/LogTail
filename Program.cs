using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                Console.WriteLine("\nStopping LogTail...");
            };

            try
            {
                var options = ParseArguments(args);
                var filePath = await ResolveFilePath(options.Path, options.IsDirectory);
                
                Console.WriteLine($"Monitoring: {filePath} (showing last {options.Lines} lines)");
                Console.WriteLine("Press Ctrl+C to stop...\n");
                
                await TailFile(filePath, options.Lines, cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        private static (string Path, bool IsDirectory, int Lines) ParseArguments(string[] args)
        {
            var path = DefaultLogPath;
            var lines = 30; // 默认显示100行
            
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                
                if (arg == "-n" && i + 1 < args.Length)
                {
                    if (int.TryParse(args[i + 1], out var lineCount) && lineCount > 0)
                    {
                        lines = lineCount;
                        i++; // 跳过下一个参数
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid line count: {args[i + 1]}");
                    }
                }
                else if (arg.StartsWith("-") && arg.Length > 1 && int.TryParse(arg.Substring(1), out var shortLines) && shortLines > 0)
                {
                    lines = shortLines; // 支持 -100 这样的简写
                }
                else if (arg == "--help" || arg == "-h")
                {
                    ShowHelp();
                    Environment.Exit(0);
                }
                else if (!arg.StartsWith("-"))
                {
                    path = arg; // 文件或目录路径
                }
            }
            
            var isDirectory = Directory.Exists(path);
            
            if (!File.Exists(path) && !isDirectory)
            {
                throw new FileNotFoundException($"Path not found: {path}");
            }
            
            return (path, isDirectory, lines);
        }
        
        private static void ShowHelp()
        {
            Console.WriteLine("LogTail - Real-time log monitoring tool with syntax highlighting");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  LogTail [options] [file|directory]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -n <number>    Show the last <number> lines (default: 100)");
            Console.WriteLine("  -<number>      Same as -n <number> (e.g., -50)");
            Console.WriteLine("  -h, --help     Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  LogTail                          # Monitor default log directory, show last 100 lines");
            Console.WriteLine("  LogTail app.log                  # Monitor specific file, show last 100 lines");
            Console.WriteLine("  LogTail -n 50 app.log            # Monitor file, show last 50 lines");
            Console.WriteLine("  LogTail -200 /var/log/           # Monitor directory, show last 200 lines");
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

        private static async Task TailFile(string filePath, int initialLines, CancellationToken cancellationToken)
        {
            // 首先显示最后N行
            await ShowLastLines(filePath, initialLines);
            
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
        
        private static async Task ShowLastLines(string filePath, int lineCount)
        {
            var lines = new List<string>();
            
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fileStream);
            
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lines.Add(line);
                if (lines.Count > lineCount)
                {
                    lines.RemoveAt(0); // 保持只有最后 N 行
                }
            }
            
            foreach (var lastLine in lines)
            {
                PrintHighlightedLine(lastLine);
            }
        }

        private static void PrintHighlightedLine(string line)
        {
            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
            PrintColorizedLine(line);
            Console.WriteLine();
        }

        private static void PrintColorizedLine(string line)
        {
            var patterns = new List<(string Pattern, ConsoleColor Color)>
            {
                // 时间戳模式
                (@"\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}(?:\.\d{3})?", ConsoleColor.Cyan),
                (@"\+\d{2}:\d{2}", ConsoleColor.Cyan),
                
                // 日志级别 (支持简写)
                (@"\[(?:ERROR|ERR|FATAL|FTL)\]", ConsoleColor.Red),
                (@"\[(?:WARN|WRN)\]", ConsoleColor.Yellow),
                (@"\[(?:INFO|INF)\]", ConsoleColor.Green),
                (@"\[(?:DEBUG|DBG|TRACE|TRC)\]", ConsoleColor.DarkCyan),
                
                // 数字 (价格、时间、ID等)
                (@"\b\d+(?:\.\d+)?\b", ConsoleColor.Magenta),
                
                // 字符串 (引号包围)
                ("\"[^\"]*\"", ConsoleColor.DarkYellow),
                ("'[^']*'", ConsoleColor.DarkYellow),
                
                // URL和路径
                (@"https?://[^\s]+", ConsoleColor.Blue),
                (@"/[^\s]*", ConsoleColor.Blue),
                
                // 模型名称
                (@"claude-[a-z0-9-]+", ConsoleColor.DarkBlue),
                
                // 货币符号和金额
                (@"\$\d+(?:\.\d+)?", ConsoleColor.DarkGreen),
                
                // HTTP状态码
                (@"\b[1-5]\d{2}\b", ConsoleColor.DarkMagenta),
                
                // 时间单位 (ms等)
                (@"\d+(?:\.\d+)?(?:ms|s|m|h)\b", ConsoleColor.DarkRed)
            };

            var parts = new List<(int Start, int Length, ConsoleColor? Color)>();
            
            foreach (var (pattern, color) in patterns)
            {
                var matches = Regex.Matches(line, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    parts.Add((match.Index, match.Length, color));
                }
            }
            
            // 按位置排序，避免重叠
            parts = parts.OrderBy(p => p.Start).ToList();
            
            var currentIndex = 0;
            foreach (var (start, length, color) in parts)
            {
                // 如果有重叠，跳过
                if (start < currentIndex) continue;
                
                // 打印前面的普通文本
                if (start > currentIndex)
                {
                    Console.ResetColor();
                    Console.Write(line.Substring(currentIndex, start - currentIndex));
                }
                
                // 打印高亮文本
                if (color.HasValue)
                {
                    Console.ForegroundColor = color.Value;
                    Console.Write(line.Substring(start, length));
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(line.Substring(start, length));
                }
                
                currentIndex = start + length;
            }
            
            // 打印剩余的普通文本
            if (currentIndex < line.Length)
            {
                Console.ResetColor();
                Console.Write(line.Substring(currentIndex));
            }
        }
    }
}