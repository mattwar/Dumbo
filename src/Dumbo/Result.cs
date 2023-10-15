using System.Diagnostics.CodeAnalysis;

namespace Dumbo
{
    public enum ResultKind { Success, Failure };

    public struct Result<S, E>
    {
        private readonly ResultKind _kind;
        private readonly Variant _value;

        private Result(ResultKind kind, Variant value)
        {
            _kind = kind;
            _value = value;
        }

        public static Result<S, E> CreateSuccess(S value) => new Result<S, E>(ResultKind.Success, Variant.Create(value));
        public static Result<S, E> CreateFailure(E error) => new Result<S, E>(ResultKind.Failure, Variant.Create(error));

        public static implicit operator Result<S, E>(S value) => new Result<S, E>(ResultKind.Success, Variant.Create(value));
        public static implicit operator Result<S, E>(E error) => new Result<S, E>(ResultKind.Failure, Variant.Create(error));

        public ResultKind Kind => _kind;

        public bool IsSuccess => _kind == ResultKind.Success;
        public bool IsFailure => _kind == ResultKind.Failure;

        public S SuccessValue => IsSuccess ? _value.AsType<S>() : default!;
        public E FailureError => IsFailure ? _value.AsType<E>() : default!;

        public bool TryGetSuccess([NotNullWhen(true)] out S value)
        {
            if (IsSuccess)
            {
                value = _value.Get<S>()!;
                return true;
            }

            value = default!;
            return false;
        }

        public bool TryGetFailure([NotNullWhen(true)] out E error)
        {
            if (IsFailure && _value.TryGet<E>(out var f))
            {
                error = _value.Get<E>()!;
                return true;
            }

            error = default!;
            return false;
        }
    }
}
