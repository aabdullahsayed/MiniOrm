using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Npgsql;
using MiniOrm.Data;
using MiniOrm.Attributes;

namespace MiniOrm.Migrations.Commands;

/*
 Manages the entire lifecycle of SQL migrations (Create, Apply, List, Rollback)
Dynamically generates Up/Down scripts from C# models
 
 */
public class MigrationRunner
{
    private readonly string _connString;
    private readonly string _migrationsFolder;

    public MigrationRunner(string connString)
    {
        _connString = connString;
        
        _migrationsFolder = Path.Combine(Directory.GetCurrentDirectory(), "SqlMigrations");
        if (!Directory.Exists(_migrationsFolder)) Directory.CreateDirectory(_migrationsFolder);
        
        EnsureMigrationsTableExists();
    }

    // Add Migration
    public void AddMigration(string name)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string filename = $"{timestamp}_{name}.sql";
        string filePath = Path.Combine(_migrationsFolder, filename);

        
        var assembly = Assembly.Load("MiniOrm");
        var entityTypes = assembly.GetTypes().Where(t => t.GetCustomAttribute<TableAttribute>() != null);

        var upSql = new List<string>();
        var downSql = new List<string>();

        foreach (var type in entityTypes)
        {
            var meta = TypeMapper.GetMetadata(type);

            var colStrings = meta.Columns.Select(c => $"    {c.ColumnName} {c.PostgresDefinition}");
            string createTable = $"CREATE TABLE IF NOT EXISTS {meta.TableName} (\n{string.Join(",\n", colStrings)}\n);";
            
            upSql.Add(createTable);
            downSql.Add($"DROP TABLE IF EXISTS {meta.TableName};");
        }

        string fileContent = $"-- up\n{string.Join("\n\n", upSql)}\n\n-- down\n{string.Join("\n", downSql)}";
        
        File.WriteAllText(filePath, fileContent);
        Console.WriteLine($"✅ Generated migration file: {filename}");
    }

    // Apply Migration
    public void Apply()
    {
        var applied = GetAppliedMigrations();
        var files = Directory.GetFiles(_migrationsFolder, "*.sql").OrderBy(f => f).ToList();
        
        int count = 0;
        using var conn = new NpgsqlConnection(_connString);
        conn.Open();

        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);
            if (applied.Contains(fileName)) continue; 

            string content = File.ReadAllText(file);
            string upScript = content.Split("-- down")[0].Replace("-- up", "").Trim();

            using var tx = conn.BeginTransaction(); // Wrap in transaction for safety
            try
            {
                using (var cmd = new NpgsqlCommand(upScript, conn, tx)) cmd.ExecuteNonQuery();
                
                string insertSql = "INSERT INTO __migrations (filename, applied_on) VALUES (@name, @date)";
                using (var cmd = new NpgsqlCommand(insertSql, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@name", fileName);
                    cmd.Parameters.AddWithValue("@date", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();
                }
                
                tx.Commit();
                Console.WriteLine($"✅ Applied: {fileName}");
                count++;
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
        }
        if (count == 0) Console.WriteLine("Database is already up to date.");
    }

    // Migrations list
    public void List()
    {
        var applied = GetAppliedMigrations();
        var files = Directory.GetFiles(_migrationsFolder, "*.sql").OrderBy(f => f).Select(Path.GetFileName).ToList();

        Console.WriteLine("\n--- Migration History ---");
        foreach (var file in files)
        {
            string status = applied.Contains(file) ? "[Applied]" : "[Pending]";
            Console.WriteLine($"{status.PadRight(10)} {file}");
        }
        if (files.Count == 0) Console.WriteLine("No migrations found.");
    }
    
    public void Rollback()
    {
        var applied = GetAppliedMigrations();
        if (!applied.Any())
        {
            Console.WriteLine("No applied migrations to rollback.");
            return;
        }

        string lastApplied = applied.Last();
        string filePath = Path.Combine(_migrationsFolder, lastApplied);

        if (!File.Exists(filePath)) throw new Exception($"Migration file missing: {filePath}");

        string content = File.ReadAllText(filePath);
        var parts = content.Split("-- down");
        if (parts.Length < 2) throw new Exception("Could not find '-- down' section in file.");
        
        string downScript = parts[1].Trim();

        using var conn = new NpgsqlConnection(_connString);
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            using (var cmd = new NpgsqlCommand(downScript, conn, tx)) cmd.ExecuteNonQuery();
            
            using (var cmd = new NpgsqlCommand("DELETE FROM __migrations WHERE filename = @name", conn, tx))
            {
                cmd.Parameters.AddWithValue("@name", lastApplied);
                cmd.ExecuteNonQuery();
            }
            
            tx.Commit();
            Console.WriteLine($"⏪ Rolled back: {lastApplied}");
        }
        catch (Exception)
        {
            tx.Rollback();
            throw;
        }
    }

    private void EnsureMigrationsTableExists()
    {
        using var conn = new NpgsqlConnection(_connString);
        conn.Open();
        string sql = @"
            CREATE TABLE IF NOT EXISTS __migrations (
                id SERIAL PRIMARY KEY,
                filename TEXT NOT NULL UNIQUE,
                applied_on TIMESTAMP NOT NULL
            );";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }

    private List<string> GetAppliedMigrations()
    {
        var list = new List<string>();
        using var conn = new NpgsqlConnection(_connString);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT filename FROM __migrations ORDER BY id ASC", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) list.Add(reader.GetString(0));
        return list;
    }
}