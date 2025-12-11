using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class MyRequestDataService : IMyRequestDataService
    {
        public long TotalListItem { get; set; } = 10; // Mock total

        public async Task<ObservableCollection<MyRequestListModel>> RetrieveMyRequestList(ObservableCollection<MyRequestListModel> list, ListParam obj)
        {
            // Simulate API Delay
            await Task.Delay(500);

            // Return Mock Data if list is empty or paging
            if (list == null) list = new ObservableCollection<MyRequestListModel>();

            // Mock Data Generation
            // In real app, fetching from API based on obj (Page, Limit, Keyword, etc.)
            
             if (list.Count < TotalListItem)
            {
                list.Add(new MyRequestListModel 
                { 
                    TransactionId = 101, 
                    TransactionType = "Leave Request", 
                    TransactionTypeId = 1, // Leave
                    DateFiled = DateTime.Now.AddDays(-2),
                    Status = "Approved",
                    Details = "Vacation Leave (3 Days)"
                });

                list.Add(new MyRequestListModel 
                { 
                    TransactionId = 102, 
                    TransactionType = "Overtime Request", 
                    TransactionTypeId = 3, // Overtime
                    DateFiled = DateTime.Now.AddDays(-5),
                    Status = "Pending",
                    Details = "Project Deadline (2 Hours)"
                });
                
                list.Add(new MyRequestListModel 
                { 
                    TransactionId = 103, 
                    TransactionType = "Official Business", 
                    TransactionTypeId = 5, // OB
                    DateFiled = DateTime.Now.AddDays(-10),
                    Status = "Rejected",
                    Details = "Client Meeting"
                });

                 list.Add(new MyRequestListModel 
                { 
                    TransactionId = 104, 
                    TransactionType = "Change Schedule", 
                    TransactionTypeId = 6, // Change Sched
                    DateFiled = DateTime.Now.AddDays(-12),
                    Status = "Approved",
                    Details = "Shift Swap with John Doe"
                });
            }

            return list;
        }
    }
}
