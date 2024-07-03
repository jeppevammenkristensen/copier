using System.Diagnostics;
using AutoSpectre;
using Nuke.Common.IO;

namespace Copier;

[AutoSpectreForm(Culture = "da-DK")]
public class Prompt
{
    private readonly CombosSettings _settings;

    public Prompt(CombosSettings settings)
    {
        _settings = settings;
    }
    

    [SelectPrompt(Title = "Select combo")] 
    public FileCopy FileCopyCombo { get; internal set; } = default!;

    public IEnumerable<FileCopy> FileCopyComboSource => _settings.FileCopy;
    
    [TextPrompt]
    public bool SelectSource { get; set; }
    
    [SelectPrompt(Condition = nameof(SelectSource),SearchEnabled = true)]
    public AbsolutePath? ChildSource { get; set; }

    public IEnumerable<AbsolutePath> ChildSourceSource()
    {
        return AbsolutePath.Create(FileCopyCombo.SourceDirectory).GetDirectories();
    }
    
    [TextPrompt]
    public bool SelectDestination { get; set; }
    
    [SelectPrompt(Condition = nameof(SelectDestination), SearchEnabled = true)]
    public AbsolutePath? ChildDestination { get; set; }

    [TextPrompt]
    public bool CreateNewSubfolder { get; set; }
    
    [TextPrompt(Title = "Destination folder")] 
    public string? NewDestinationFolder { get; set; }
    public IEnumerable<AbsolutePath> ChildDestinationSource()
    {
        return AbsolutePath.Create(FileCopyCombo.DestinationDirectory).GetDirectories();
    }

    public AbsolutePath Source() => ChildSource ?? AbsolutePath.Create(FileCopyCombo.SourceDirectory);

    public AbsolutePath Destination()
    {
        if (NewDestinationFolder is { })
        {
            return AbsolutePath.Create(FileCopyCombo.DestinationDirectory) / NewDestinationFolder;
        }
        return ChildDestination ?? AbsolutePath.Create(FileCopyCombo.DestinationDirectory);  
    } 

    // public string FileCopyComboConverter(FileCopy copy)
    // {
    //     return 
    // }

}

public class ProcessWrapper
{
    private readonly Action<string?> _output;
    private readonly Action<string?> _errorOutput;

    public ProcessWrapper(Action<string?> output, Action<string?> errorOutput)
    {
        _output = output;
        _errorOutput = errorOutput;
    }

    public async Task Run(string command, string arguments)
    {
        Process process = new();
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.OutputDataReceived += ProcessOnOutputDataReceived;
        process.ErrorDataReceived += ProcessOnErrorDataReceived;
        process.Start();
        
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        await process.WaitForExitAsync();
        
        
    }

    private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        _errorOutput(e.Data);
    }

    private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
         _output(e.Data);
    }
}

public class CombosSettings
{
    public List<FileCopy> FileCopy { get; set; }
}

public class FileCopy
{
    public string SourceDirectory { get; set; }
    public string DestinationDirectory { get; set; }

    public override string ToString()
    {
        return $"{nameof(SourceDirectory)}: {SourceDirectory}, {nameof(DestinationDirectory)}: {DestinationDirectory}";
    }
}