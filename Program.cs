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
        //Creates a new 'GithubClient" which a feature of OctoKit, which allows access to the github API
        var gitHubClient = new GitHubClient(new ProductHeaderValue("WallpaperApp"));
        //Asynchronously gets all the contents of a github repo. In this case, I am using a wallpaper repository.
        var repositoryContents = await gitHubClient.Repository.Content.GetAllContents("D3Ext", "aesthetic-wallpapers");
        Console.WriteLine(repositoryContents);
        //Uses the repo that we got earlier to search the given path
        RepositoryContent? firstOrDefault = repositoryContents.FirstOrDefault(c => c.Path == @"images/3squares.png");
        Console.WriteLine(firstOrDefault);
        //Gets the raw download url from the file path, and returns it
        string? downloadUrl = firstOrDefault?.DownloadUrl;
        return downloadUrl;
    }

    private static void SetWallpaper(string imagePath)
    {
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINFILE | SPIF_SENDCHANGE);
    }
}