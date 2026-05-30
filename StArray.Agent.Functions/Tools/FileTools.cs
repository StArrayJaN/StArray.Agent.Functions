using StArray.Agent.Functions.Annotations;
using StArray.Agent.Functions.Core;

namespace StArray.Agent.Functions.Tools;

/// <summary>
/// 文件工具集：提供文件存在检查、读写操作。
/// File tools: provides file existence check, read, and write operations.
/// </summary>
public partial class FileTools : ITools
{
    private const string RES_FILE_PATH_KEY = "Tool_File_FilePath";
    private const string RES_DIRECTORY_PATH_KEY = "Tool_File_DirectoryPath";
    [AgentTool("Tool_File_Exists")]
    public ToolResult<bool> Exists(
        [ToolParameter(RES_FILE_PATH_KEY)] string path)
    {
        return ToolResult<bool>.Success(File.Exists(path));
    }
    
    [AgentTool("Tool_File_ListFiles")]
    public ToolResult<string[]> ListFiles(
        [ToolParameter(RES_DIRECTORY_PATH_KEY)] string path,
        [ToolParameter("Tool_File_ListFiles_recursive")] bool recursive = false)
    {
        return ToolResult<string[]>.Success(recursive ? Directory.GetFiles(path, "*", SearchOption.AllDirectories) : Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly));
    }

    [AgentTool("Tool_File_ReadAllText")]
    public ToolResult<string> ReadAllText(
        [ToolParameter(RES_FILE_PATH_KEY)] string path)
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
        [ToolParameter(RES_FILE_PATH_KEY)] string path,
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
    public ToolResult<string> ReadFileContent([ToolParameter(RES_FILE_PATH_KEY)] string path,
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
    public ToolResult<string> ReplaceStringInFile([ToolParameter(RES_FILE_PATH_KEY)] string path,
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

    [AgentTool("Tool_File_Delete")]
    public ToolResult<string> Delete([ToolParameter("Tool_File_Delete_path")] string path)
    {
        try
        {
            File.Delete(path);
            return ToolResult<string>.Success("删除成功。");
        }
        catch (Exception e)
        {
            return ToolResult<string>.Failure($"删除文件失败：{e.Message}");
        }
    }
    
    [AgentTool("Tool_File_SearchContentInFiles")]
    public ToolResult<string> SearchContentInFiles([ToolParameter("Tool_File_SearchContentInFiles_paths")] string[] paths,
        [ToolParameter("Tool_File_SearchContentInFiles_content")] string content,
        [ToolParameter("Tool_File_SearchContentInFiles_isRegex")] bool isRegex = false)
    { 
        try
        {
            List<string> result = new();
            foreach (string path in paths)
            {
                if (!File.Exists(path))
                {
                    continue;
                }
                string fileContent = File.ReadAllText(path);
                if (isRegex)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(fileContent, content))
                    {
                        result.Add($"文件：{path}");
                    }
                }
                else
                {
                    if (fileContent.Contains(content))
                    {
                        result.Add($"文件：{path}");
                    }
                }
            }
            return ToolResult<string>.Success(string.Join("\n", result));
        } catch (Exception e)
        {
            return ToolResult<string>.Failure($"搜索文件内容失败：{e.Message}");
        }
    }
    
    [AgentTool("Tool_File_GetCurrentDirectory")]
    public ToolResult<string> GetCurrentDirectory()
    {
        return ToolResult<string>.Success(Directory.GetCurrentDirectory());
    }
    
    [AgentTool("Tool_File_ChangeWorkingDirectory")]
    public ToolResult<string> ChangeWorkingDirectory(
        [ToolParameter(RES_DIRECTORY_PATH_KEY)] string path)
    {
        try
        {
            Directory.SetCurrentDirectory(path);
            return ToolResult<string>.Success("更改工作目录成功。");
        } catch (Exception e)
        {
            return ToolResult<string>.Failure($"更改工作目录失败：{e.Message}");
        }
    }
}