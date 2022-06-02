using AdoWrapper.Converters;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

namespace AdoWrapper.Executers.Impl;

internal class ProcedureExecuter<ResultType> : AdoExecuter<ResultType>
{
    public ProcedureExecuter(string connectionStr) => _connectionString = connectionStr;

    protected override IList<T> RunExecuter<T>(string name, 
                                               SqlParameter[]? paramsProc, 
                                               SqlConnection connection, 
                                               SqlTransaction? transaction, 
                                               int timeout = 480)
    {
        IList<T> table;
        using SqlCommand command = new(name, connection);
        command.CommandType = CommandType.StoredProcedure;
        if (paramsProc != null)
            command.Parameters.AddRange(paramsProc);
        command.Transaction = transaction;
        command.CommandTimeout = timeout;
        // Выполнение процедуры и преобразование результата в List
        using var reader = command.ExecuteReader();
        table = ObjectConverter.DataReaderToList<T>(reader);
        reader.Close();
        return table;
    }
    
    protected override async Task<IList<T>> RunExecuterAsync<T>(string name,
                                            SqlParameter[]? paramsProc,
                                            SqlConnection connection,
                                            SqlTransaction? transaction,
                                            int timeout = 480)
    {
        IList<T> table;
        using SqlCommand command = new(name, connection);
        command.CommandType = CommandType.StoredProcedure;
        if (paramsProc != null)
            command.Parameters.AddRange(paramsProc);
        command.Transaction = transaction;
        command.CommandTimeout = timeout;
        // Выполнение процедуры и преобразование результата в List
        using var reader = await command.ExecuteReaderAsync();
        table = ObjectConverter.DataReaderToList<T>(reader);
        await reader.CloseAsync();
        return table;
    }

    protected override void RunExecuterWithoutResult(string name, 
                                                     SqlParameter[]? paramsProc, 
                                                     SqlConnection connection, 
                                                     SqlTransaction? transaction, 
                                                     int timeout)
    {
        using SqlCommand command = new(name, connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        if (paramsProc != null)
            command.Parameters.AddRange(paramsProc);
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
        using SqlCommand command = new(name, connection);
        //command.CommandType = CommandType.StoredProcedure;
        if (parameters != null)
            command.Parameters.AddRange(parameters);
        command.Transaction = transaction;
        command.CommandTimeout = timeout;

        var dbValue = command.ExecuteScalar();
        T resultValue = Convert.IsDBNull(dbValue) ? default : (T)dbValue;
        return resultValue;
    }
}

