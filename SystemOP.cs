using PublicUtility.Nms.Enums;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PublicUtility.SystemHelper {

  public static class SystemOP {

    #region PRIVATE METHODS

    private static void BaseLocateFileOnSystem(string fileName, string rootDir, bool firstOnly, ref IList<string> lstFilePath, bool exactFileName, out bool found) {
      /* This method is used recursively to read all files from the root path. 
       * Folders are traversed one by one until all files with the specified name are found 
       * or the first to be found which depends on the "firstOnly" parameter.
       * Folders that require administrator access will be blocked and cause an access denied error that is handled directly within the folder reading loop.
       * The list of paths is passed by reference so it is possible to keep a single list in memory and persist the data through recursive calls.
       */

      List<string> lstdir = Directory.GetDirectories(rootDir).ToList();
      found = false;
      foreach(string dir in lstdir) {
        try {
          // checks if the repository has other repositories. If yes, make a recursive call.
          if(Directory.GetDirectories(dir).ToList().Count > 1)
            BaseLocateFileOnSystem(fileName, dir, firstOnly, ref lstFilePath, exactFileName, out found);

          if(firstOnly && found)
            return;

        } catch(Exception ex) {

          // checks if the error is access denied
          if(ex.Message.ToLower().Contains("is denied"))
            continue;
          else
            throw new Exception(ex.Message);

        }

        var lstFiles = GetFilesFromFolder(dir);
        foreach(var file in lstFiles) {

          if(exactFileName) {
            if(file.Name == fileName) {
              lstFilePath.Add(file.FullName);
              found = true;
              break;
            }

          } else {
            if(file.Name.ToLower().Contains(fileName.ToLower())) {
              lstFilePath.Add(file.FullName);
              found = true;
              break;
            }
          }

        }

        if(firstOnly && found)
          return;
      }

    }

    #endregion

    #region INTEROPT DLLS

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    #endregion

    public static void HideConsole() => ShowWindow(GetConsoleWindow(), (int)WidowMode.Hide);

    public static void ShowConsole() => ShowWindow(GetConsoleWindow(), (int)WidowMode.Show);

    public static void ShowWindow(IntPtr handle) => ShowWindow(handle, (int)WidowMode.Show);

    public static void HideWindow(IntPtr handle) => ShowWindow(handle, (int)WidowMode.Hide);

    public static Process GetProcessById(int id) => Process.GetProcessById(id);

    public static IEnumerable<Process> GetAllProcessByName(string name, bool exactName = true) {
      if(exactName)
        return Process.GetProcesses().Where(x => x.ProcessName.Contains(name));

      return Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(name[1..].ToLower()));
    }

    public static string GetVariableByName(string name) => Environment.GetEnvironmentVariable(name);

    public static IEnumerable<FileInfo> GetFilesFromFolder(string dirPath) => new DirectoryInfo(dirPath).GetFiles().OrderBy(x => x.CreationTime);

    public static void Exit(int exitCode = 0) => Environment.Exit(exitCode);

    public static bool IsOS64Bits() => Environment.Is64BitOperatingSystem;

    public static void RunPromptScript(IEnumerable<string> commands, string privatePath = @"", bool shellExecute = false) {
      var psi = new ProcessStartInfo();

      privatePath = Path.Combine(privatePath, "NewCommandsForExec.bat");

      if(!commands.Any())
        throw new Exception($"{nameof(commands)} less than zero");

      File.WriteAllLines(privatePath, commands);

      psi.FileName = privatePath;
      psi.UseShellExecute = shellExecute;
      psi.CreateNoWindow = true;
      psi.WindowStyle = ProcessWindowStyle.Hidden;

      using Process process = Process.Start(psi);
      process.WaitForExit();

      // Delete the file after finishing the compilation
      if(File.Exists(privatePath))
        File.Delete(privatePath);
    }

    public async static ValueTask<IEnumerable<string>> LocateFileOnSystemAsync(string fileName, bool exactFilename, bool firstOnly = false, string rootDir = "C://") {
      return await Task.Run(() => {
        IList<string> result = new List<string>();

        BaseLocateFileOnSystem(fileName, rootDir, firstOnly, ref result, exactFilename, out _);

        return result;
      });
    }

    public static IEnumerable<string> LocateFileOnSystem(string fileName, bool exactFilename, bool firstOnly = false, string rootDir = "C://") {
      IList<string> result = new List<string>();

      BaseLocateFileOnSystem(fileName, rootDir, firstOnly, ref result, exactFilename, out _);

      return result;
    }
  }
}