using ado_wrapper_lib.Config;
using ado_wrapper_lib.Converters;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

namespace ado_wrapper_lib.Executers.Impl;

internal class ProcedureExecuter<ParamsType, ResultType> : IAdoExecuter<ParamsType, ResultType>
{
    private string _connectionString = Connection.connectionString;
    public IEnumerable<ResultType> Execute(string name,
                                           ParamsType dbParams, 
                                           SqlConnection? connection = null, 
                                           SqlTransaction? transaction = null, 
                                           int timeout = 480)
    {
        if (typeof(ParamsType).IsPrimitive) 
            throw new ArgumentException("Преобразования из примитивного типа в SqlParameter[] не реализовано");

        IList<ResultType> table;
        SqlParameter[] parameters;

        // Преобраз. обьекта в массив SQL параметров
        if (typeof(ParamsType) == typeof(SqlParameter[]))
        {
            parameters = dbParams as SqlParameter[];
        }
        else
        {
            parameters = ObjectConverter.ObjectToArraySQLParameters(dbParams);
        }


       

        if (typeof(ParamsType) == typeof(IList))
        {
            throw new InvalidOperationException("");
        }
        if (connection is null)
        {
            using SqlConnection newConnection = new(_connectionString);
            newConnection.Open();
            table = RunExecuteProc<ResultType>(name, parameters, newConnection, transaction, timeout);
        }
        else
            table = RunExecuteProc<ResultType>(name, parameters, connection, transaction, timeout);

        return table;
    }

    public void ExecuteWithouResult(string name,
                                    ParamsType dbParams, 
                                    SqlConnection? connection = null, 
                                    SqlTransaction? transaction = null, 
                                    int timeout = 480)
    {

        if (typeof(ParamsType).IsPrimitive)
            throw new ArgumentException("Преобразования из примитивного типа в SqlParameter[] не реализовано");

        IList<ResultType> table;
        SqlParameter[] parameters;

        // Преобраз. обьекта в массив SQL параметров
        if (typeof(ParamsType) == typeof(SqlParameter[]))
        {
            parameters = dbParams as SqlParameter[];
        }
        else
        {
            parameters = ObjectConverter.ObjectToArraySQLParameters(dbParams);
        }

        if (connection == null)
        {
            using SqlConnection connectionNew = new(_connectionString);
            connectionNew.Open();
            RunExecuteProcWithoutResult(name, parameters, connection, transaction, timeout);

        }
        else
        {
            RunExecuteProcWithoutResult(name, parameters, connection, transaction, timeout);
        }
    }




     private IList<T> RunExecuteProc<T>(string nameProc,
                                      SqlParameter[]? paramsProc,
                                      SqlConnection connection,
                                      SqlTransaction? transaction,
                                      int timeout = 480
                                      )
    {
       
        IList<T> table;
        using SqlCommand command = new(nameProc, connection);
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

    private void RunExecuteProcWithoutResult(string nameProc,
                                            SqlParameter[]? paramsProc,
                                            SqlConnection connection,
                                            SqlTransaction? transaction,
                                            int timeout)
    {
       
        using SqlCommand command = new(nameProc, connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        if (paramsProc != null) 
            command.Parameters.AddRange(paramsProc);
        command.Transaction = transaction;
        command.CommandTimeout = timeout;
        command.ExecuteNonQuery();
    }
}

