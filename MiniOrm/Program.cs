using System;
using MiniOrm;
using MiniOrm.Data;
using MiniOrm.Models;
using Npgsql;


var connStr = Environment.GetEnvironmentVariable("MINIORM_CONN");
var db = new AppDbContext(connStr);

Console.WriteLine();

//Create
var keyboard = new Product { Name = "Keyboard", Price = 89.99m, Discount = null, InStock = true };
int id = db.Products.Insert(keyboard);
Console.WriteLine("[ONE] Create:");
Console.WriteLine($"Inserted Product ID: {id}");

Console.WriteLine();

//Read (Single)
var found = db.Products.FindById(1);
Console.WriteLine("[TWO] Read Single Row Using ID: ");

if (found != null)
{
    Console.WriteLine($" Product Id : {found.Id}");
    Console.WriteLine($" Name : {found.Name}");
    Console.WriteLine($" Price : {found.Price}");
    Console.WriteLine($" Discount : {(found.Discount == null ? "NULL" : found.Discount)}");
    Console.WriteLine($" InStock : {found.InStock}");
}

Console.WriteLine();

//Read (All)
var products = db.Products.GetAll(); 
Console.WriteLine("[Three] Read All Rows ");

foreach (var p in products)
{
    Console.WriteLine(
        $"[{p.Id}] {p.Name} | Price = {p.Price} | Discount = {(p.Discount == null ? "NULL" : p.Discount)} | InStock = {p.InStock}"
    );
}

Console.WriteLine();

//Update
found.Price = 79.99m; found.Discount = 5.00m;
db.Products.Update(found);

var updated = db.Products.FindById(id);
Console.WriteLine("[Four] UPDATE ");
Console.WriteLine($" Updated Product Id : {updated.Id}");
Console.WriteLine($" New Price : {updated.Price}");
Console.WriteLine($" New Discount : {updated.Discount}");

Console.WriteLine();
//Delete

Console.WriteLine("[Five] DELETE");
/*
db.Products.Delete(1);
Console.WriteLine(" DELETE ");
Console.WriteLine($" Deleted Product Id = {id}");
*/