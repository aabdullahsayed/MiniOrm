using System.Collections.Generic;
using System.Reflection;

namespace MiniOrm.Data;

public class ColumnMetadata
{
    public PropertyInfo Property { get; set; } = null!;
    public string ColumnName { get; set; } = null!;
    public string PostgresDefinition { get; set; } = null!;
    public bool IsPrimaryKey { get; set; }
}

public class EntityMetadata
{
    public string TableName { get; set; } = null!;
    public ColumnMetadata PrimaryKey { get; set; } = null!;
    public List<ColumnMetadata> Columns { get; set; } = new();
}