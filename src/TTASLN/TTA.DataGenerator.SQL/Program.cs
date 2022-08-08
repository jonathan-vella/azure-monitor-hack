﻿using System.Data.SqlClient;
using Bogus;
using Dapper;
using Spectre.Console;
using TTA.Models;

AnsiConsole.Write(new FigletText("Data generator for TTA demo app").Centered().Color(Color.Red));
var sqlConn = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
if (string.IsNullOrEmpty(sqlConn))
    sqlConn = @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=SSPI";

var sqlConnection = new SqlConnection(sqlConn);
try
{
    sqlConnection.Open();
    AnsiConsole.Write(
        new Markup($"[bold yellow]SQL connection to database[/] [red]{sqlConn}[/] [bold yellow]is opened.[/]"));
    AnsiConsole.WriteLine();
}
catch (Exception sqlCouldNotOpenException)
{
    AnsiConsole.WriteException(sqlCouldNotOpenException);
    return;
}

var dropIt =
    AnsiConsole.Confirm("Database will be recreated. Create backup before continuing. Are you sure to continue?");
if (!dropIt)
{
    AnsiConsole.WriteLine("Shutting down - run the app when backup is finished to be able to work with your data.");
    return;
}

var folderRoot = Environment.GetEnvironmentVariable("FOLDER_ROOT");
if (string.IsNullOrEmpty(folderRoot))
{
    folderRoot = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter [green]folder root[/]")
            .PromptStyle("green")
            .ValidationErrorMessage(
                "[red]That's not a valid folder - check https://https://github.com/vrhovnik/azure-monitor-automation-wth[/]")
            .Validate(Directory.Exists));
}

AnsiConsole.WriteLine("You have entered the following:");
var path = new TextPath(folderRoot)
{
    RootStyle = new Style(foreground: Color.Red),
    SeparatorStyle = new Style(foreground: Color.Green),
    StemStyle = new Style(foreground: Color.Blue),
    LeafStyle = new Style(foreground: Color.Yellow)
};
AnsiConsole.Write(path);

var dbExistsCount = await sqlConnection.QuerySingleOrDefaultAsync<int>("SELECT count(*) FROM master.dbo.sysdatabases WHERE name = 'TTADB'");
if (dbExistsCount > 0)
{
    AnsiConsole.Write(
        new Markup("[bold yellow]Database [/] [red]TTADB[/] [bold yellow] will be dropped."));
    try
    {
        var affectedRows = await sqlConnection.ExecuteAsync("DROP DATABASE TTADB");
        if (affectedRows == 0) throw new Exception("DROP was not successful, check logs");
    }
    catch (Exception dropException)
    {
        AnsiConsole.WriteException(dropException);
        return;
    }
}

//read script
var initSqlDataScript = await File.ReadAllTextAsync(Path.Combine(folderRoot, "/scripts/SQL/0-init-database.sql"));

try
{
    var affectedRows = await sqlConnection.ExecuteAsync(initSqlDataScript);
    if (affectedRows == 0) throw new Exception("Creation of database and tables was not successful, check logs");
}
catch (Exception initException)
{
    AnsiConsole.WriteException(initException);
    return;
}

await AnsiConsole.Status()
    .AutoRefresh(false)
    .Spinner(Spinner.Known.Dots6)
    .SpinnerStyle(Style.Parse("green bold"))
    .StartAsync("Generating data started ...", async ctx =>
    {
        //0. make sure DB is created and structure populated
        ctx.Status("Getting tags and inserting tags ...");
        ctx.Refresh();
        
        //generate data with Bogus library
        //1. Tags
        var tagFilePath = Path.Join(folderRoot, "docs/tag.data");
        var tagFile = await File.ReadAllTextAsync(tagFilePath);
        ctx.Status($"Reading from file {tagFilePath}");
        ctx.Refresh();
        
        var tags = tagFile.Split(',');
        foreach (var currentTag in tags)
        {
            ctx.Status($"Traversing through - current tag {currentTag}");
            ctx.Refresh();
        }

        //2. Categories
        //3. Users
        //4. WorkTasks
    });