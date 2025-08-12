# LogTail

[English](#english) | [中文](#中文)

## English

A cross-platform C# implementation of Linux `tail -f` command with syntax highlighting for real-time log monitoring.

### Features

- **Real-time monitoring**: Similar to `tail -f`, monitors file changes in real-time
- **Historical display**: Shows last N lines on startup (default: 30 lines)
- **Smart file selection**: Automatically selects the newest file in a directory based on timestamp in filename or modification time
- **Enhanced syntax highlighting**: Color-coded log levels, timestamps, numbers, strings, URLs, and more
  - Log levels: `[INF]`/`[ERROR]`/`[WARN]`/`[DEBUG]` (supports abbreviations)
  - Timestamps, numbers, strings, URLs, HTTP codes, etc.
- **Flexible line count**: Use `-n <number>` or `-<number>` to specify how many lines to show
- **Cross-platform**: Runs on Windows, Linux, and macOS
- **Graceful shutdown**: Clean exit with Ctrl+C
- **Flexible input**: Support both file and directory monitoring

### Prerequisites

- .NET 8.0 Runtime (for framework-dependent deployment)
- Or use self-contained version (no .NET required)

### Installation

#### Option 1: Build from Source
```bash
git clone https://github.com/citizenll/LogTail.git
cd LogTail
dotnet build -c Release
```

#### Option 2: Download Releases
Download the latest release from [Releases](https://github.com/citizenll/LogTail/releases) page.

### Usage

```bash
# Monitor default log directory (shows last 30 lines)
LogTail

# Monitor specific file (shows last 30 lines)
LogTail /path/to/logfile.log

# Show last 50 lines, then monitor
LogTail -n 50 /path/to/logfile.log
LogTail -50 /path/to/logfile.log      # Short syntax

# Monitor directory (automatically selects newest file)
LogTail /path/to/logs/

# Show help
LogTail --help

# Windows examples
LogTail.exe "C:\app\logs\application.log"
LogTail.exe -n 100 "C:\app\logs\application.log"
LogTail.exe -20 "C:\app\logs\"
```

### File Selection Logic

When monitoring a directory, the tool uses the following priority to select the newest file:

1. **Timestamp in filename**: Supports formats like:
   - `app-2024-08-12-14-30-45.log`
   - `app_20240812_143045.log`
   - `app-20240812.log`
   - `app20240812.log`

2. **File modification time**: If no timestamp found in filename, uses the most recently modified file

### Build Options

```bash
# Framework-dependent (smaller size, requires .NET runtime)
dotnet publish -c Release

# Self-contained (larger size, includes .NET runtime)
dotnet publish -c Release --self-contained

# Platform-specific builds
dotnet publish -c Release -r win-x64 --self-contained
dotnet publish -c Release -r linux-x64 --self-contained
dotnet publish -c Release -r osx-x64 --self-contained
```

### Command Line Options

```
Usage: LogTail [options] [file|directory]

Options:
  -n <number>    Show the last <number> lines (default: 30)
  -<number>      Same as -n <number> (e.g., -50)
  -h, --help     Show help message

Examples:
  LogTail                          # Monitor default directory, show last 30 lines
  LogTail app.log                  # Monitor specific file, show last 30 lines  
  LogTail -n 50 app.log            # Monitor file, show last 50 lines
  LogTail -200 /var/log/           # Monitor directory, show last 200 lines
```

### Configuration

You can modify the default settings by editing constants in `Program.cs`:

```csharp
private static readonly string DefaultLogPath = @"C:\logs";
var lines = 30; // Default number of lines to show
```

### Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 中文

一个跨平台的 C# 实现的类似 Linux `tail -f` 命令的工具，支持实时日志监控和语法高亮。

### 功能特性

- **实时监控**：类似 `tail -f`，实时监控文件变化
- **历史显示**：启动时显示最后N行（默认：30行）
- **智能文件选择**：根据文件名时间戳或修改时间自动选择目录中最新的文件
- **增强语法高亮**：彩色编码的日志级别、时间戳、数字、字符串、URL等
  - 日志级别：`[INF]`/`[ERROR]`/`[WARN]`/`[DEBUG]`（支持简写）
  - 时间戳、数字、字符串、URL、HTTP状态码等
- **灵活行数控制**：使用 `-n <数字>` 或 `-<数字>` 指定显示行数
- **跨平台**：支持 Windows、Linux 和 macOS
- **优雅退出**：使用 Ctrl+C 干净地退出
- **灵活输入**：支持文件和目录监控

### 系统要求

- .NET 8.0 运行时（框架依赖部署）
- 或使用独立版本（无需安装 .NET）

### 安装

#### 方式一：从源码构建
```bash
git clone https://github.com/citizenll/LogTail.git
cd LogTail
dotnet build -c Release
```

#### 方式二：下载发布版本
从 [Releases](https://github.com/citizenll/LogTail/releases) 页面下载最新版本。

### 使用方法

```bash
# 监控默认日志目录（显示最后30行）
LogTail

# 监控指定文件（显示最后30行）
LogTail /path/to/logfile.log

# 显示最后50行，然后监控
LogTail -n 50 /path/to/logfile.log
LogTail -50 /path/to/logfile.log      # 简写语法

# 监控目录（自动选择最新文件）
LogTail /path/to/logs/

# 显示帮助
LogTail --help

# Windows 示例
LogTail.exe "C:\app\logs\application.log"
LogTail.exe -n 100 "C:\app\logs\application.log"
LogTail.exe -20 "C:\app\logs\"
```

### 文件选择逻辑

监控目录时，工具使用以下优先级选择最新文件：

1. **文件名中的时间戳**：支持以下格式：
   - `app-2024-08-12-14-30-45.log`
   - `app_20240812_143045.log`
   - `app-20240812.log`
   - `app20240812.log`

2. **文件修改时间**：如果文件名中没有找到时间戳，则使用最近修改的文件

### 构建选项

```bash
# 框架依赖（体积小，需要 .NET 运行时）
dotnet publish -c Release

# 独立部署（体积大，包含 .NET 运行时）
dotnet publish -c Release --self-contained

# 平台特定构建
dotnet publish -c Release -r win-x64 --self-contained
dotnet publish -c Release -r linux-x64 --self-contained
dotnet publish -c Release -r osx-x64 --self-contained
```

### 命令行选项

```
用法: LogTail [选项] [文件|目录]

选项:
  -n <数字>      显示最后 <数字> 行（默认：30）
  -<数字>        同 -n <数字>（如：-50）
  -h, --help     显示帮助信息

示例:
  LogTail                          # 监控默认目录，显示最后30行
  LogTail app.log                  # 监控指定文件，显示最后30行
  LogTail -n 50 app.log            # 监控文件，显示最后50行
  LogTail -200 /var/log/           # 监控目录，显示最后200行
```

### 配置

你可以通过编辑 `Program.cs` 中的常量来修改默认设置：

```csharp
private static readonly string DefaultLogPath = @"C:\logs";
var lines = 30; // 默认显示行数
```

### 贡献

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建 Pull Request

### 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。