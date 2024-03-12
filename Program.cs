using System.Runtime.InteropServices;
using Octokit;

namespace WallpaperApp;

internal class BackgroundChanger
{
    public const int SPI_SETDESKWALLPAPER = 20;
    public const int SPIF_UPDATEINFILE = 1;
    public const int SPIF_SENDCHANGE = 2;


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fulWinIni);

    private static void Main(string[] args)
    {
        Task.WaitAny(GetInfo());
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    private static async Task<string> GetInfo()
    {
        var gitHubClient = new GitHubClient(new ProductHeaderValue("WallpaperApp"));
        var repositoryContents = await gitHubClient.Repository.Content.GetAllContents("D3Ext", "aesthetic-wallpapers"); 
        // Console.WriteLine(repositoryContents[0].Path);
        RepositoryContent? firstOrDefault = repositoryContents.FirstOrDefault(c => c.Path == @"path-of-the-file-you-want");
        string? downloadUrl = firstOrDefault?.DownloadUrl;
        return downloadUrl;
    }

    private static void SetWallpaper(string imagePath)
    {
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINFILE | SPIF_SENDCHANGE);
    }
}