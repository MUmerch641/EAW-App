namespace MauiHybridApp.Models;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public T Model { get; set; }
    public string Message { get; set; }
}
