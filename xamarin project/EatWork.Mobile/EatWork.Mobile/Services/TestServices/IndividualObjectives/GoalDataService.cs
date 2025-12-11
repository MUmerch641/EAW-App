using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services.TestServices
{
    public class GoalDataService : IGoalDataService
    {
        public GoalDataService()
        {
            TotalListItem = 0;
        }

        public long TotalListItem { get; set; }

        public async Task<GoalsHolder> InitForm(ObjectiveDetailHolder holder)
        {
            var response = new GoalsHolder();

            using (Acr.UserDialogs.UserDialogs.Instance.Loading())
            {
                await Task.Delay(500);
                try
                {
                    response.Header = new GoalHeaderDto()
                    {
                        HeaderId = 1,
                        GoalName = $"{holder.EffectiveYear} {holder.DepartmentName} Goals",
                        GoalDescription = $"{holder.EffectiveYear} {holder.DepartmentName} Goals",
                    };

                    response.GoalHeaderDetails = new ObservableCollection<GoalHeaderDetailDto>()
                    {
                        new GoalHeaderDetailDto()
                        {
                            HeaderId = 1,
                            HeaderDetailId = 1,
                            Name = "Expand Sales Search",
                            Description = "Expand Sales Search",
                            GoalDetails = new ObservableCollection<GoalDetailDto>()
                            {
                                new GoalDetailDto()
                                {
                                    DetailId = 1,
                                    HeaderDetailId = 1,
                                    Name = "Gain new clients",
                                    Description = "Invite and reach out of gain 10,000 more clients within a year",
                                },
                                new GoalDetailDto()
                                {
                                    DetailId = 2,
                                    HeaderDetailId = 1,
                                    Name = "Increase the number of sales call",
                                    Description = "Increase the number of sales call by 90%",
                                }
                            }
                        },
                        new GoalHeaderDetailDto()
                        {
                            HeaderId = 1,
                            HeaderDetailId = 2,
                            Name = "Increase in Overall Profit",
                            Description = "Increase in Overall Profit",
                            GoalDetails = new ObservableCollection<GoalDetailDto>()
                            {
                                new GoalDetailDto()
                                {
                                    DetailId = 3,
                                    HeaderDetailId = 2,
                                    Name = "Maintain the relationship with old clients",
                                    Description = "Maintain the number of repeat clients by 100%",
                                },
                                new GoalDetailDto()
                                {
                                    DetailId = 4,
                                    HeaderDetailId = 2,
                                    Name = "Reach 10M total premium",
                                    Description = "Reach a total amount of premium of 10M",
                                }
                            }
                        },
                    };
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return response;
        }
    }
}