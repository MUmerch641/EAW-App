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
    public class MyApprovalDataService : IMyApprovalDataService
    {
        public MyApprovalDataService()
        {
        }

        public long TotalListItem { get; set; }

        public async Task<SfListView> InitListView(SfListView listview, bool isAsceding = false)
        {
            var retValue = listview;

            await Task.Run((Action)(() =>
            {
                if (retValue.DataSource.GroupDescriptors.Count == 0)
                {
                    retValue.DataSource.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "DateFiledDisplay",
                        KeySelector = (object obj1) =>
                        {
                            return (object)(obj1 as MyApprovalListModel).DateFiled.ToString("ddd MMM dd yyyy");
                        }
                    });

                    retValue.DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "DateFiled",
                        Direction = ListSortDirection.Descending
                    });
                }
            }));

            return retValue;
        }

        public async Task<ObservableCollection<MyApprovalListModel>> RetrieveApprovalList(ObservableCollection<MyApprovalListModel> list, ListParam obj)
        {
            var retValue = list;

            var temp = await InitListRecord(obj.IsAscending);
            obj.Count = (temp.Count <= obj.Count ? temp.Count : obj.Count);

            if (temp.Count > 0)
            {
                try
                {
                    for (int i = obj.ListCount; i < obj.ListCount + obj.Count; i++)
                    {
                        if (temp.ElementAtOrDefault(i) != null)
                        {
                            var model = new MyApprovalListModel()
                            {
                                TransactionId = temp[i].TransactionId,
                                TransactionType = temp[i].TransactionType,
                                DateFiled = temp[i].DateFiled,
                                Details = temp[i].Details,
                                Status = temp[i].Status,
                                RequestedDate = temp[i].RequestedDate,
                                RequestedTime = temp[i].RequestedTime,
                                RequestedHours = temp[i].RequestedHours,
                                IsVisible = temp[i].IsVisible,
                                TransactionTypeId = temp[i].TransactionTypeId,
                                EmployeeName = temp[i].EmployeeName,
                                EmployeeNo = temp[i].EmployeeNo,
                                Position = temp[i].Position,
                                Department = temp[i].Department,
                                ImageSource = temp[i].ImageSource,
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

        public async Task<ObservableCollection<MyApprovalListModel>> InitListRecord(bool isAscending = false)
        {
            var retValue = new ObservableCollection<MyApprovalListModel>
            {
                //new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 1, EmployeeName = "Kim, John A.", EmployeeNo = "ALGA-00007", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Leave, TransactionType = "Leave (VL)", DateFiled = Convert.ToDateTime("01/01/2017"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="Partially Approved", IsVisible = false },
                //new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 2, EmployeeName = "Kim, Ryeowook A.", EmployeeNo = "ALGA-00008", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Leave, TransactionType = "Leave (VL)", DateFiled = Convert.ToDateTime("02/01/2017"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 3, EmployeeName = "Dela Cruz, John C.", EmployeeNo = "003562", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Leave, TransactionType = "Leave (VL)", DateFiled = Convert.ToDateTime("03/01/2017"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 4, EmployeeName = "Cece, Thorneloe R.", EmployeeNo = "ALGA-10010", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Leave, TransactionType = "Leave (VL)", DateFiled = Convert.ToDateTime("04/01/2018"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 5, EmployeeName = "Erhard, Lias M.", EmployeeNo = "ALGA-10011", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Leave, TransactionType = "Leave (VL)", DateFiled = Convert.ToDateTime("05/01/2018"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 6, EmployeeName = "Jarrod, Wardle H.", EmployeeNo = "COMP-10012", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Leave, TransactionType = "Leave (SL)", DateFiled = Convert.ToDateTime("06/01/2018"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 7, EmployeeName = "Fredric, Thomasen L.", EmployeeNo = "ALGA-10013", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Leave, TransactionType = "Leave (SL)", DateFiled = Convert.ToDateTime("07/01/2019"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval", IsVisible = false },
                new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 8, EmployeeName = "Jock, Goadbie S.", EmployeeNo = "ALGA-10014", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Leave, TransactionType = "Leave (SL)", DateFiled = Convert.ToDateTime("08/01/2019"), Details = "Travel", RequestedDate = "02/28/2019 - 02/29/2019", RequestedHours = "16.00" , Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 9, EmployeeName = "Launce, Macvain W.", EmployeeNo = "ALGA-10015", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Undertime, TransactionType = "Undertime", DateFiled = Convert.ToDateTime("09/01/2019"), Details = "Travel", RequestedDate = "09/19/2019", RequestedHours = "4.00" , Status="For Approval", IsVisible = false },
                new MyApprovalListModel {ImageSource = "profile.jpg", TransactionId = 10, EmployeeName = "Ardath, Giberd C.", EmployeeNo = "ALGA-10016", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Undertime, TransactionType = "Undertime", DateFiled = Convert.ToDateTime("09/19/2019"), Details = "Undertime for something", RequestedDate = "09/19/2019", RequestedHours = "4.00" , Status="For Approval", IsVisible = false },

                new MyApprovalListModel {TransactionId = 10, EmployeeName = "Launce, Macvain W.", EmployeeNo = "ALGA-10015", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.TimeOff, TransactionType = "Time-Off", DateFiled = Convert.ToDateTime("07/01/2019"), Details = "Time-off for client", RequestedDate = "07/01/2019", RequestedHours = "4.00" , Status="Partially Approved", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 11, EmployeeName = "Ardath, Giberd C.", EmployeeNo = "ALGA-10016", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.TimeOff, TransactionType = "Time-Off", DateFiled = Convert.ToDateTime("07/01/2019"), Details = "Time-off for client", RequestedDate = "07/01/2019", RequestedHours = "4.00" , Status="Partially Approved", IsVisible = false },

                new MyApprovalListModel {TransactionId = 10, EmployeeName = "Launce, Macvain W.", EmployeeNo = "ALGA-10015", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.TimeLog, TransactionType = "Time Entry Log", DateFiled = Convert.ToDateTime("09/23/2019"), Details = "Time Entry Requst Remarks", RequestedDate = "09/23/2019", RequestedTime = "08:30 AM" , Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 11, EmployeeName = "Ardath, Giberd C.", EmployeeNo = "ALGA-10016", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.TimeLog, TransactionType = "Time Entry Log", DateFiled = Convert.ToDateTime("09/23/2019"), Details = "Time Entry Requst Remarks", RequestedDate = "09/23/2019", RequestedTime = "08:30 AM" , Status="For Approval", IsVisible = false },

                new MyApprovalListModel {TransactionId = 10, EmployeeName = "Launce, Macvain W.", EmployeeNo = "ALGA-10015", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Document, TransactionType = "Document", DateFiled = Convert.ToDateTime("09/25/2019"), Details = "Document Requst Remarks. Will get another car", RequestedDate = "09/25/2019",RequestedTime = "-", RequestedHours = "-",  Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 11, EmployeeName = "Ardath, Giberd C.", EmployeeNo = "ALGA-10016", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Document, TransactionType = "Document", DateFiled = Convert.ToDateTime("09/25/2019"), Details = "Document Requst Remarks. Will get another car", RequestedDate = "09/25/2019", RequestedTime = "-", RequestedHours = "-",  Status="For Approval", IsVisible = false },

                new MyApprovalListModel {TransactionId = 11, EmployeeName = "Ardath, Giberd C.", EmployeeNo = "ALGA-10016", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Loan, TransactionType = "Loan", DateFiled = Convert.ToDateTime("09/27/2019"), Details = "Reason for my loan request. Personal matters.", RequestedDate = "09/27/2019", RequestedTime = "-", RequestedHours = "-",  Status="For Approval", IsVisible = false },

                //new MyApprovalListModel {TransactionId = 12, EmployeeName = "Kim, John A.", EmployeeNo = "ALGA-00007", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("01/01/2017"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 13, EmployeeName = "Kim, Ryeowook A.", EmployeeNo = "ALGA-00008", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("02/01/2017"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 14, EmployeeName = "Dela Cruz, John C.", EmployeeNo = "003562", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("03/01/2017"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 15, EmployeeName = "Cece, Thorneloe R.", EmployeeNo = "ALGA-10010", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("04/01/2018"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 16, EmployeeName = "Erhard, Lias M.", EmployeeNo = "ALGA-10011", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("05/01/2018"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 17, EmployeeName = "Jarrod, Wardle H.", EmployeeNo = "COMP-10012", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("06/01/2018"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 18, EmployeeName = "Jock, Goadbie S.", EmployeeNo = "ALGA-10014", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("07/01/2019"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 19, EmployeeName = "Fredric, Thomasen L.", EmployeeNo = "ALGA-10013", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("08/01/2019"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 20, EmployeeName = "Launce, Macvain W.", EmployeeNo = "ALGA-10015", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("09/01/2019"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                new MyApprovalListModel {TransactionId = 21, EmployeeName = "Ardath, Giberd C.", EmployeeNo = "ALGA-10016", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.Overtime, TransactionType = "Overtime", DateFiled = Convert.ToDateTime("10/01/2019"), Details = "Backjobs and deadlines", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },

                //new MyApprovalListModel {TransactionId = 22, EmployeeName = "Cece, Thorneloe R.", EmployeeNo = "ALGA-10010", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("01/01/2017"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 23, EmployeeName = "Jock, Goadbie S.", EmployeeNo = "ALGA-10014", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("02/01/2017"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 24, EmployeeName = "Kim, John A.", EmployeeNo = "ALGA-00007", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("03/01/2017"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 25, EmployeeName = "Fredric, Thomasen L.", EmployeeNo = "ALGA-10013", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("04/01/2018"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 26, EmployeeName = "Launce, Macvain W.", EmployeeNo = "ALGA-10015", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("05/01/2018"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                new MyApprovalListModel {TransactionId = 27, EmployeeName = "Ardath, Giberd C.", EmployeeNo = "ALGA-10016", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.OfficialBusiness, TransactionType = "Official Business", DateFiled = Convert.ToDateTime("06/01/2018"), Details = "OB for meeting", RequestedDate = "02/28/2019 - 02/28/2019", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                //new MyApprovalListModel {TransactionId = 28, EmployeeName = "Ardath, Giberd C.", EmployeeNo = "ALGA-10016", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.ChangeWorkSchedule, TransactionType = "Change Work Schedule", DateFiled = Convert.ToDateTime("02/01/2017"), Details = "Change Work Shced Reason", RequestedDate = "02/28/2017 - 02/28/2017", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
                new MyApprovalListModel {TransactionId = 29, EmployeeName = "Launce, Macvain W.", EmployeeNo = "ALGA-10015", Position = "HR Associate", Department = "Customer Service", TransactionTypeId = TransactionType.ChangeWorkSchedule, TransactionType = "Change Work Schedule", DateFiled = Convert.ToDateTime("02/01/2017"), Details = "Change Work Shced Reason", RequestedDate = "02/28/2017 - 02/28/2017", RequestedTime = "6:00 PM-8:00 PM", RequestedHours = "02.00", Status="For Approval", IsVisible = false },
            };

            retValue = new ObservableCollection<MyApprovalListModel>(retValue.OrderByDescending(p => p.DateFiled));

            return retValue;
        }
    }
}