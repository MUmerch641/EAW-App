using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Services.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class MainPageDataService : IMainPageDataService
    {
        private readonly IGenericRepository _repository;

        public MainPageDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<object>> GetMenuItemsAsync()
        {
            await Task.Delay(100);
            return new List<object>
            {
                new MenuItemModel { MenuName = "Dashboard" },
                new MenuItemModel { MenuName = "Leave" },
                new MenuItemModel { MenuName = "Overtime" },
                new MenuItemModel { MenuName = "Official Business" },
                new MenuItemModel { MenuName = "Time Entry" },
                new MenuItemModel { MenuName = "Approvals" },
                new MenuItemModel { MenuName = "Attendance" },
                new MenuItemModel { MenuName = "Profile" }
            };
        }

        public Task SaveDeviceInfoAsync()
        {
            // TODO: Implement device info saving
            return Task.CompletedTask;
        }
    }
}
