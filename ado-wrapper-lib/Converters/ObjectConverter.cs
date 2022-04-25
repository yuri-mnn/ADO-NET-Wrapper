using ado_wrapper_lib.Attributes;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Reflection;

namespace ado_wrapper_lib.Converters;

public class ObjectConverter
{
    public static SqlParameter[] ObjectToArraySQLParameters<T>(T obj)
    {
        IList<SqlParameter> sqlParams = new List<SqlParameter>();
        SqlParameter sqlParam;

        try
        {
            Type typeGeneric = typeof(T);
            if (typeGeneric.IsPrimitive)
            {

                throw new InvalidCastException("Невозможно преобразовать примитивный тип в массив SQLParameters");
            }
            foreach (PropertyInfo prop in typeGeneric.GetProperties())
            {
                DBColumnAttribute? dBColumnAttribute = prop.GetCustomAttribute<DBColumnAttribute>(true);

                if (dBColumnAttribute?.Mapped ?? true)
                {
                    sqlParam = new SqlParameter()
                    {
                        ParameterName = "@" + (dBColumnAttribute?.Name ?? prop.Name),
                        Value = obj?.GetType().GetProperty(prop.Name)?.GetValue(obj, null) ?? Convert.DBNull,
                        Direction = dBColumnAttribute?.Output == true ? ParameterDirection.InputOutput 
                                                                      : ParameterDirection.Input,
                    };

                    if (dBColumnAttribute?.Type != null) 
                        sqlParam.SqlDbType = (dBColumnAttribute?.Type ?? default);
                    if (dBColumnAttribute?.Lenght != null) 
                        sqlParam.Size = dBColumnAttribute?.Lenght ?? 0;

                    sqlParams.Add(sqlParam);
                }
            }

            return sqlParams.ToArray();
        }
        catch (Exception e) { throw new Exception("Ошибка преобразования generic object в array SqlParameter, подробнности:" + e.Message); }
    }

    public static IList<ResultType> DataReaderToList<ResultType>(IDataReader dr)
    {
        IList<ResultType> resultData = new List<ResultType>(); // выходные данные
        Type typeResultList = typeof(ResultType);

        //var isDict = typeResultList == typeof(Dictionary<string, object?>);
        var isDict = typeof(IDictionary).IsAssignableFrom(typeResultList);
        //var isDict = (typeResultList.GetGenericTypeDefinition() == typeof(Dictionary<,>));

        // Если типизированный лист
        if (!isDict)
        {
            ResultType rowData;
            Type typeColumn;

            List<string> nameColumn = new();
            for (var i = 0; i < dr.FieldCount; i++)
                nameColumn.Add(dr.GetName(i));

            while (dr.Read())
            {
                rowData = Activator.CreateInstance<ResultType>();
                foreach (PropertyInfo prop in typeResultList.GetProperties())
                {

                    typeColumn = prop.PropertyType;
                    typeColumn = Nullable.GetUnderlyingType(typeColumn) ?? typeColumn;
                    if (nameColumn.IndexOf(prop.Name) != -1)
                    {
                        prop.SetValue(rowData, Convert.IsDBNull(dr[prop.Name]) ? default(ResultType) : Convert.ChangeType(dr[prop.Name], typeColumn), null);
                    }

                }
                resultData.Add(rowData);
            }

        }
        // Если словарь
        else
        {
            //object[] row;
            while (dr.Read())
            {
                /* Dictionary<string, object?> dic = (Dictionary<string, object?>)Enumerable.Range(0, dr.FieldCount)
                                                .ToDictionary();*/
                Dictionary<string, object?> dic = new();
                for (var i = 0; i < dr.FieldCount; i++)
                {
                    try
                    {
                        dic.Add(dr.GetName(i), Convert.IsDBNull(dr.GetValue(i)) ? null : dr.GetValue(i));
                    }
                    catch (Exception e) { }
                    finally { }
                }

                resultData.Add((ResultType)(dic as object));
            }
        }

        return resultData;
    }
}
