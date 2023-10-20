namespace Dumbo.TaggedUnion.Heirarchy
{
    public abstract record Result<TValue, TError>()
    {
        public record Success(TValue Value) : Result<TValue, TError>;
        public record Failure(TError Error) : Result<TValue, TError>;

        public bool IsSuccess => this is Success;
        public bool IsFailure => this is Failure;

        public static implicit operator Result<TValue, TError>(TValue value) =>
            new Success(value);

        public static implicit operator Result<TValue, TError>(TError error) =>
            new Failure(error);
    }
}
