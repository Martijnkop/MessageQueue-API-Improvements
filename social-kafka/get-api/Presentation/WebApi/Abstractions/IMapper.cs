namespace Social.WebApi.Abstractions;

public interface IMapper<TIn, TOut>
{
    public TOut Map(TIn input);
}
