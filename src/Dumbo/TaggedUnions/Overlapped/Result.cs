using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TaggedUnions.Overlapped;

public struct Result<TValue, TError>
{
    private enum Kind { Success = 1, Failure };

    private readonly Kind _kind;
    private readonly TValue _value; // cannot overlap generics
    private readonly TError _error;

    private Result(Kind kind, TValue successValue, TError failureError)
    {
        _kind = kind;
        _value = successValue;
        _error = failureError;
    }

    public static Result<TValue, TError> Success(TValue value) => 
        new Result<TValue, TError>(Kind.Success, value, default!);

    public static Result<TValue, TError> Failure(TError error) =>
        new Result<TValue, TError>(Kind.Failure, default!, error);

    public bool IsSuccess => _kind == Kind.Success;
    public bool IsFailure => _kind == Kind.Failure;

    public bool TryGetSuccess([NotNullWhen(true)] out TValue value)
    {
        if (IsSuccess)
        {
            value = _value!;
            return true;
        }

        value = default!;
        return false;
    }

    public bool TryGetFailure([NotNullWhen(true)] out TError error)
    {
        if (IsFailure)
        {
            error = _error!;
            return true;
        }

        error = default!;
        return false;
    }
}
