using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services.TestServices
{
    public class EmployeeListDataService : IEmployeeDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;

        public EmployeeListDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
        }

        public async Task<SfListView> InitListView(SfListView listview)
        {
            var retValue = listview;

            return retValue;
        }

        public async Task<ObservableCollection<EmployeeListModel>> RetrieveEmployeeList(ObservableCollection<Models.EmployeeListModel> list, ListParam obj)
        {
            var retValue = list;
            await Task.Run(() =>
            {
                var temp = new ObservableCollection<EmployeeListModel>
                {
                  new EmployeeListModel { ProfileId = 1,  EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Cola", Position="Senior Developer" },
                  new EmployeeListModel { ProfileId = 2,  EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Cola", Position="Senior Developer" },
                  new EmployeeListModel { ProfileId = 3,  EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Cola", Position="Senior Developer" },
                  new EmployeeListModel { ProfileId = 4,  EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Cola", Position="Senior Developer" },
                  new EmployeeListModel { ProfileId = 5,  EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Cola", Position="Senior Developer" },
                  new EmployeeListModel { ProfileId = 6,  EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Algar Holiday Branch", Position="Junior Developer" },
                  new EmployeeListModel { ProfileId = 7,  EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Algar Holiday Branch", Position="Junior Developer" },
                  new EmployeeListModel { ProfileId = 8,  EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Algar Holiday Branch", Position="Junior Developer" },
                  new EmployeeListModel { ProfileId = 9,  EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Algar Holiday Branch", Position="Junior Developer" },
                  new EmployeeListModel { ProfileId = 10, EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Algar Holiday Branch", Position="Junior Developer" },
                  new EmployeeListModel { ProfileId = 11, EmployeeNo = "546135-546546-541", EmployeeName = "Basa, Kris Valenzuela", Department="Human Resource Department", Branch="Algar Holiday Branch", Position="Junior Developer" },
                };

                obj.Count = (temp.Count <= obj.Count ? temp.Count : obj.Count);

                if (temp.Count > 0)
                {
                    try
                    {
                        for (int i = obj.ListCount; i < obj.ListCount + obj.Count; i++)
                        {
                            if (temp.ElementAtOrDefault(i) != null)
                            {
                                var model = new EmployeeListModel()
                                {
                                    ProfileId = temp[i].ProfileId,
                                    EmployeeNo = temp[i].EmployeeNo,
                                    EmployeeName = temp[i].EmployeeName,
                                    Department = temp[i].Department,
                                    Branch = temp[i].Branch,
                                    Position = temp[i].Position,
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
                        //throw new Exception(ex.Message);
                    }
                }
            });

            return retValue;
        }
    }
}