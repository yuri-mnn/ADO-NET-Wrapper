using AdoWrapper.Attributes;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Reflection;

namespace AdoWrapper.Converters;

public class ObjectConverter
{
    public static SqlParameter[] ObjectToArraySQLParameters<T>(T obj)
    {
        IList<SqlParameter> sqlParams = new List<SqlParameter>();
        SqlParameter sqlParam;
        DBColumnAttribute? dBColumnAttribute;

        try
        {
            if (obj is null)
                return sqlParams.ToArray();

            Type typeGeneric = (obj ?? throw new NullReferenceException("Передан пустая ссылка на объект") ).GetType();
            
            if (typeGeneric.IsPrimitive)
               throw new InvalidCastException("Невозможно преобразовать примитивный тип в массив SQLParameters");
           
            foreach (PropertyInfo prop in typeGeneric.GetProperties())
            {
                dBColumnAttribute = prop.GetCustomAttribute<DBColumnAttribute>(true);

                if (dBColumnAttribute?.Mapped ?? true)
                {
                    sqlParam = new();

                    // Установка имени параметра
                    sqlParam.ParameterName = "@" + (dBColumnAttribute?.Name ?? prop.Name);

                    // Установка значения параметра
                    sqlParam.Value = obj?.GetType().GetProperty(prop.Name)?.GetValue(obj, null) ?? Convert.DBNull;

                    // Установка типа данных sql параметра
                    if (dBColumnAttribute?.Type is not null) 
                        sqlParam.SqlDbType = dBColumnAttribute.Type!;
                    
                    // Установка размергости sql параметра
                    if (dBColumnAttribute?.Lenght is not null) 
                       sqlParam.Size = dBColumnAttribute.Lenght!;
                    
                    // Утстановка типа параметра(input, output, input-output)
                    if (dBColumnAttribute?.Input == true && dBColumnAttribute.Output == true)
                      sqlParam.Direction = ParameterDirection.InputOutput;
                    else if (dBColumnAttribute?.Input == false && dBColumnAttribute.Output == true)
                      sqlParam.Direction = ParameterDirection.Output;
                    else 
                      sqlParam.Direction = ParameterDirection.Input;

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

            // Список названий колонок в результирующей таблице
            List<string> nameColumn = new();
            for (var i = 0; i < dr.FieldCount; i++)
                nameColumn.Add(dr.GetName(i));

            // Атрибут свойства
            DBColumnAttribute? dBColumnAttribute;
            string nameProp;

            // Чтение данных из БД
            while (dr.Read())
            {
                // Созлдание модели
                rowData = Activator.CreateInstance<ResultType>();

                // Заполнение свойств
                foreach (PropertyInfo prop in typeResultList.GetProperties())
                {
                    // Атрибут над свойством в модели
                    dBColumnAttribute = prop.GetCustomAttribute<DBColumnAttribute>(true);
                    
                    // Тип данных свойсва
                    typeColumn = prop.PropertyType;
                    typeColumn = Nullable.GetUnderlyingType(typeColumn) ?? typeColumn;

                    // Поиск в записи из БД колонки по атрибуту модели
                    nameProp = dBColumnAttribute?.Name ?? prop.Name;
                    if (nameColumn.IndexOf(nameProp) != -1)
                    {
                        prop.SetValue(rowData, Convert.IsDBNull(dr[nameProp]) ? default(ResultType) : Convert.ChangeType(dr[nameProp], typeColumn), null);
                    }

                }
                resultData.Add(rowData);
            }

        }
        // Если словарь
        else
        {
            Dictionary<string, object?> resultDictionary;
            while (dr.Read())
            {
                /* Dictionary<string, object?> dic = (Dictionary<string, object?>)Enumerable.Range(0, dr.FieldCount)
                                                .ToDictionary();*/
                resultDictionary = new();
                for (var i = 0; i < dr.FieldCount; i++)
                {
                    try
                    {
                        resultDictionary.Add(dr.GetName(i), Convert.IsDBNull(dr.GetValue(i)) ? null : dr.GetValue(i));
                    }
                    catch { }
                }

                resultData.Add((ResultType)(resultDictionary as object));
            }
        }

        return resultData;
    }
}
