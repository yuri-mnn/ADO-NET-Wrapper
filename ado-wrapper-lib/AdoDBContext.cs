using ado_wrapper_lib.Config;
using ado_wrapper_lib.Executers.Impl;
using Microsoft.Data.SqlClient;

namespace ado_wrapper_lib;

public class AdoDBContext : IAdoDBContext
{
    public AdoDBContext(string connectionString)
    {
        Connection.connectionString = connectionString;
    }

    public IEnumerable<TResult>? Execute<TParam, TResult>(AdoParams<TParam> adoParams, TypeExecuter typeExecuter)
    {
        IAdoExecuter<TParam, TResult>? procExecuter = null;
        if (typeExecuter == TypeExecuter.Procedure)
            procExecuter = new ProcedureExecuter<TParam, TResult>();
        else
            throw new InvalidOperationException("Executer not supported");

        return procExecuter?.Execute(name: adoParams.Name, dbParams: adoParams.DBParams, connection: adoParams.Connection,
                                    transaction: adoParams.Transaction, timeout: (adoParams.Timeout ?? 120)) ?? null;
    }

    public void Execute<TParam>(AdoParams<TParam> adoParams, TypeExecuter typeExecuter)
    {
        IAdoExecuter<TParam, object>? procExecuter = null;
        if (typeExecuter == TypeExecuter.Procedure)
            procExecuter = new ProcedureExecuter<TParam, object>();
        else
            throw new InvalidOperationException("Executer not supported");

        procExecuter?.ExecuteWithouResult(name: adoParams.Name, dbParams: adoParams.DBParams, connection: adoParams.Connection,
                                          transaction: adoParams.Transaction, timeout: (adoParams.Timeout ?? 120));
    }
}

public enum TypeExecuter
{
    Procedure,
    Function,
    Query
}

public record AdoParams<ParamsType>(string Name,
                                    ParamsType? DBParams = default,
                                    SqlConnection? Connection = null,
                                    SqlTransaction? Transaction = null,
                                    int? Timeout = 120);
