using System.Data;

namespace ado_wrapper_lib.Attributes;

[System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Parameter)]
internal class DBColumnAttribute : System.Attribute
{
    public SqlDbType Type { set; get; }
    public string? Name { set; get; }
    public int Lenght { get; set; }
    public bool Mapped { get; set; } = true;
    public bool Output { get; set; } = false;
    public DBColumnAttribute() { }
}