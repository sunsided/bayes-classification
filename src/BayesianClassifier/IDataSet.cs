namespace BayesianClassifier
{
    /// <summary>
    /// Interface IDataSet
    /// </summary>
    /// <typeparam name="TClass">The type of the t class.</typeparam>
    /// <typeparam name="TToken">The type of the t token.</typeparam>
    public interface IDataSet<out TClass, TToken> : IDataSetAccessor<TClass, TToken>, ITokenRegistration<TToken> 
        where TClass : IClass 
        where TToken : IToken
    {
    }
}