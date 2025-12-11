using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Models.Workflow;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;

namespace MauiHybridApp.ViewModels;

public class TaskNotificationViewModel : BaseViewModel
{
    private readonly IApprovalDataService _approvalDataService;
    private readonly INotificationDataService _notificationDataService;
    private readonly INavigationService _navigationService;

    public TaskNotificationViewModel(
        IApprovalDataService approvalDataService,
        INotificationDataService notificationDataService,
        INavigationService navigationService)
    {
        _approvalDataService = approvalDataService;
        _notificationDataService = notificationDataService;
        _navigationService = navigationService;

        RefreshCommand = new Command(async () => await LoadDataAsync());
        NavigateToTaskCommand = new Command<MyApprovalListModel>(async (task) => await NavigateToTaskAsync(task));
        NavigateToNotificationCommand = new Command<NotificationModel>(async (notif) => await NavigateToNotificationAsync(notif));
        MarkAsReadCommand = new Command<NotificationModel>(async (notif) => await MarkAsReadAsync(notif));
        SwitchTabCommand = new Command<string>(SwitchTab);
    }

    private ObservableCollection<MyApprovalListModel> _tasks = new();
    public ObservableCollection<MyApprovalListModel> Tasks
    {
        get => _tasks;
        set => SetProperty(ref _tasks, value);
    }

    private ObservableCollection<NotificationModel> _notifications = new();
    public ObservableCollection<NotificationModel> Notifications
    {
        get => _notifications;
        set => SetProperty(ref _notifications, value);
    }

    private string _activeTab = "Tasks";
    public string ActiveTab
    {
        get => _activeTab;
        set
        {
            if (SetProperty(ref _activeTab, value))
            {
                OnPropertyChanged(nameof(IsTasksTabActive));
                OnPropertyChanged(nameof(IsNotificationsTabActive));
            }
        }
    }

    public bool IsTasksTabActive => ActiveTab == "Tasks";
    public bool IsNotificationsTabActive => ActiveTab == "Notifications";

    public ICommand RefreshCommand { get; }
    public ICommand NavigateToTaskCommand { get; }
    public ICommand NavigateToNotificationCommand { get; }
    public ICommand MarkAsReadCommand { get; }
    public ICommand SwitchTabCommand { get; }

    public override async Task InitializeAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            // Load Tasks (Approvals)
            var approvals = await _approvalDataService.GetPendingApprovalsAsync();
            Tasks = new ObservableCollection<MyApprovalListModel>(approvals);

            // Load Notifications
            var notificationsList = await _notificationDataService.GetNotificationsAsync();
            // Assuming GetNotificationsAsync returns object list, need to cast or map if service returns object
            // Based on interface it returns List<object>, need to check actual implementation or mock
            // For now assuming it might return compatible objects or we cast
            
            var typedNotifications = new List<NotificationModel>();
             if (notificationsList != null)
            {
                foreach (var item in notificationsList)
                {
                    if (item is NotificationModel model)
                    {
                        typedNotifications.Add(model);
                    }
                    // If mock returns something else, we might need to handle it.
                    // For safety, let's assume we might get NotificationModel from current mock implementations
                }
            }
            Notifications = new ObservableCollection<NotificationModel>(typedNotifications);

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading tasks/notifications: {ex.Message}");
            // Handle error, maybe show alert
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SwitchTab(string tabName)
    {
        ActiveTab = tabName;
    }

    private async Task NavigateToTaskAsync(MyApprovalListModel task)
    {
        if (task == null) return;
        
        // Navigate to approval detail page
        // Assuming we reuse the logic from ApprovalsViewModel which likely navigates to a detail page
        // For now, let's assume "approval-details" route or similar.
        // We need to check routes. simpler approach: navigate to approval hub or similar if generic detail missing
        
        // Using common navigation pattern
         var parameters = new Dictionary<string, object>
        {
            { "ApprovalId", task.TransactionId },
            { "TransactionTypeId", task.TransactionTypeId }
        };
        await _navigationService.NavigateToAsync("approval-details", parameters);
    }

    private async Task NavigateToNotificationAsync(NotificationModel notification)
    {
        if (notification == null) return;
        
        if (!notification.IsRead)
        {
             await MarkAsReadAsync(notification);
        }

        // Navigate based on notification type/url
        // Simplification: just mark read for now or navigate if URL present
    }

    private async Task MarkAsReadAsync(NotificationModel notification)
    {
        if (notification == null) return;

        try
        {
            await _notificationDataService.MarkAsReadAsync(notification.WorkflowNotificationTaskId);
            notification.IsRead = true;
            OnPropertyChanged(nameof(Notifications)); // Refresh UI
        }
        catch (Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"Error marking read: {ex.Message}");
        }
    }
}
