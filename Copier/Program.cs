// See https://aka.ms/new-console-template for more information

using Copier;
using Dumpify;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectre.Console;
using System.Collections.Immutable;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", false, true);
        config.AddJsonFile("appsettings.custom.json", true, true);
        config.AddEnvironmentVariables();

    })
    .ConfigureServices((context,config) =>
    {
        config.Configure<CombosSettings>(context.Configuration.GetSection("Combos"));
    })
    .Build();


var combos = host.Services.GetRequiredService<IOptions<CombosSettings>>().Value;
var prompt = new Prompt(combos).SpectrePrompt();

var process = new ProcessWrapper(x => AnsiConsole.MarkupLineInterpolated($"[green]{x}[/]"), x => AnsiConsole.MarkupLineInterpolated($"[red]{x}[/]"));
await process.Run("robocopy",$""" "{prompt.Source()}" "{prompt.Destination()}" /S /MT:8""");