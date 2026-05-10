using System;
using MiniOrm.Migrations.Commands;

string connStr = Environment.GetEnvironmentVariable("MINIORM_CONN");

var runner = new MigrationRunner(connStr);

Console.WriteLine("MiniOrm CLI");
Console.WriteLine("Valid commands: add <name>, apply, list, rollback, exit");

while (true)
{
    Console.Write("> ");
    string? input = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(input)) continue;

    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    string command = parts[0].ToLower();

    try
    {
        switch (command)
        {
            case "add":
                if (parts.Length < 2) 
                    throw new Exception("Provide a name  add <name>).");
                runner.AddMigration(parts[1]);
                break;

            case "apply":
                runner.Apply();
                break;

            case "list":
                runner.List();
                break;

            case "rollback":
                runner.Rollback();
                break;

            case "exit":
                return; 

            default:
                Console.WriteLine($"Command '{command}' does not work. Valid commands are: add, apply, list, rollback, exit.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}