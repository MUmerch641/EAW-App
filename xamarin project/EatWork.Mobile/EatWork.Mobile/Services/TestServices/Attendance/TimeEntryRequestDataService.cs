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
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class TimeEntryRequestDataService : ITimeEntryRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;

        public TimeEntryRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
        }

        public async Task<TimeEntryHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new TimeEntryHolder();

            var userInfo = PreferenceHelper.UserInfo();

            if (recordId > 0)
            {
                await Task.Delay(500);
                using (UserDialogs.Instance.Loading())
                {
                    var url = await commonDataService_.RetrieveClientUrl();

                    await commonDataService_.HasInternetConnection(url);

                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetTimeEntryRequestDetail}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.TimeEntryLogsResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        retValue.TimeEntryLogModel = new TimeEntryLogModel();
                        PropertyCopier<R.Models.TimeEntryLog, TimeEntryLogModel>.Copy(response.TimeEntryLog, retValue.TimeEntryLogModel);
                        PropertyCopier<R.Models.TimeEntryLog, TimeEntryHolder>.Copy(response.TimeEntryLog, retValue);

                        retValue.LogTime = Convert.ToDateTime(retValue.TimeEntryLogModel.TimeEntry).TimeOfDay;

                        switch (retValue.TimeEntryLogModel.Type)
                        {
                            case TimeEntryTypeValue.TimeIn:
                                retValue.TimeInChecked = true;
                                break;

                            case TimeEntryTypeValue.TimeOut:
                                retValue.TimeOutChecked = true;
                                break;

                            case TimeEntryTypeValue.BreakIn:
                                retValue.BreakInChecked = true;
                                break;

                            case TimeEntryTypeValue.BreakOut:
                                retValue.BreakOutChecked = true;
                                break;

                            default:
                                break;
                        }

                        retValue.IsEnabled = (retValue.TimeEntryLogModel.StatusId == RequestStatusValue.Draft);
                        retValue.ShowCancelButton = (retValue.TimeEntryLogModel.StatusId == RequestStatusValue.ForApproval);
                    }
                    else
                        throw new Exception(response.ErrorMessage);
                }
            }
            else
            {
                retValue.TimeEntryLogModel = new TimeEntryLogModel
                {
                    //CreateId = userInfo.UserSecurityId,
                    ProfileId = userInfo.ProfileId,
                };

                retValue.IsEnabled = true;
            }

            return retValue;
        }

        public async Task<TimeEntryHolder> SubmitRequest(TimeEntryHolder form)
        {
            var retValue = form;

            retValue.ErrorTimeEntryDate = false;
            retValue.ErrorTimeEntryTime = false;
            retValue.ErrorRemark = false;

            if (string.IsNullOrWhiteSpace(retValue.TimeEntryLogModel.Remark))
                retValue.ErrorRemark = true;

            if (!retValue.ErrorTimeEntryDate && !retValue.ErrorTimeEntryTime && !retValue.ErrorRemark)
            {
                if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        try
                        {
                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            form.TimeEntryLogModel.TimeEntry = Convert.ToDateTime(form.LogDate).Date + form.LogTime;

                            if (form.TimeInChecked)
                                form.TimeEntryLogModel.Type = TimeEntryTypeValue.TimeIn;

                            if (form.TimeOutChecked)
                                form.TimeEntryLogModel.Type = TimeEntryTypeValue.TimeOut;

                            if (form.BreakInChecked)
                                form.TimeEntryLogModel.Type = TimeEntryTypeValue.BreakIn;

                            if (form.BreakOutChecked)
                                form.TimeEntryLogModel.Type = TimeEntryTypeValue.BreakOut;

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitTimeEntryRequest
                            };

                            var data = new R.Models.TimeEntryLog();
                            PropertyCopier<TimeEntryLogModel, R.Models.TimeEntryLog>.Copy(form.TimeEntryLogModel, data);

                            var param = new R.Requests.SubmitTimeEntryLogRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitTimeEntryLogRequest, R.Responses.BaseResponse<R.Models.TimeEntryLog>>(builder.ToString(), param);

                            if (response.Model.TimeEntryLogId > 0)
                            {
                                retValue.Success = true;

                                FormSession.IsSubmitted = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }

            return retValue;
        }

        public async Task<TimeEntryHolder> WorkflowTransactionRequest(TimeEntryHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.TimeEntryLogModel.TimeEntryLogId, TransactionType.TimeLog);
                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.TimeEntryLogModel.TimeEntryLogId,
                                             TransactionType.TimeLog,
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

        public async Task<TimeEntryApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new TimeEntryApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetTimeEntryRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.TimeEntryLogsResponse>(builder.ToString());
                if (response.IsSuccess)
                {
                    retValue.TimeEntryModel = new TimeEntryLogModel();

                    PropertyCopier<R.Models.TimeEntryLog, TimeEntryLogModel>.Copy(response.TimeEntryLog, retValue.TimeEntryModel);
                    PropertyCopier<R.Models.TimeEntryLog, TimeEntryApprovalHolder>.Copy(response.TimeEntryLog, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.EmployeeName;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;

                    switch (retValue.TimeEntryModel.Type)
                    {
                        case TimeEntryTypeValue.TimeIn:
                            retValue.TimeEntryType = TimeEntryTypeDisplay.TimeIn;
                            break;

                        case TimeEntryTypeValue.TimeOut:
                            retValue.TimeEntryType = TimeEntryTypeDisplay.TimeOut;
                            break;

                        case TimeEntryTypeValue.BreakIn:
                            retValue.TimeEntryType = TimeEntryTypeDisplay.BreakIn;
                            break;

                        case TimeEntryTypeValue.BreakOut:
                            retValue.TimeEntryType = TimeEntryTypeDisplay.BreakOut;
                            break;

                        default:
                            break;
                    }

                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }
                }
                else
                    throw new Exception(response.ErrorMessage);

                retValue.IsSuccess = response.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<TimeEntryApprovalHolder> WorkflowTransaction(TimeEntryApprovalHolder form)
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
                throw new Exception(ex.Message);
            }

            return retValue;
        }
    }
}