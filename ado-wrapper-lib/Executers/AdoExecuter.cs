using ado_wrapper_lib.Converters;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace ado_wrapper_lib;

public abstract class AdoExecuter<ParamsType, ResultType>
{
    protected string _connectionString = null!;
    
    /// <summary>
    /// Выполнение процедуры, функции, запроса в БД
    /// </summary>
    /// <param name="name">имя процедуры, функции или запроса</param>
    /// <param name="dbParams">параметры</param>
    /// <param name="connection">соединение</param>
    /// <param name="transaction">текущая транзакция</param>
    /// <param name="timeout">максмальное время выполнения запроса</param>
    /// <returns>Словарь или лист с результатом</returns>
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
            table = RunExecuter<ResultType>(name, parameters, newConnection, transaction, timeout);
        }
        else
            table = RunExecuter<ResultType>(name, parameters, connection, transaction, timeout);

        return table;
    }

    public ResultType ExecuteScalar(string name,
                                    ParamsType dbParams,
                                    SqlConnection? connection = null,
                                    SqlTransaction? transaction = null,
                                    int timeout = 480)
    {
        if (typeof(ParamsType).IsPrimitive)
            throw new ArgumentException("Преобразования из примитивного типа в SqlParameter[] не реализовано");

        ResultType resultValue;
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
            resultValue = RunScalarExecuter<ResultType>(name, parameters, newConnection, transaction, timeout);
        }
        else
            resultValue = RunScalarExecuter<ResultType>(name, parameters, connection, transaction, timeout);

        return resultValue;
    }



    /// Выполнение процедуры, функции, запроса в БД без результата
    /// </summary>
    /// <param name="name">имя процедуры, функции или запроса</param>
    /// <param name="dbParams">параметры</param>
    /// <param name="connection">соединение</param>
    /// <param name="transaction">текущая транзакция</param>
    /// <param name="timeout">максмальное время выполнения запроса</param>
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
            RunExecuterWithoutResult(name, parameters, connection, transaction, timeout);

        }
        else
        {
            RunExecuterWithoutResult(name, parameters, connection, transaction, timeout);
        }
    }

    protected abstract IList<T> RunExecuter<T>(string name,
                                 SqlParameter[]? paramsProc,
                                 SqlConnection connection,
                                 SqlTransaction? transaction,
                                 int timeout = 480);

    protected abstract void RunExecuterWithoutResult(string name,
                                                     SqlParameter[]? paramsProc,
                                                     SqlConnection connection,
                                                     SqlTransaction? transaction,
                                                     int timeout);
    protected abstract T RunScalarExecuter<T>(string name, 
                                              SqlParameter[]? parameters, 
                                              SqlConnection connection, 
                                              SqlTransaction? transaction, 
                                              int timeout);

}

