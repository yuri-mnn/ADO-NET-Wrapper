

namespace ado_wrapper_lib;

public interface IAdoDBContext
{
    public IEnumerable<TResult>? Execute<TParam, TResult>(AdoParams<TParam> adoParams, 
                                                         TypeExecuter typeExecuter);

    public void Execute<TParam>(AdoParams<TParam> adoParams,
                                TypeExecuter typeExecuter);
}
