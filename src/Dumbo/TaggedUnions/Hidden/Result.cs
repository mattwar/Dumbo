namespace Dumbo.TaggedUnion.Hidden
{
    public abstract record Result<TValue, TError>()
    {
        private record SuccessData(TValue value) : Result<TValue, TError>;
        private record FailureData(TError error) : Result<TValue, TError>;

        public static Result<TValue, TError> Success(TValue value) =>
            new SuccessData(value);

        public static Result<TValue, TError> Failure(TError error) =>
            new FailureData(error);

        public bool IsSuccess => this is SuccessData;
        public bool IsFailure => this is FailureData;

        public bool TryGetSuccess(out TValue value)
        {
            if (this is SuccessData data)
            {
                value = data.value;
                return true;
            }
            value = default!;
            return false;
        }

        public bool TryGetFailure(out TError error)
        {
            if (this is FailureData data)
            {
                error = data.error;
                return true;
            }
            error = default!;
            return false;
        }

        public static implicit operator Result<TValue, TError>(TValue value) =>
            Success(value);

        public static implicit operator Result<TValue, TError>(TError error) =>
            Failure(error);
    }
}
