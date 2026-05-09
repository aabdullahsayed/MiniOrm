using System;

namespace MiniOrm.Attributes;

[AttributeUsage(AttributeTargets.Property)]

/*
we need it to map C# naming conventions to SQL naming conventions
*/
public class ColumnAttribute : Attribute
{
    public string Name { get; }
    public ColumnAttribute(string name)
    {
        Name = name;
    }
}