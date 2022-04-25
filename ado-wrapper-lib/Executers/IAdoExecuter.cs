using Microsoft.Data.SqlClient;

namespace ado_wrapper_lib;

public interface IAdoExecuter<ParamsType, ResultType>
{
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
                                           int timeout = 480);
    /// <summary>
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
                                    int timeout = 480);
}

