namespace ado_wrapper_lib;

public interface IAdoDBContext
{
    public IEnumerable<TResult>? Execute<TParam, TResult>(AdoParams adoParams, TypeExecuter typeExecuter);
    public TResult? ExecuteScalar<TParam, TResult>(AdoParams adoParams, TypeExecuter typeExecuter);
    public void Execute<TParam>(AdoParams adoParams, TypeExecuter typeExecuter);
}
