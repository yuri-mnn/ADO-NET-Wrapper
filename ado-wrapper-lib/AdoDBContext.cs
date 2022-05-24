using ado_wrapper_lib.Executers.Impl;
using Microsoft.Data.SqlClient;

namespace ado_wrapper_lib;

public class AdoDBContext : IAdoDBContext
{
    private readonly string _connectionString;
    public string ConnectionString => _connectionString;

    public AdoDBContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<TResult>? Execute<TResult>(AdoParams adoParams, TypeExecuter typeExecuter)
    {
        AdoExecuter<TResult>? procExecuter = null;
        if (typeExecuter == TypeExecuter.Procedure)
            procExecuter = new ProcedureExecuter<TResult>(_connectionString);
        else if (typeExecuter == TypeExecuter.Function) 
            procExecuter = new FuncExecuter<TResult>(_connectionString);
        else
            throw new InvalidOperationException("Executer not supported");

        return procExecuter?.Execute(name: adoParams.Name, 
                                     dbParams: adoParams?.DBParams, 
                                     outputDBParams: out adoParams.OutputDBParams, 
                                     connection: adoParams.Connection,
                                     transaction: adoParams.Transaction, 
                                     timeout: (adoParams.Timeout ?? 120));
    }

    public TResult? ExecuteScalar<TResult>(AdoParams adoParams, TypeExecuter typeExecuter)
    {
        AdoExecuter<TResult>? procExecuter = null;
        if (typeExecuter == TypeExecuter.Procedure)
            procExecuter = new ProcedureExecuter<TResult>(_connectionString);
        else if (typeExecuter == TypeExecuter.Function)
            procExecuter = new FuncExecuter<TResult>(_connectionString);
        else
            throw new InvalidOperationException("Executer not supported");

        return procExecuter.ExecuteScalar(name: adoParams.Name,
                                          dbParams: adoParams?.DBParams,
                                          outputDBParams: out adoParams.OutputDBParams,
                                          connection: adoParams.Connection,
                                          transaction: adoParams.Transaction,
                                          timeout: (adoParams.Timeout ?? 120));
    }

    public void Execute(AdoParams adoParams, TypeExecuter typeExecuter)
    {
        AdoExecuter<Dictionary<string, object?>>? procExecuter = null;
        if (typeExecuter == TypeExecuter.Procedure)
            procExecuter = new ProcedureExecuter<Dictionary<string, object?>>(_connectionString);
        else if (typeExecuter == TypeExecuter.Function)
            procExecuter = new FuncExecuter<Dictionary<string, object?>>(_connectionString);
        else
            throw new InvalidOperationException("Executer not supported");

        procExecuter?.ExecuteWithouResult(name: adoParams.Name,
                                          dbParams: adoParams?.DBParams,
                                          outputDBParams: out adoParams.OutputDBParams,
                                          connection: adoParams.Connection,
                                          transaction: adoParams.Transaction,
                                          timeout: (adoParams.Timeout ?? 120));
    }
}


public enum TypeExecuter
{
    Procedure,
    Function,
    Query
}

public class AdoParams
{
 
    public AdoParams(string name,
                     object? dbParams = null,
                     SqlConnection? connection = null,
                     SqlTransaction? transaction = null,
                     int? timeout = 120)
    {
        Name = name;
        DBParams = dbParams;
        Connection = connection;
        Transaction = transaction;
        Timeout = timeout;
    }

    public string Name { get; set; }
    public object? DBParams { get; set; }
    public SqlConnection? Connection { get; set; }
    public SqlTransaction? Transaction { get; set; }
    public int? Timeout { get; set; }

    public Dictionary<string, object?> OutputDBParams = null!;
}
