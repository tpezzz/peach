
namespace Tree.Api.Map.QueryMap {
    public interface IQueryFilter<TEntity, TEntityQuery> {
        TEntity Run(TEntity entity, TEntityQuery query);
    }
}