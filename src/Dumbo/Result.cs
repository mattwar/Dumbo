using System.Diagnostics.CodeAnalysis;

namespace Dumbo
{
    public enum ResultKind { Success, Failure };

    public struct Result<TValue, TError>
    {
        private readonly ResultKind _kind;
        private readonly Variant _value;

        private Result(ResultKind kind, Variant value)
        {
            _kind = kind;
            _value = value;
        }

        public static Result<TValue, TError> Success(TValue value) => new Result<TValue, TError>(ResultKind.Success, Variant.Create(value));
        public static Result<TValue, TError> Failure(TError error) => new Result<TValue, TError>(ResultKind.Failure, Variant.Create(error));

        public static implicit operator Result<TValue, TError>(TValue value) => new Result<TValue, TError>(ResultKind.Success, Variant.Create(value));
        public static implicit operator Result<TValue, TError>(TError error) => new Result<TValue, TError>(ResultKind.Failure, Variant.Create(error));

        public ResultKind Kind => _kind;

        public bool IsSuccess => _kind == ResultKind.Success;
        public bool IsFailure => _kind == ResultKind.Failure;

        public TValue SuccessValue => IsSuccess ? _value.AsType<TValue>() : default!;
        public TError FailureError => IsFailure ? _value.AsType<TError>() : default!;

        public bool TryGetSuccess([NotNullWhen(true)] out TValue value)
        {
            if (IsSuccess)
            {
                value = _value.Get<TValue>()!;
                return true;
            }

            value = default!;
            return false;
        }

        public bool TryGetFailure([NotNullWhen(true)] out TError error)
        {
            if (IsFailure && _value.TryGet<TError>(out var f))
            {
                error = _value.Get<TError>()!;
                return true;
            }

            error = default!;
            return false;
        }
    }
}
