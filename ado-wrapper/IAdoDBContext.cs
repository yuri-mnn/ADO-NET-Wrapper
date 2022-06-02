namespace AdoWrapper;

public interface IAdoDBContext
{
    /// <summary>
    /// DB Connection string
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Execute  a query in the database with data retrieval
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="adoParams"></param>
    /// <param name="typeExecuter"></param>
    /// <returns></returns>
    public IEnumerable<TResult>? Execute<TResult>(AdoParams adoParams, TypeExecuter typeExecuter);


    /// <summary>
    /// Execute asynchronously a query in the database with data retrieval
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="adoParams"></param>
    /// <param name="typeExecuter"></param>
    /// <returns></returns>
    public Task<IEnumerable<TResult>?> ExecuteAsync<TResult>(AdoParams adoParams, TypeExecuter typeExecuter);

    /// <summary>
    /// Execute a query to the database with a scalar value
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="adoParams"></param>
    /// <param name="typeExecuter"></param>
    /// <returns></returns>
    public TResult? ExecuteScalar<TResult>(AdoParams adoParams, TypeExecuter typeExecuter);

    /// <summary>
    /// Execute а query without result
    /// </summary>
    /// <param name="adoParams"></param>
    /// <param name="typeExecuter"></param>
    public void Execute(AdoParams adoParams, TypeExecuter typeExecuter);
}
