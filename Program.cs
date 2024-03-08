using System.Runtime.InteropServices;
// ReSharper disable IdentifierTypo
// ReSharper disable MemberCanBePrivate.Global

namespace WallpaperApp;

internal class BackgroundChanger
{
    public const int SPI_SETDESKWALLPAPER = 20;
    public const int SPIF_UPDATEINFILE = 1;
    public const int SPIF_SENDCHANGE = 2;
    
    // ReSharper disable once InconsistentNaming
    private static readonly HttpClient httpClient = new HttpClient();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fulWinIni);

    private static void Main(string[] args)
    {
        const string imagePath = @"C:\Wallpapers\1.jpg";
        SetWallpaper(imagePath);
        Task downloadImage = DownloadImage("https://raw.githubusercontent.com/D3Ext/aesthetic-wallpapers/main/images/3squares.png",
            @"C:\Wallpapers\downloadedImage");
        Task.WaitAny(downloadImage);

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    private static async Task DownloadImage(string downloadUrl, string filePath)
    {
        //Asynchronously downloads the file
        HttpResponseMessage response = await httpClient.GetAsync(downloadUrl);

        response.EnsureSuccessStatusCode();

        byte[] content = await response.Content.ReadAsByteArrayAsync();
        await File.WriteAllBytesAsync(filePath, content);
    }

    private static void SetWallpaper(string imagePath)
    {
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINFILE | SPIF_SENDCHANGE);
    }
}