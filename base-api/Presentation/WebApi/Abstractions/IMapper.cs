namespace POS.WebApi.Abstractions;

public interface IMapper<TIn1, TOut1, TIn2, TOut2, TIn3, TOut3> : IBaseMapper<TIn1, TOut1>
{
    public TOut2 Map(TIn2 input);
    public TOut3 Map(TIn3 input);
}
