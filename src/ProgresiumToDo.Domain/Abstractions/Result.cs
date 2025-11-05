namespace ProgresiumToDo.Domain.Abstractions;

public class Result
{
    protected internal Result(bool isSuccess, List<Error> errors)
    {
        if (isSuccess && errors.Count > 0)
            throw new InvalidOperationException("A successful result cannot have an error.");
        
        if (!isSuccess && errors.Count == 0)
            throw new InvalidOperationException("A failure result must have an error.");
        
        IsSuccess = isSuccess;
        Errors = errors;
    }
    
    public bool IsSuccess { get; }
    
    public bool IsFailure => !IsSuccess;
    
    public List<Error> Errors { get; }

    public static Result Success() => new Result(true, []);
    
    public static Result Failure(List<Error> errors) => new Result(false, errors);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, []);
    
    public static Result<TValue> Failure<TValue>(List<Error> errors) => new(default, false, errors);
    
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>([Error.NullValue]);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, List<Error> errors) : base(isSuccess, errors)
    {
        _value = value;
    }
    
    public TValue Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}