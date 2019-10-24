
namespace Tree.Api.Map.CommandMap {
    public interface ICommandMapper<TMapFrom, TMapTo> {
        TMapTo Map(TMapFrom command);
    }
}