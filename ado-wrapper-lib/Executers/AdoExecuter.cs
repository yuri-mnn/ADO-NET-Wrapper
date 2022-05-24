using ado_wrapper_lib.Converters;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

namespace ado_wrapper_lib;

public abstract class AdoExecuter<ResultType>
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
                                           object? dbParams,
                                           out Dictionary<string, object?> outputDBParams,
                                           SqlConnection? connection = null,
                                           SqlTransaction? transaction = null,
                                           int timeout = 480
                                           )
    {
      
        IList<ResultType> table;
        SqlParameter[] parameters;

        // Преобраз. обьекта в массив SQL параметров
        if (dbParams is not null)
        {
            if (dbParams.GetType().IsPrimitive)
                throw new ArgumentException("Преобразования из примитивного типа в SqlParameter[] не реализовано");

            if (dbParams?.GetType() == typeof(SqlParameter[]))
            {
                parameters = dbParams as SqlParameter[];
            }
            else
            {
                parameters = ObjectConverter.ObjectToArraySQLParameters(dbParams);
            }

            if (dbParams?.GetType() == typeof(IList))
            {
                throw new InvalidOperationException("");
            }
        }
        else
        {
            parameters = new SqlParameter[] { };
        }

        if (connection is null)
        {
            using SqlConnection newConnection = new(_connectionString);
            newConnection.Open();
            table = RunExecuter<ResultType>(name, parameters, newConnection, transaction, timeout);
        }
        else
            table = RunExecuter<ResultType>(name, parameters, connection, transaction, timeout);


        outputDBParams = GetValueOutputSqlParams(parameters);
        return table;
    }

    /// <summary>
    /// Выполнение процедуры, функции, запроса с результатом скалярным значением
    /// </summary>
    /// <param name="name"></param>
    /// <param name="dbParams"></param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="timeout"></param>
    /// <param name=""></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public ResultType ExecuteScalar(string name,
                                    object? dbParams,
                                    out Dictionary<string, object?> outputDBParams,
                                    SqlConnection? connection = null,
                                    SqlTransaction? transaction = null,
                                    int timeout = 480)
    {
       
        ResultType resultValue;
        SqlParameter[] parameters;

        // Преобраз. обьекта в массив SQL параметров
        if (dbParams is not null)
        {
            if (dbParams.GetType().IsPrimitive)
                throw new ArgumentException("Преобразования из примитивного типа в SqlParameter[] не реализовано");

            if (dbParams?.GetType() == typeof(SqlParameter[]))
            {
                parameters = dbParams as SqlParameter[];
            }
            else
            {
                parameters = ObjectConverter.ObjectToArraySQLParameters(dbParams);
            }

            if (dbParams?.GetType() == typeof(IList))
            {
                throw new InvalidOperationException("");
            }
        }
        else
        {
            parameters = new SqlParameter[] {};
        }


 
        if (connection is null)
        {
            using SqlConnection newConnection = new(_connectionString);
            newConnection.Open();
            resultValue = RunScalarExecuter<ResultType>(name, parameters, newConnection, transaction, timeout);
        }
        else
            resultValue = RunScalarExecuter<ResultType>(name, parameters, connection, transaction, timeout);

        outputDBParams = GetValueOutputSqlParams(parameters);
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
                                    object? dbParams,
                                    out Dictionary<string, object?> outputDBParams,
                                    SqlConnection? connection = null,
                                    SqlTransaction? transaction = null,
                                    int timeout = 480)
    {

       

        IList<ResultType> table;
        SqlParameter[] parameters;

        // Преобраз. обьекта в массив SQL параметров
        if (dbParams is not null)
        {
            if (dbParams.GetType().IsPrimitive)
                throw new ArgumentException("Преобразования из примитивного типа в SqlParameter[] не реализовано");

            if (dbParams?.GetType() == typeof(SqlParameter[]))
            {
                parameters = dbParams as SqlParameter[];
            }
            else
            {
                parameters = ObjectConverter.ObjectToArraySQLParameters(dbParams);
            }

            if (dbParams?.GetType() == typeof(IList))
            {
                throw new InvalidOperationException("");
            }
        }
        else
        {
            parameters = new SqlParameter[] { };
        }

        if (connection == null)
        {
            using SqlConnection connectionNew = new(_connectionString);
            connectionNew.Open();
            RunExecuterWithoutResult(name, parameters, connectionNew, transaction, timeout);

        }
        else
        {
            RunExecuterWithoutResult(name, parameters, connection, transaction, timeout);
        }
        outputDBParams = GetValueOutputSqlParams(parameters);

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

    private Dictionary<string, object?> GetValueOutputSqlParams(SqlParameter[] sqlParameters)
    {
        IDictionary<string, object?> outputValues = new Dictionary<string, object?>();
        foreach (SqlParameter parameter in sqlParameters)
        {
            if (parameter.Direction == ParameterDirection.Output ||
                parameter.Direction == ParameterDirection.InputOutput ||
                parameter.Direction == ParameterDirection.ReturnValue)
            {
                outputValues.Add(parameter.ParameterName, parameter.Value);
            }
        }
        return (Dictionary<string, object?>)outputValues;
    }

}

