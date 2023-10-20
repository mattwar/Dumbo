using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TaggedUnion.Hybrid;

public struct Result<TValue, TError>
{
    private enum Kind { Success = 1, Failure };

    private readonly Kind _kind;
    private readonly Variant _value;

    private Result(Kind kind, Variant value)
    {
        _kind = kind;
        _value = value;
    }

    public static Result<TValue, TError> Success(TValue value) => 
        new Result<TValue, TError>(Kind.Success, Variant.Create(value));

    public static Result<TValue, TError> Failure(TError error) => 
        new Result<TValue, TError>(Kind.Failure, Variant.Create(error));

    public bool IsSuccess => _kind == Kind.Success;
    public bool IsFailure => _kind == Kind.Failure;

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

    public static implicit operator Result<TValue, TError>(TValue value) =>
        Success(value);

    public static implicit operator Result<TValue, TError>(TError error) =>
        Failure(error);
}
