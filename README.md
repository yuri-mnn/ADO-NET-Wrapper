# ADO-NET-Wrapper

Для выполения запросов в БД через DI созадать экземпляр AdoWrapper:

```
services.AddTransient<IAdoDBContext>
    (s => new AdoDBContext(configuration.GetConnectionString("name_param")));
```

## Sql Procedures и Functions (Table, Scalar)

_db - экземляр IAdoDBContex внедренного через DI в класс
```
 private readonly IAdoDBContext _db;
 public Class_Name(IAdoDBContext db) =>_db = db;
```

### Выполнения запроса в БД без SQL параметров и результата
```
var adoParams = new AdoParams(name: "name_procedure");
_db.Execute(adoParams, TypeExecuter.Procedure);
```

### Выполнения запроса в БД с SQL параметрами и без результата
  
  - Параметры обьявлены как массив SqlParameter[] 
    ```
     var sqlParams = new SqlParameter[]
     {
         new SqlParameter("@ID", id_value),
         new SqlParameter("@Name", name_value)
     };
     var adoParams = new AdoParams(name: "name_procedure", dbParams: sqlParams);
     _db.Execute(adoParams, TypeExecuter.Procedure);
    ```
  - Параметры явлются объектом (class, record)
    ```
    public record User (int ID, string? Name);
    
    ...
    
    User user = new(id_value, name_value);
    var adoParams = new AdoParams(name: "name_procedure", dbParams: user);
    _db.Execute(adoParams, TypeExecuter.Procedure);
    ```
    
### Получение табличных данных из БД
 - Результат при получении сохранить в IEnumerable<Dictionary<string, object?>>
     ```
      var userInfo = _db.Execute<Dictionary<string, object?>>(adoParams, TypeExecuter.Procedure);
     ```
  - Результат при получении сохранить в IEnumerable<Class_Name>
     ```
       public record UserInfo (int Age, string Address)
       
       ...
       
       var userInfo = _db.Execute<UserInfo>(adoParams, TypeExecuter.Procedure);
     ```
     
 ### Изменения названия sql параметра или сопоставление названия столбца в БД с названием свойства
 Использовать над свойством в классе (или record) аттрибут **DBColumn** со свойствами:
 - SqlDbType - тип данных в БД
 - Name - имя sql параметра при выполнении запроса или имя столбца
 - Length - размерность типа данных
 - Mapped - использовать ли свойство при отправке запроса 
 - Output - true если SQL параметра является выходныи
 - Intput - если Output=true - параметр InputOutput, Input=false Output=true - параметр Output  
   
   Пример для record: 
   ```
   public record UserInfo (int Age, [property: DBColumn(Name = "Home_Address")] string Address)
   ```
   Пример для класса:
   ```
   public class UserInfo 
   {
      public int Age { get; set; }
        
      [DBColumn(Name = "Home_Address")]
      public int? IDPP { get; set; }
   }
   ```
 
 ### Выходные параметры
 После выполнения запроса в БД в объекте AdoParams значение выходных параметров сохраняется в св-во **OutputDBParams** (тип данных: Dictionary<string, object?>)
 
 ```
  public record User (int ID, [property: DBColumn(Output = true)] string? Name);
    
   ...
    
  User user = new(id_value, null);
  var adoParams = new AdoParams(name: "name_procedure", dbParams: user);
  _db.Execute(adoParams, TypeExecuter.Procedure);
  
  // Получение значения выходного параметра
  object outputParam =  adoParams.OutputDBParams["@Name"];
 ```
 
 ### Выполнение нескольких запросов в одном подключении к БД
 ```
  using SqlConnection connection = new(_db.ConnectionString);
  connection.Open();
 
  // 1 запрос
  var adoParams1 = new AdoParams(name: "name_proc_1", connection: connection);
  _db.Execute(adoParams1, TypeExecuter.Procedure);

  // 2 запрос
  var adoParams2 = new AdoParams(name: "name_proc_2", connection: connection);
  _db.Execute(adoParams2, TypeExecuter.Procedure);
 ```
 
 ### Выполнения нескольких запросов в транзакции
 ```
   using SqlConnection connection = new(_db.ConnectionString);
   transaction = null;
   connection.Open();
   try
   {
     SqlTransaction transaction = connection.BeginTransaction();
     
     // 1 запрос
     var adoParams1 = new AdoParams(name: "name_proc_1", connection: connection, transaction: transaction);
     _db.Execute(adoParams1, TypeExecuter.Procedure);

     // 2 запрос
     var adoParams2 = new AdoParams(name: "name_proc_2", connection: connection, transaction: transaction);
     _db.Execute(adoParams2, TypeExecuter.Procedure);
    
     transaction.Commit();
    
   catch (Exception ex)
   {
           
      transaction?.Rollback();
      throw new Exception(ex.Message);
   }
 ```
 ### Получение скалярного значения из БД 
 **Для sql функции** - если есть параметры при выполнении, то порядок параметров в sql скрипте и классе должен совпадать
 ```
  Query query = new(id);    
  var adoParams = new AdoParams(name: "function_name", dbParams: query);
  var result = _db.ExecuteScalar<string>(adoParams, TypeExecuter.Function));
 ```
 
 ## SQL запросы
 Примечаниe: 
 -  TypeExecuter = Query
 -  dbParams не поддерживаются
 -  в AdoParams свойтва Name указать запрос: Name="select * from table_name";
