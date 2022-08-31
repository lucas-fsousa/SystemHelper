# System Helper

Helper to handle operating system tools.

## Installation

To install, just run the C# compiler to generate the .dll file and once the file has been generated, just add the reference to the project or use [Nuget](https://www.nuget.org/packages/PublicUtility.SystemHelper) or in nuget console, use the following command:


```bash
install-Package PublicUtility.SystemHelper
```

## Usage

```csharp
using PublicUtility.SystemHelper;

var allProccess = SystemOP.GetAllProcessByName("Chrome", false); // search for all processes that have names similar to chrome

var proccess = SystemOP.GetProcessById(12); // get a system process by its identifier

var variable = SystemOP.GetVariableByName("myPc"); // gets the value of an environment variable from the local computer.

var files = SystemOP.GrabFilesFromFolder("C://"); // Gets all files from the specified directory. (does not include folders and subfolders)

SystemOP.HideConsole(); // hide the application console
SystemOP.ShowConsole(); // retrieves the application console view.

SystemOP.HideWindow(your application handle here); // hides an application window through its handle.
SystemOP.ShowWindow(your application handle here); // retrieves an application window through its handle.

SystemOP.Exit(); // close the running program

```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)