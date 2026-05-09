using System;

namespace MiniOrm.Attributes;


[AttributeUsage(AttributeTargets.Property)]

/*
flags a specific C# property as the unique identifier (the Primary Key) for that table.
*/
public class PrimaryKeyAttribute : Attribute
{
}