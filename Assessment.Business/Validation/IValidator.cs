namespace Assessment.Business.Validation
{
    public interface IValidator<out T, in TK> where T : class where TK : class
    {
        T ValidateRequest(TK request);
    }
}
