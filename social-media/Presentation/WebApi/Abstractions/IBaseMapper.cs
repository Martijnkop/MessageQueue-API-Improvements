namespace Social.WebApi.Abstractions;

public interface IBaseMapper<TIn, TOut>
{
    public TOut Map(TIn input);
}
