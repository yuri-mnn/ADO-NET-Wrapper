using System.Data;

namespace AdoWrapper.Attributes;

[System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Parameter)]
public class DBColumnAttribute : System.Attribute
{
    public SqlDbType Type { set; get; }
    public string? Name { set; get; }
    public int Lenght { get; set; }
    public bool Mapped { get; set; } = true;
    public bool Output { get; set; } = false;
    public bool Input { get; set; } = true;
    public DBColumnAttribute() { }
}