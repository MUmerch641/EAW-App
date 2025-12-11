using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services.TestServices
{
    public class MyRequestDataService : IMyRequestDataService
    {
        public long TotalListItem { get; set; }

        public MyRequestDataService()
        {
        }

        public async Task<SfListView> InitListView(SfListView listview)
        {
            var retValue = listview;

            await Task.Run(() =>
            {
                if (retValue.DataSource.GroupDescriptors.Count == 0)
                {
                    retValue.DataSource.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "DateFiledDisplay",
                        KeySelector = (object obj1) =>
                        {
                            return (obj1 as MyRequestListModel).DateFiled.ToString("ddd MMM dd yyyy");
                        }
                    });

                    retValue.DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "DateFiled",
                        Direction = ListSortDirection.Descending
                    });
                }
            });

            return retValue;
        }

        public async Task<ObservableCollection<MyRequestListModel>> RetrieveMyRequestList(ObservableCollection<Models.MyRequestListModel> list, ListParam obj)
        {
            var retValue = list;

            var temp = await InitListRecord();
            obj.Count = (temp.Count <= obj.Count ? temp.Count : obj.Count);

            if (temp.Count > 0)
            {
                try
                {
                    for (int i = obj.ListCount; i < obj.ListCount + obj.Count; i++)
                    {
                        if (temp.ElementAtOrDefault(i) != null)
                        {
                            var model = new MyRequestListModel()
                            {
                                TransactionId = temp[i].TransactionId,
                                TransactionType = temp[i].TransactionType,
                                DateFiled = temp[i].DateFiled,
                                Details = temp[i].Details,
                                Status = temp[i].Status,
                                RequestedDate = temp[i].RequestedDate,
                                RequestedTime = temp[i].RequestedTime,
                                RequestedHours = temp[i].RequestedHours,
                                TransactionTypeId = temp[i].TransactionTypeId,
                                DateFiledDisplay = temp[i].DateFiled
                            };

                            retValue.Add(model);
                        }
                        else
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return retValue;
        }

        public async Task<ObservableCollection<MyRequestListModel>> InitListRecord()
        {
            var retValue = new ObservableCollection<MyRequestListModel>()
            {
                new MyRequestListModel {TransactionId = 1, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("01/01/2017"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval"},
                new MyRequestListModel {TransactionId = 2, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("02/01/2017"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="Disapproved" },
                new MyRequestListModel {TransactionId = 3, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("03/01/2017"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="Approved" },
                new MyRequestListModel {TransactionId = 4, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("04/01/2018"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="Draft" },
                new MyRequestListModel {TransactionId = 5, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("05/01/2018"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="Cancelled" },
                new MyRequestListModel {TransactionId = 6, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("06/01/2018"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="Approved" },
                new MyRequestListModel {TransactionId = 7, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("07/01/2019"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval" },
                new MyRequestListModel {TransactionId = 8, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("08/01/2019"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval" },
                new MyRequestListModel {TransactionId = 9, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("09/01/2019"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval" },
                new MyRequestListModel {TransactionId = 10, TransactionTypeId = TransactionType.Leave, TransactionType = "Leave - VL", DateFiled = Convert.ToDateTime("10/01/2019"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval" },

                new MyRequestListModel {TransactionId = 11, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("01/01/2017"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval"},
                new MyRequestListModel {TransactionId = 12, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("02/01/2017"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Disapproved" },
                new MyRequestListModel {TransactionId = 13, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("03/01/2017"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Approved" },
                new MyRequestListModel {TransactionId = 14, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("04/01/2018"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Draft" },
                new MyRequestListModel {TransactionId = 15, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("05/01/2018"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Cancelled" },
                new MyRequestListModel {TransactionId = 16, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("06/01/2018"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Approved" },
                new MyRequestListModel {TransactionId = 17, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("07/01/2019"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval" },
                new MyRequestListModel {TransactionId = 18, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("08/01/2019"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval" },
                new MyRequestListModel {TransactionId = 19, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("09/01/2019"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval" },
                new MyRequestListModel {TransactionId = 20, TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("10/01/2019"), Details = "Backjobs, testing and deadlines. Finished some tasks", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval" },

                new MyRequestListModel {TransactionId = 21, TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("01/01/2017"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval"},
                new MyRequestListModel {TransactionId = 22, TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("02/01/2017"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Disapproved" },
                new MyRequestListModel {TransactionId = 23, TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("03/01/2017"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Approved" },
                new MyRequestListModel {TransactionId = 24, TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("04/01/2018"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Draft" },
                new MyRequestListModel {TransactionId = 25, TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("05/01/2018"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Cancelled" },
                new MyRequestListModel {TransactionId = 26, TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("06/01/2018"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="Approved" },
            };

            retValue = new ObservableCollection<MyRequestListModel>(retValue.OrderByDescending(p => p.DateFiled));

            return retValue;
        }
    }
}