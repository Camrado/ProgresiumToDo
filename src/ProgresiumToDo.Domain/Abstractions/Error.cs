namespace ProgresiumToDo.Domain.Abstractions;

public record Error(string Code, string Message)
{
    public static Error None = new Error(string.Empty, string.Empty);
    
    public static Error NullValue = new Error("Error.NullValue", "The value cannot be null.");
}