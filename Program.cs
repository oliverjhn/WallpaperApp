using System.Runtime.InteropServices;
//This is a package that simplifies access to the command line, and allows easily setting up arguments and verbs
using CommandLine;

//Disables these warnings because in the cases where they appear, I know that it is impossible for them to be meaningful errors
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace WallpaperApp;

internal class BackgroundChanger
{
    //Using CommandLine, this code sets up the verb 'run' and makes it the default
    [Verb("run", isDefault: true, HelpText = "Re-runs the program and sets a new wallpaper. This will replace the current wallpaper.")]
    public class RunOptions
    {
    }

    //Variables for use in the method SystemParametersInfo. These are defined here for increased code readability.
    public const int SPI_SETDESKWALLPAPER = 20;
    public const int SPIF_UPDATEINFILE = 1;
    public const int SPIF_SENDCHANGE = 2;


    //Imports the DLL required for SystemParametersInfo
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //This is kind of complicated, so for further reading this is the documentation link:
    //https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-systemparametersinfoa
    //It is a method that takes:
    //  uAction, this is the type of user action that will occur. In this case it will be a desktop change
    //  uParam, this is used for any additional parameters or flags.
    //  lpvParam, this takes a string input, usually a file path. In this case it is the location of the wallpaper
    //  fulWinIni, this controls how how the user profile is updated and how other applications are notified.
    //      It often combines multiple flags, in this case SPIF_UPDATEINFILE and SPIF_SENDCHANGE
    public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fulWinIni);

    //Entry point for the program, sets up the command line parser
    private static void Main(string[] args)
    {
        //Using CommandLine, this code defines what code should run for each argument from the CLI
        var result = Parser.Default.ParseArguments<RunOptions>(args)
            .WithParsed<RunOptions>(options => WallpaperRun());
    }

    //This function is called when the user passes the verb 'run' as an argument. It is where the main logic of the program occurs,
    //It chooses what image should be displayed and calls another function to set the background
    private static void WallpaperRun()
    {
        //This code loops through every directory until it finds one called 'wallpapers', then sets that as the wallpaper path
        DirectoryInfo wallpaperDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (wallpaperDirectory.Name != "wallpapers")
        {
            wallpaperDirectory = wallpaperDirectory.Parent;
            foreach (var currentDirectory in wallpaperDirectory.GetDirectories())
            {
                if (currentDirectory.Name == "wallpapers")
                {
                    wallpaperDirectory = currentDirectory;
                }
            }
        }

        //This code gets some of the different image filetypes in the folder, and merges them into one string[] and the end.
        string[] pngFilePaths = Directory.GetFiles(wallpaperDirectory.FullName, "*.png", SearchOption.TopDirectoryOnly);
        string[] jpgFilePaths = Directory.GetFiles(wallpaperDirectory.FullName, "*.jpg", SearchOption.TopDirectoryOnly);
        string[] webpFilePaths = Directory.GetFiles(wallpaperDirectory.FullName, "*.webp", SearchOption.TopDirectoryOnly);
        string[] imagePaths = pngFilePaths.Concat(jpgFilePaths.Concat(webpFilePaths).ToArray()).ToArray();

        //Randomly chooses an image from the directory and sets it as the wallpaper
        string imageDirectory = imagePaths[new Random().Next(0, imagePaths.Length)];
        SetWallpaper(imageDirectory);
        Console.WriteLine($"Set desktop wallpaper as {imageDirectory}");
    }


    //Sets the wallpaper
    private static void SetWallpaper(string imagePath)
    {
        //Uses the function from earlier to set the desktop background.
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINFILE | SPIF_SENDCHANGE);
    }
}


// Code that I simply couldn't get to work in time, I really spent way too long on this and should've spent this time on other features
//      like a GUI or making it a windows service app

// private static async Task<string> GetInfo()
// {
//     //Creates a new 'GithubClient" which a feature of OctoKit, which allows access to the github API
//     var gitHubClient = new GitHubClient(new ProductHeaderValue("WallpaperApp"));
//     //Asynchronously gets all the contents of a github repo. In this case, I am using a wallpaper repository.
//     var repositoryContents = await gitHubClient.Repository.Content.GetAllContents("D3Ext", "aesthetic-wallpapers");
//     //Uses the repo that we got earlier to search the given path
//     RepositoryContent? firstOrDefault = repositoryContents.FirstOrDefault(c => c.Path == @"/images/3squares.png");
//     //Gets the raw download url from the file path, and returns it
//     string? downloadUrl = firstOrDefault?.DownloadUrl;
//     Console.WriteLine(downloadUrl);
//     return downloadUrl;
// }