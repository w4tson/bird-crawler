using PuppeteerSharp;

namespace ConsoleApp1;

public static class ElementExtensions
{
    public static async Task<string> GetInnerTextAsync(this IElementHandle elementHandle)
    {
        return await elementHandle.EvaluateFunctionAsync<string>("e => e.innerText");
    }
    
    public static async Task<string> GetHref(this IElementHandle elementHandle)
    {
        return await elementHandle.EvaluateFunctionAsync<string>("e => e.href");
    }
    
    public static String ReplaceFirst(this String text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }
    
    public static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
    {
        DirectoryInfo? previousDir  = null;
        var directory = new DirectoryInfo(
            currentPath ?? Directory.GetCurrentDirectory());
        
        do
        {
            previousDir = directory;
            directory = directory.Parent;
        } while (directory != null && !directory.GetFiles("*.sln").Any());
        
        return previousDir;
    }
}