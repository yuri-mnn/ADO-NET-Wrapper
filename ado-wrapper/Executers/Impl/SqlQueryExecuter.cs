using AdoWrapper.Converters;
using Microsoft.Data.SqlClient;

namespace AdoWrapper.Executers.Impl;

internal class SqlQueryExecuter<ResultType> : AdoExecuter<ResultType>
{
    protected override IList<T> RunExecuter<T>(string name, 
                                               SqlParameter[]? parameters, 
                                               SqlConnection connection, 
                                               SqlTransaction? transaction, 
                                               int timeout = 480)
    {
        IList<T> table;
        using SqlCommand command = new(name, connection);
        //command.CommandType = CommandType.StoredProcedure;
        if (parameters is not null)
            command.Parameters.AddRange(parameters);
        command.Transaction = transaction;
        command.CommandTimeout = timeout;

        using var reader = command.ExecuteReader();
        table = ObjectConverter.DataReaderToList<T>(reader);
        reader.Close();
        return table;
    }

    protected override Task<IList<T>> RunExecuterAsync<T>(string name, SqlParameter[]? paramsProc, SqlConnection connection, SqlTransaction? transaction, int timeout = 480)
    {
        throw new NotImplementedException();
    }

    protected override void RunExecuterWithoutResult(string name, 
                                                     SqlParameter[]? parameters, 
                                                     SqlConnection connection, 
                                                     SqlTransaction? transaction, 
                                                     int timeout)
    {
        using SqlCommand command = new(name, connection);
        //command.CommandType = CommandType.StoredProcedure;
        if (parameters is not null)
            command.Parameters.AddRange(parameters);
        command.Transaction = transaction;
        command.CommandTimeout = timeout;
        command.ExecuteNonQuery();
    }

    protected override T RunScalarExecuter<T>(string name, 
                                              SqlParameter[]? parameters, 
                                              SqlConnection connection, 
                                              SqlTransaction? transaction, 
                                              int timeout)
    {
        throw new NotImplementedException();
    }
}
