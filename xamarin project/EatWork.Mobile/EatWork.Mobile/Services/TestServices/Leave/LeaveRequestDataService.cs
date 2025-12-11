using Acr.UserDialogs;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class LeaveRequestDataService : ILeaveRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly StringHelper string_;

        public LeaveRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            StringHelper url)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            string_ = url;
        }

        public async Task<LeaveRequestHolder> InitFormHelpers(long recordId)
        {
            var retValue = new LeaveRequestHolder()
            {
                ApplyTo = new ObservableCollection<ComboBoxObject>(),
                ApplyToSelectedItem = new ComboBoxObject(),
                LeaveType = new ObservableCollection<SelectableListModel>(),
                LeaveTypeSelectedItem = new SelectableListModel(),
                ShowPartialOptions = false,
                LeaveRequestModel = new LeaveRequestModel()
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            try
            {
                #region leave types

                var leaveTypeUrl = new UriBuilder(url)
                {
                    Path = ApiConstants.GetLeaveTypes
                };

                var leaveTypeParam = new R.Requests.LeaveTypeSetupRequest()
                {
                    CompanyId = userInfo.CompanyId
                };

                var requestUrl = string_.CreateUrl<R.Requests.LeaveTypeSetupRequest>(leaveTypeUrl.ToString(), leaveTypeParam);

                var leaveTypes = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.LeaveTypeSetup>>(requestUrl.ToString());

                if (leaveTypes.ListData.Count > 0)
                {
                    foreach (var item in leaveTypes.ListData)
                        retValue.LeaveType.Add(new SelectableListModel() { Id = item.LeaveTypeSetupId, DisplayText = item.Code });
                }

                #endregion leave types

                #region apply to

                var enumUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{EnumValues.LeaveRequestApplyTo}"
                };

                var applyTo = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                if (applyTo.Count > 0)
                {
                    foreach (var item in applyTo)
                        retValue.ApplyTo.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });
                }

                #endregion apply to

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetLeaveRequestDetail}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.LeaveRequestDetailResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        #region get leave type

                        leaveTypeUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetLeaveTypeById}{response.LeaveRequestHeader.LeaveTypeId}"
                        };

                        var leaveType = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.LeaveTypeSetup>>(leaveTypeUrl.ToString());

                        retValue.LeaveTypeSelectedItem = new SelectableListModel()
                        {
                            DisplayText = leaveType.Model.Code,
                            Id = leaveType.Model.LeaveTypeSetupId
                        };

                        #endregion get leave type

                        retValue.LeaveRequestModel = new LeaveRequestModel()
                        {
                            DateFiled = response.LeaveRequestHeader.CreateDate,
                            InclusiveStartDate = response.LeaveRequestHeader.InclusiveStartDate,
                            InclusiveEndDate = response.LeaveRequestHeader.InclusiveEndDate,
                            LeaveTypeId = response.LeaveRequestHeader.LeaveTypeId,
                            LeaveRequestHeaderId = response.LeaveRequestHeader.LeaveRequestHeaderId,
                            PartialDayApplyTo = response.LeaveRequestHeader.PartialDayApplyTo,
                            PartialDayLeave = response.LeaveRequestHeader.PartialDayLeave,
                            StatusId = response.LeaveRequestHeader.StatusId,
                            Reason = response.LeaveRequestHeader.Reason,
                            RemainingHours = response.LeaveRequestHeader.RemainingHours,
                            ProfileId = response.LeaveRequestHeader.ProfileId,
                        };

                        //PropertyCopier<R.Models.LeaveRequestHeader, LeaveRequestModel>.Copy(response.LeaveRequestHeader, retValue.LeaveRequestModel);

                        var detail = response.LeaveRequestHeader.LeaveRequest.FirstOrDefault();
                        if (detail != null)
                        {
                            retValue.Planned = Convert.ToBoolean(detail.Planned);
                            retValue.LeaveRequestModel.NoOfHours = detail.NoOfHours;
                        }

                        retValue.IsPartialLeave = (short)response.LeaveRequestHeader.PartialDayLeave;
                        retValue.ShowPartialOptions = Convert.ToBoolean(response.LeaveRequestHeader.PartialDayLeave);
                    }

                    //retValue.IsEnabled = (retValue.LeaveRequestModel.StatusId == RequestStatusValue.Draft);
                    retValue.IsEnabled = (retValue.LeaveRequestModel.StatusId == RequestStatusValue.Submitted ||
                                          retValue.LeaveRequestModel.StatusId == RequestStatusValue.Draft);
                    retValue.ShowCancelButton = (retValue.LeaveRequestModel.StatusId == RequestStatusValue.ForApproval);
                }
                else
                {
                    retValue.LeaveRequestModel = new LeaveRequestModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        CompanyId = userInfo.CompanyId
                    };

                    retValue.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<LeaveRequestHolder> GetLeaveBalance(LeaveRequestHolder form)
        {
            var retValue = form;
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            if (form.LeaveTypeSelectedItem.Id > 0)
            {
                using (UserDialogs.Instance.Loading())
                {
                    try
                    {
                        await Task.Delay(500);

                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetLeaveRequestBalance}{form.LeaveRequestModel.ProfileId}/{form.LeaveTypeSelectedItem.Id}/{DateTime.Now:yyyy-MM-dd}"
                        };

                        var response = await genericRepository_.GetAsync<R.Responses.LeaveRequestBalanceResponse>(builder.ToString());

                        if (response.IsSuccess)
                        {
                            retValue.LeaveRequestModel.RemainingHours = Convert.ToDecimal(response.BalanceHours);
                        }

                        retValue.LeaveRequestModel.Reason = form.LeaveTypeSelectedItem.DisplayText;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            return retValue;
        }

        public async Task<LeaveRequestHolder> SubmitRequest(LeaveRequestHolder form)
        {
            var retValue = form;
            var errors = new List<int>();
            retValue.ErrorLeaveType = false;
            retValue.ErrorPartialType = false;

            if (form.LeaveTypeSelectedItem.Id == 0)
            {
                retValue.ErrorLeaveType = true;
                errors.Add(1);
            }

            if (retValue.ShowPartialOptions && retValue.ApplyToSelectedItem.Id == 0)
            {
                retValue.ErrorPartialType = true;
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))
                {
                    try
                    {
                        using (UserDialogs.Instance.Loading())
                        {
                            await Task.Delay(500);
                            retValue.LeaveRequestModel.InclusiveStartDate = form.InclusiveStartDate;
                            retValue.LeaveRequestModel.InclusiveEndDate = form.InclusiveEndDate;
                            retValue.LeaveRequestModel.LeaveTypeId = form.LeaveTypeSelectedItem.Id;
                            retValue.LeaveRequestModel.PartialDayApplyTo = (short)form.ApplyToSelectedItem.Id;
                            retValue.LeaveRequestModel.Planned = (short)(form.Planned ? 1 : 0);

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitLeaveRequestRequest
                            };

                            var data = new R.Models.LeaveRequest();
                            PropertyCopier<LeaveRequestModel, R.Models.LeaveRequest>.Copy(form.LeaveRequestModel, data);

                            var param = new R.Requests.SubmitLeaveRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitLeaveRequest, R.Responses.BaseResponse<R.Models.LeaveRequest>>(builder.ToString(), param);

                            if (response.Model.LeaveRequestHeaderId > 0)
                            {
                                retValue.Success = true;
                                FormSession.IsSubmitted = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            return retValue;
        }

        public async Task<LeaveApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new LeaveApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetLeaveRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.LeaveRequestDetailResponse>(builder.ToString());

                if (response.IsSuccess)
                {
                    #region get leave type

                    var leaveTypeUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetLeaveTypeById}{response.LeaveRequestHeader.LeaveTypeId}"
                    };

                    var leaveType = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.LeaveTypeSetup>>(leaveTypeUrl.ToString());

                    #endregion get leave type

                    #region get enum value

                    if (response.LeaveRequestHeader.PartialDayApplyTo != 0)
                    {
                        var enumUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.LeaveRequestApplyTo}/{response.LeaveRequestHeader.PartialDayApplyTo}"
                        };

                        var enumval = await genericRepository_.GetAsync<R.Models.Enums>(enumUrl.ToString());

                        retValue.ApplyTo = enumval.DisplayText;
                    }

                    #endregion get enum value

                    var detail = response.LeaveRequestHeader.LeaveRequest.FirstOrDefault();

                    retValue.LeaveRequestModel = new LeaveRequestModel();

                    PropertyCopier<R.Models.LeaveRequestHeader, LeaveRequestModel>.Copy(response.LeaveRequestHeader, retValue.LeaveRequestModel);
                    PropertyCopier<R.Models.LeaveRequestHeader, LeaveApprovalHolder>.Copy(response.LeaveRequestHeader, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.ShowPartialApplyTo = (response.LeaveRequestHeader.PartialDayApplyTo == 1 ? true : false);
                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;

                    retValue.DateFiled = Convert.ToDateTime(response.LeaveRequestHeader.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.LeaveType = leaveType.Model.Code;
                    retValue.RemainingBalance = string.Format("{0:0.00} hrs", response.LeaveRequestHeader.RemainingHours);

                    retValue.InclusiveDate = string.Empty.ConcatDate(Convert.ToDateTime(response.LeaveRequestHeader.InclusiveStartDate),
                        Convert.ToDateTime(response.LeaveRequestHeader.InclusiveEndDate));

                    retValue.TotalRequestedHours = string.Format("{0:0.00} hrs{1}", response.LeaveRequestHeader.TotalNoOfHours,
                        (response.LeaveRequestHeader.LeaveRequest.Count > 1 ? string.Format(" ({0:00} hrs per day)", detail.NoOfHours) : ""));

                    retValue.IsPartialDay = (response.LeaveRequestHeader.PartialDayLeave == 1 ? "Yes" : "No");

                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    retValue.ShowPartialApplyTo = Convert.ToBoolean(response.LeaveRequestHeader.PartialDayLeave);

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }
                }
                else
                    throw new ArgumentException(response.ErrorMessage);

                retValue.IsSuccess = response.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<ObservableCollection<LeaveUsageListHolder>> InitLeaveUsage(long profileId, long leaveTypeId)
        {
            var retValue = new ObservableCollection<LeaveUsageListHolder>();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    //Path = $"{ApiConstants.GetLeaveUsage}{profileId}/{leaveTypeId}/{ApiConstants.LeaveUsageConstant}"
                    Path = string.Format(ApiConstants.GetLeaveUsage, profileId, leaveTypeId)
                };

                var response = await genericRepository_.GetAsync<R.Responses.LeaveUsageResponse>(builder.ToString());

                if (response.LeaveUsageList.Count > 0)
                {
                    Parallel.ForEach(response.LeaveUsageList, item =>
                    {
                        retValue.Add(new LeaveUsageListHolder()
                        {
                            LeaveTypeSetup = item.LeaveTypeSetup,
                            DateFiled = item.DateFiled,
                            InclusiveDate = item.InclusiveDate,
                            LeaveRequestDisplay = item.LeaveRequestDisplay,
                            Reason = item.Reason,
                            Status = item.Status,
                            NoOfHours = item.NoOfHours,
                            DisplayInDays = item.DisplayInDays,
                            NoOfHoursPerDay = item.NoOfHoursPerDay
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<LeaveApprovalHolder> WorkflowTransaction(LeaveApprovalHolder form)
        {
            var retValue = form;
            retValue.IsSuccess = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.SelectedWorkflowAction.ActionTriggeredId, form.SelectedWorkflowAction.ActionMessage);
                if (confirm.Continue)
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);

                        await commonDataService_.HasInternetConnection(await commonDataService_.RetrieveClientUrl());

                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.TransactionId, form.TransactionTypeId, form.SelectedWorkflowAction.ActionTriggeredId, confirm.ResponseText, form.StageId);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            retValue.IsSuccess = response.IsSuccess;
                            FormSession.MyApprovalSelectedItemUpdated = response.IsSuccess;
                            FormSession.MyApprovalSelectedItemStatus = FormSession.SetDefaultMyApprovalSelectedItemStatus(form.SelectedWorkflowAction.ActionType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<LeaveRequestHolder> WorkflowTransactionRequest(LeaveRequestHolder form)
        {
            var retValue = form;
            retValue.Success = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.ActionTypeId, form.Msg);
                if (confirm.Continue)
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(Convert.ToInt64(form.LeaveRequestModel.LeaveRequestHeaderId), TransactionType.Leave);
                        var response = await workflowDataService_.ProcessWorkflowByRecordId(Convert.ToInt64(form.LeaveRequestModel.LeaveRequestHeaderId),
                                             TransactionType.Leave,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            FormSession.IsSubmitted = response.IsSuccess;
                            retValue.Success = response.IsSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<LeaveRequestHolder> InitLeaveRequestForm(long recordId, DateTime? selectedDate)
        {
            throw new NotImplementedException();
        }

        public async Task<LeaveRequestHolder> ValidateLeaveRequestGeneration(LeaveRequestHolder form)
        {
            throw new NotImplementedException();
        }

        public async Task<LeaveRequestHolder> SubmitRequestEngine(LeaveRequestHolder form)
        {
            throw new NotImplementedException();
        }

        public async Task<LeaveRequestHolder> GenerateAndSubmitRequest(LeaveRequestHolder form)
        {
            throw new NotImplementedException();
        }
    }
}