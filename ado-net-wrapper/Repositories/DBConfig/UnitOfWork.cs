using ado_wrapper_lib;

namespace ado_net_wrapper.Repositories.DBConfig;

public class UnitOfWork
{
    private readonly IAdoDBContext _db = 
        new AdoDBContext("Server=LENOVO_PC;Database=AdventureWorks2016_EXT;Integrated Security=true;Trust Server Certificate=True;Min Pool Size=5;Connection Timeout=300;");
    private ProductRepository? _productRepository;
    public ProductRepository ProductRepository
    {
        get
        {
            if (_productRepository == null)
                _productRepository = new ProductRepository(_db);
            return _productRepository;
        }
    }
}


public interface IRepository<T> where T : class
{
    IEnumerable<T>? GetAll();
    T? Get(int id);
    void Create(T item);
    void Update(T item);
    void Delete(int id);
}