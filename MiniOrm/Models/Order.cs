using MiniOrm.Attributes;
namespace MiniOrm.Models;
[Table("Orders")]
public class Order
{
    [PrimaryKey]
    public int ID { get; set;}
    
    [Column("ProductName")]
    public string Pname { get; set; }
}