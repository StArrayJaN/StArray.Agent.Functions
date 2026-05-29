using StArray.Agent.Functions.Annotations;
using StArray.Agent.Functions.Core;

namespace StArray.Agent.Functions.Tools;

/// <summary>
/// 文件工具集：提供文件存在检查、读写操作。
/// File tools: provides file existence check, read, and write operations.
/// </summary>
public partial class FileTools : ITools
{
    [AgentTool("Tool_File_Exists")]
    public ToolResult<bool> Exists(
        [ToolParameter("Tool_File_Exists_path")] string path)
    {
        return ToolResult<bool>.Success(File.Exists(path));
    }

    [AgentTool("Tool_File_ReadAllText")]
    public ToolResult<string> ReadAllText(
        [ToolParameter("Tool_File_ReadAllText_path")] string path)
    {
        try
        {
            return ToolResult<string>.Success(File.ReadAllText(path));
        }
        catch (Exception ex)
        {
            return ToolResult<string>.Failure($"读取文件失败：{ex.Message}");
        }
    }

    [AgentTool("Tool_File_WriteAllText")]
    public ToolResult<string> WriteAllText(
        [ToolParameter("Tool_File_WriteAllText_path")] string path,
        [ToolParameter("Tool_File_WriteAllText_content")] string content)
    {
        try
        {
            File.WriteAllText(path, content);
            return ToolResult<string>.Success("文件写入成功。");
        }
        catch (Exception ex)
        {
            return ToolResult<string>.Failure($"写入文件失败：{ex.Message}");
        }
    }

    [AgentTool("Tool_FileTools_ReadFileContent")]
    public ToolResult<string> ReadFileContent([ToolParameter("Tool_FileTools_ReadFileContent_path")] string path,
        [ToolParameter("Tool_FileTools_ReadFileContent_startLine")] int startLine,
        [ToolParameter("Tool_FileTools_ReadFileContent_rangeLines")] int rangeLines)
    {
        try
        {
            StreamReader reader = new(path);
            List<string> lines = new();
            int currentLine = 0;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine() ?? string.Empty;
                if (currentLine >= startLine && currentLine < startLine + rangeLines)
                {
                    lines.Add(line);
                }
                currentLine++;
            }
            return ToolResult<string>.Success(string.Join("\n", lines));
        } catch (Exception e) {
            return ToolResult<string>.Failure($"读取文件内容失败：{e.Message}");
        }
    }
    
    [AgentTool("Tool_File_ReplaceStringInFile")]
    public ToolResult<string> ReplaceStringInFile([ToolParameter("Tool_File_ReplaceStringInFile_path")] string path,
        [ToolParameter("Tool_File_ReplaceStringInFile_oldValue")] string oldValue,
        [ToolParameter("Tool_File_ReplaceStringInFile_newValue")] string newValue,
        [ToolParameter("Tool_File_ReplaceStringInFile_isRegex")] bool isRegex = false)
    {
        try
        {
            string content = File.ReadAllText(path);
            if (isRegex) {
                content = System.Text.RegularExpressions.Regex.Replace(content, oldValue, newValue);
            } else {
                content = content.Replace(oldValue, newValue);
            }
            File.WriteAllText(path, content);
            return ToolResult<string>.Success("替换成功。");
        } catch (Exception e) {
            return ToolResult<string>.Failure($"替换文件内容失败：{e.Message}");
        }
    }
}