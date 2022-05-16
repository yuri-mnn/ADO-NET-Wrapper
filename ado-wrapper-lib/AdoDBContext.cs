using ado_wrapper_lib.Config;
using ado_wrapper_lib.Executers.Impl;
using Microsoft.Data.SqlClient;

namespace ado_wrapper_lib;

public class AdoDBContext : IAdoDBContext
{
    private readonly string _connectionString;

    public AdoDBContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<TResult>? Execute<TParam, TResult>(AdoParams adoParams, TypeExecuter typeExecuter)
    {
        AdoExecuter<TParam, TResult>? procExecuter = null;
        if (typeExecuter == TypeExecuter.Procedure)
            procExecuter = new ProcedureExecuter<TParam, TResult>(_connectionString);
        else if (typeExecuter == TypeExecuter.Function) 
            procExecuter = new FuncExecuter<TParam, TResult>(_connectionString);
        else
            throw new InvalidOperationException("Executer not supported");

        return procExecuter?.Execute(name: adoParams.Name, dbParams: (TParam)adoParams.DBParams, connection: adoParams.Connection,
                                    transaction: adoParams.Transaction, timeout: (adoParams.Timeout ?? 120)) ?? null;
    }

    public TResult? ExecuteScalar<TParam, TResult>(AdoParams adoParams, TypeExecuter typeExecuter)
    {
        AdoExecuter<TParam, TResult>? procExecuter = null;
        if (typeExecuter == TypeExecuter.Procedure)
            procExecuter = new ProcedureExecuter<TParam, TResult>(_connectionString);
        else if (typeExecuter == TypeExecuter.Function)
            procExecuter = new FuncExecuter<TParam, TResult>(_connectionString);
        else
            throw new InvalidOperationException("Executer not supported");

        return procExecuter.ExecuteScalar(name: adoParams.Name, dbParams: (TParam)adoParams.DBParams, connection: adoParams.Connection,
                                    transaction: adoParams.Transaction, timeout: (adoParams.Timeout ?? 120));
    }

    public void Execute<TParam>(AdoParams adoParams, TypeExecuter typeExecuter)
    {
        AdoExecuter<TParam, Dictionary<string, object?>>? procExecuter = null;
        if (typeExecuter == TypeExecuter.Procedure)
            procExecuter = new ProcedureExecuter<TParam, Dictionary<string, object?>>(_connectionString);
        else if (typeExecuter == TypeExecuter.Function)
            procExecuter = new FuncExecuter<TParam, Dictionary<string, object?>>(_connectionString);
        else
            throw new InvalidOperationException("Executer not supported");

        procExecuter?.ExecuteWithouResult(name: adoParams.Name, dbParams: (TParam)adoParams.DBParams, connection: adoParams.Connection,
                                          transaction: adoParams.Transaction, timeout: (adoParams.Timeout ?? 120));
    }
}

public enum TypeExecuter
{
    Procedure,
    Function,
    Query
}

public record AdoParams(string Name,
                        object? DBParams = default,
                                    SqlConnection? Connection = null,
                                    SqlTransaction? Transaction = null,
                                    int? Timeout = 120);
