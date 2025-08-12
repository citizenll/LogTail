# LogTail

[English](#english) | [中文](#中文)

## English

A cross-platform C# implementation of Linux `tail -f` command with syntax highlighting for real-time log monitoring.

### Features

- **Real-time monitoring**: Similar to `tail -f`, monitors file changes in real-time
- **Smart file selection**: Automatically selects the newest file in a directory based on timestamp in filename or modification time
- **Syntax highlighting**: Color-coded log levels for better readability
  - `ERROR/FATAL`: Red
  - `WARN`: Yellow
  - `INFO`: Green
  - `DEBUG`: Cyan
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
# Monitor default log directory
LogTail

# Monitor specific file
LogTail /path/to/logfile.log

# Monitor directory (automatically selects newest file)
LogTail /path/to/logs/

# Windows examples
LogTail.exe "C:\app\logs\application.log"
LogTail.exe "C:\app\logs\"
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

### Configuration

You can modify the default log path by editing the `DefaultLogPath` constant in `Program.cs`:

```csharp
private static readonly string DefaultLogPath = @"C:\logs";
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
- **智能文件选择**：根据文件名时间戳或修改时间自动选择目录中最新的文件
- **语法高亮**：彩色编码的日志级别，提高可读性
  - `ERROR/FATAL`：红色
  - `WARN`：黄色
  - `INFO`：绿色
  - `DEBUG`：青色
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
# 监控默认日志目录
LogTail

# 监控指定文件
LogTail /path/to/logfile.log

# 监控目录（自动选择最新文件）
LogTail /path/to/logs/

# Windows 示例
LogTail.exe "C:\app\logs\application.log"
LogTail.exe "C:\app\logs\"
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

### 配置

你可以通过编辑 `Program.cs` 中的 `DefaultLogPath` 常量来修改默认日志路径：

```csharp
private static readonly string DefaultLogPath = @"C:\logs";
```

### 贡献

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建 Pull Request

### 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。