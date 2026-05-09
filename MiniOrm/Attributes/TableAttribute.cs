using System;

namespace MiniOrm.Attributes;

[AttributeUsage(AttributeTargets.Class)]
/*
It does the exact same job as ColumnAttribute but for the table name instead of the column names
*/
public class TableAttribute : Attribute
{
    public string Name { get; }
    public TableAttribute(string name)
    {
        Name = name;
    }
}