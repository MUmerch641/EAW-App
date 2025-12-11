using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services
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

            try
            {
                using (Acr.UserDialogs.UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);

                    if (holder != null)
                    {
                        response.Header = new GoalHeaderDto()
                        {
                            GoalName = $"{holder.EffectiveYear} {holder.DepartmentName} Goals",
                            GoalDescription = $"{holder.EffectiveYear} {holder.DepartmentName} Goals",
                        };

                        response.ParentObjectiveList = holder.ParentObjectiveList;
                        response.OtherObjectiveList = holder.OtherObjectiveList;

                        if (holder.ParentObjectiveList.Count > 0)
                        {
                            response.GoalHeaderDetails = new ObservableCollection<GoalHeaderDetailDto>(
                                holder.ParentObjectiveList.Select(p => new GoalHeaderDetailDto()
                                {
                                    Name = p.Header.OrgGoal,
                                    Description = p.Header.Description,
                                    GoalDetails = new ObservableCollection<GoalDetailDto>(
                                            p.Detail.Select(x => new GoalDetailDto()
                                            {
                                                Description = x.Description,
                                                DetailId = x.OrganizationGoalId,
                                                Name = x.OrgGoal,
                                                HeaderDetailName = x.ParentGoal,
                                            })
                                        ),
                                }));
                        }

                        if (holder.OtherObjectiveList.Count > 0)
                        {
                            response.GoalHeaderDetails.Add(new GoalHeaderDetailDto()
                            {
                                Name = Constants.OrgGoalsConstant,
                                Description = Constants.OrgGoalsConstant,
                                GoalDetails = new ObservableCollection<GoalDetailDto>(
                                    holder.OtherObjectiveList.Select(p => new GoalDetailDto()
                                    {
                                        DetailId = p.OrganizationGoalId,
                                        Name = p.OrgGoal,
                                        Description = p.Description,
                                        HeaderDetailName = Constants.OrgGoalsConstant,
                                    })
                                )
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }
    }
}