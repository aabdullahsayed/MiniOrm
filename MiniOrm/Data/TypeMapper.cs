using System;
using System.Linq;
using System.Reflection;
using MiniOrm.Attributes;

namespace MiniOrm.Data;

/*
C#-to-SQL type mapping
nullability checking
packages everything into a reusable Metadata
 */
public static class TypeMapper
{
    public static EntityMetadata GetMetadata(Type type)
    {
        var metadata = new EntityMetadata();


        var tableAttr = type.GetCustomAttribute<TableAttribute>();
        metadata.TableName = tableAttr != null ? tableAttr.Name : type.Name.ToLower();
        
        var nullabilityContext = new NullabilityInfoContext();

        foreach (var prop in type.GetProperties())
        {
            var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
            var pkAttr = prop.GetCustomAttribute<PrimaryKeyAttribute>();

            //Skip any property without a [Column] or [PrimaryKey]
            if (columnAttr == null && pkAttr == null) continue;

            var colMeta = new ColumnMetadata
            {
                Property = prop,
                ColumnName = columnAttr?.Name ?? prop.Name.ToLower(),
                IsPrimaryKey = pkAttr != null
            };

            
            bool isNullableValueType = Nullable.GetUnderlyingType(prop.PropertyType) != null;
            
            
            var nullabilityInfo = nullabilityContext.Create(prop);
            bool isNullableRefType = nullabilityInfo.WriteState == NullabilityState.Nullable;
            
            bool isNullable = isNullableValueType || isNullableRefType;

            colMeta.PostgresDefinition = GetPostgresDefinition(prop.PropertyType, colMeta.IsPrimaryKey, isNullable);
            
            if (colMeta.IsPrimaryKey)
            {
                metadata.PrimaryKey = colMeta;
            }

            metadata.Columns.Add(colMeta);
        }

        return metadata;
    }

    private static string GetPostgresDefinition(Type type, bool isPrimaryKey, bool isNullable)
    {
        Type cleanType = Nullable.GetUnderlyingType(type) ?? type;

        if (isPrimaryKey && cleanType == typeof(int))
            return "SERIAL PRIMARY KEY";

        string pgType = cleanType.Name switch
        {
            nameof(Int32) => "INTEGER",
            nameof(Int64) => "BIGINT",
            nameof(Single) => "REAL",
            nameof(Double) => "DOUBLE PRECISION",
            nameof(Decimal) => "NUMERIC",
            nameof(Boolean) => "BOOLEAN",
            nameof(DateTime) => "TIMESTAMP",
            nameof(Guid) => "UUID",
            nameof(String) => "TEXT",
            _ => throw new NotSupportedException($"Type {cleanType.Name} is not supported.")
        };

        string nullability = isNullable ? "NULL" : "NOT NULL";
        return $"{pgType} {nullability}";
    }
}