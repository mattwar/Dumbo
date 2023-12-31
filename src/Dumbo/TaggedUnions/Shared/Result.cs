﻿using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TaggedUnions.Shared;

public struct Result<TValue, TError>
{
    private enum Kind { Success = 1, Failure };

    private readonly Kind _kind;
    private readonly TValue _value;  // cannot share generic fields
    private readonly TError _error;

    private Result(Kind kind, TValue value, TError error)
    {
        _kind = kind;
        _value = value;
        _error = error;
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

    public static implicit operator Result<TValue, TError>(TValue value) => 
        Success(value);

    public static implicit operator Result<TValue, TError>(TError error) => 
        Failure(error);
}
