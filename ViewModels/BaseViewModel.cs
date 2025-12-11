using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// Base class for all ViewModels implementing proper MVVM pattern
/// Provides INotifyPropertyChanged implementation and common functionality
/// All ViewModels should inherit from this class to ensure consistency
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
{
    private bool _isBusy;
    private string _busyText = "Loading...";
    private string? _errorMessage;
    private bool _disposed;

    /// <summary>
    /// Indicates if the ViewModel is currently performing an operation
    /// Bound to UI to show loading indicators
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    /// <summary>
    /// Text to display while IsBusy is true
    /// </summary>
    public string BusyText
    {
        get => _busyText;
        set => SetProperty(ref _busyText, value);
    }

    /// <summary>
    /// Current error message to display to user
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(HasError));
        }
    }

    private string? _successMessage;
    /// <summary>
    /// Current success message to display to user
    /// </summary>
    public string? SuccessMessage
    {
        get => _successMessage;
        set
        {
            SetProperty(ref _successMessage, value);
            OnPropertyChanged(nameof(Message));
        }
    }

    /// <summary>
    /// General message to display (Success or Error)
    /// </summary>
    public string? Message => !string.IsNullOrEmpty(SuccessMessage) ? SuccessMessage : ErrorMessage;

    /// <summary>
    /// Alias for IsBusy
    /// </summary>
    public bool IsLoading => IsBusy;

    /// <summary>
    /// Indicates if there is an error
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets property value and raises PropertyChanged event if value changed
    /// This is the core of MVVM data binding
    /// </summary>
    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Raises PropertyChanged event for the specified property
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Handles exceptions with proper logging and user-friendly messages
    /// </summary>
    protected virtual void HandleError(Exception ex, string userMessage = "An error occurred")
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR] {GetType().Name}: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"[STACK] {ex.StackTrace}");
        
        ErrorMessage = userMessage;
    }

    /// <summary>
    /// Clears any existing error message
    /// </summary>
    protected void ClearError()
    {
        ErrorMessage = null;
    }

    #endregion

    #region Async Helper

    /// <summary>
    /// Executes an async operation with automatic busy state management
    /// </summary>
    protected async Task ExecuteBusyAsync(Func<Task> operation, string busyText = "Loading...")
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            BusyText = busyText;
            ClearError();
            
            await operation();
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Disposes of resources used by the ViewModel
    /// Override to cleanup subscriptions, timers, etc.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Lifecycle Methods

    /// <summary>
    /// Called when the ViewModel is initialized
    /// Override to load initial data
    /// </summary>
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    #endregion
}
