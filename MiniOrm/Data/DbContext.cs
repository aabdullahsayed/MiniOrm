using System;
using System.Linq;
using System.Reflection;
using Npgsql;

namespace MiniOrm.Data;
/*
holds the actual database connection and uses reflection to automatically build  tables 
 */
public abstract class DbContext : IDisposable
{

    internal NpgsqlConnection Connection { get; }

    protected DbContext(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is missing!", nameof(connectionString));
        }
        
        Connection = new NpgsqlConnection(connectionString);
        Connection.Open();
        
        InitializeDbSets();
    }

    private void InitializeDbSets()
    {
        
        var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType && 
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        
        foreach (var prop in properties)
        {
            var dbSetInstance = Activator.CreateInstance(prop.PropertyType, this);
            prop.SetValue(this, dbSetInstance);
        }
    }

    public void Dispose()
    {
        if (Connection != null)
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
            Connection.Dispose();
        }
    }
}