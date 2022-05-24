namespace ado_wrapper_lib;

public interface IAdoDBContext
{
    public string ConnectionString { get; }
    public IEnumerable<TResult>? Execute<TResult>(AdoParams adoParams, TypeExecuter typeExecuter);
    public TResult? ExecuteScalar<TResult>(AdoParams adoParams, TypeExecuter typeExecuter);
    public void Execute(AdoParams adoParams, TypeExecuter typeExecuter);
}
