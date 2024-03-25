using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using Octokit;
//This is a library that simplifies access to the command line, and allows easily setting up arguments and verbs
using CommandLine;

namespace WallpaperApp;

internal class BackgroundChanger
{
    //Using CommandLine, this code sets up the verb 'run'
    [Verb("run", HelpText = "Re-runs the program and sets a new wallpaper. This will replace the current wallpaper.")]
    public class RunOptions
    {
    }

    //Using CommandLine, this code sets up the verb 'interval'
    [Verb("interval", HelpText = "Sets the interval that the program should run. A value of 0 never re-runs the program")]
    public class InvervalOptions
    {
    }

    public const int SPI_SETDESKWALLPAPER = 20;
    public const int SPIF_UPDATEINFILE = 1;
    public const int SPIF_SENDCHANGE = 2;


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fulWinIni);

    private static void Main(string[] args)
    {
        //Using CommandLine, this code defines what code should run for each argument from the CLI
        var result = Parser.Default.ParseArguments<RunOptions, InvervalOptions>(args)
            .WithParsed<RunOptions>(options => WallpaperRun())
            .WithParsed<InvervalOptions>(options => WallpaperInterval());

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    private static void WallpaperRun()
    {
        Console.WriteLine("Program execution should start now");
        //Checks if the directory exists, and if it doesn't, then the program creates it and informs the user
        if (!Directory.Exists(@"C:\Wallpapers"))
        {
            Directory.CreateDirectory(@"C:\Wallpapers");
            Console.WriteLine("Created directory \"C:\\Wallpapers\"");
        }
        //This code gets some of the different image filetypes in the folder, and merges them into one string[] and the end.
        string[] pngFilePaths = Directory.GetFiles(@"C:\Wallpapers\", "*.png", SearchOption.TopDirectoryOnly);
        string[] jpgFilePaths = Directory.GetFiles(@"C:\Wallpapers\", "*.jpg", SearchOption.TopDirectoryOnly);
        string[] webpFilePaths = Directory.GetFiles(@"C:\Wallpapers\", "*.webp", SearchOption.TopDirectoryOnly);
        string[] imagePaths = pngFilePaths.Concat(jpgFilePaths.Concat(webpFilePaths).ToArray()).ToArray();

        //Randomly chooses an image from the directory and sets it as the wallpaper
        string imageDirectory = imagePaths[new Random().Next(0, imagePaths.Length)];
        SetWallpaper(imageDirectory);
    }

    private static void WallpaperInterval()
    {
        Console.WriteLine("Interval code");
    }


    private static async Task<string> GetInfo()
    {
        //Creates a new 'GithubClient" which a feature of OctoKit, which allows access to the github API
        var gitHubClient = new GitHubClient(new ProductHeaderValue("WallpaperApp"));
        //Asynchronously gets all the contents of a github repo. In this case, I am using a wallpaper repository.
        var repositoryContents = await gitHubClient.Repository.Content.GetAllContents("D3Ext", "aesthetic-wallpapers");
        //Uses the repo that we got earlier to search the given path
        RepositoryContent? firstOrDefault = repositoryContents.FirstOrDefault(c => c.Path == @"/images/3squares.png");
        //Gets the raw download url from the file path, and returns it
        string? downloadUrl = firstOrDefault?.DownloadUrl;
        Console.WriteLine(downloadUrl);
        return downloadUrl;
    }

    private static void SetWallpaper(string imagePath)
    {
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINFILE | SPIF_SENDCHANGE);
    }
}