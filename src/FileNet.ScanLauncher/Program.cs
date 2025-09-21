using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace FileNet.ScanLauncher;

internal static class Program
{
    private static readonly string EpsonExe = @"C:\Program Files (x86)\epson\Epson Scan 2\Core\Epson Scan 2.exe";

    private static readonly string WatchFolder = @"E:\Scans\incoming";

    private static readonly string SiteBaseUrl = "https://localhost:7051";

    private static readonly string AgentKey = "c72e356deb784aeea4d39de17b1284c1";

    static async Task<int> Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
                return 0;

            var raw = args[0]; // filenet-scan://scan?employeeId=...&category=...&title=...
            if (!raw.StartsWith("filenet-scan://", StringComparison.OrdinalIgnoreCase))
                return 0;

            var uri = new Uri(raw.Replace("filenet-scan://", "http://placeholder/"));
            var q = HttpUtility.ParseQueryString(uri.Query);

            var employeeId = q["employeeId"] ?? throw new ArgumentException("employeeId is required.");
            var category = q["category"] ?? "0";
            var title = q["title"] ?? $"Scan_{DateTime.Now:yyyyMMdd_HHmmss}";

            Directory.CreateDirectory(WatchFolder);

            var psi = new ProcessStartInfo
            {
                FileName = EpsonExe,
                UseShellExecute = true
            };
            using var proc = Process.Start(psi);

            var created = new List<string>();
            using var watcher = new FileSystemWatcher(WatchFolder)
            {
                IncludeSubdirectories = false,
                EnableRaisingEvents = true,
                Filter = "*.*"
            };
            watcher.Created += (s, e) =>
            {
                var ext = Path.GetExtension(e.FullPath).ToLowerInvariant();
                if (ext is ".png" or ".jpg" or ".jpeg" or ".tif" or ".tiff" or ".pdf")
                    created.Add(e.FullPath);
            };

            proc?.WaitForExit();

            await Task.Delay(800);

            var file = created.Where(File.Exists)
                              .OrderByDescending(File.GetLastWriteTimeUtc)
                              .FirstOrDefault();
            if (file is null) return 0; 

            var uploadUrl = $"{SiteBaseUrl}/Employees/Documents/{employeeId}?handler=AgentUpload&k={Uri.EscapeDataString(AgentKey)}";

            using var http = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            });

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(title, Encoding.UTF8), "title");
            form.Add(new StringContent(category, Encoding.UTF8), "category");

            var bytes = await File.ReadAllBytesAsync(file);
            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(GetMime(Path.GetExtension(file)));
            form.Add(content, "file", Path.GetFileName(file));

            var resp = await http.PostAsync(uploadUrl, form);

            if (resp.Headers.Location is Uri loc)
            {
                Process.Start(new ProcessStartInfo { FileName = loc.ToString(), UseShellExecute = true });
            }
            else
            {
                var fallback = $"{SiteBaseUrl}/Employees/Documents/{employeeId}";
                Process.Start(new ProcessStartInfo { FileName = fallback, UseShellExecute = true });
            }

            return 0;
        }
        catch (Exception ex)
        {
            try { File.WriteAllText(Path.Combine(WatchFolder, "scanlauncher_error.txt"), ex.ToString()); } catch { }
            return 1;
        }
    }

    static string GetMime(string ext) => ext.ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".tif" or ".tiff" => "image/tiff",
        ".pdf" => "application/pdf",
        _ => "application/octet-stream"
    };
}
