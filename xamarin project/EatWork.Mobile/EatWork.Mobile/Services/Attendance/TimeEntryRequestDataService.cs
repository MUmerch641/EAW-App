using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.Utils;
using EAW.API.DataContracts.Requests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class TimeEntryRequestDataService : ITimeEntryRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;

        public TimeEntryRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            IDialogService dialogService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            dialogService_ = dialogService;
            employeeProfileDataService_ = AppContainer.Resolve<IEmployeeProfileDataService>();
        }

        public async Task<TimeEntryHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new TimeEntryHolder();

            var userInfo = PreferenceHelper.UserInfo();

            try
            {
                if (recordId > 0)
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
                        retValue.LogDate = Convert.ToDateTime(retValue.TimeEntryLogModel.TimeEntry);

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

                        /*
                        retValue.IsEnabled = (retValue.TimeEntryLogModel.StatusId == RequestStatusValue.Draft);
                        */

                        if ((FormSession.IsEditTimeLogs && retValue.TimeEntryLogModel.StatusId == 0) ||
                            retValue.TimeEntryLogModel.StatusId == RequestStatusValue.Draft)
                        {
                            retValue.IsEnabled = true;
                            retValue.EnabledTime = false;
                        }

                        retValue.ShowCancelButton = (retValue.TimeEntryLogModel.StatusId == RequestStatusValue.ForApproval);
                    }
                    else
                        throw new Exception(response.ErrorMessage);
                }
                else
                {
                    retValue.TimeEntryLogModel = new TimeEntryLogModel
                    {
                        ProfileId = userInfo.ProfileId,
                        //Source = Constants.SourceTimeEntry
                    };
                    retValue.LogDate = selectedDate ?? DateTime.Now.Date;
                    retValue.IsEnabled = true;
                    retValue.EnabledTime = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FormSession.IsMySchedule = false;
                FormSession.IsEditTimeLogs = false;
            }

            return retValue;
        }

        public async Task<TimeEntryHolder> SubmitRequest(TimeEntryHolder form)
        {
            var errors = new List<int>();
            form.ErrorRemark = false;

            if (form.TimeInChecked)
                form.TimeEntryLogModel.Type = TimeEntryTypeValue.TimeIn;

            if (form.TimeOutChecked)
                form.TimeEntryLogModel.Type = TimeEntryTypeValue.TimeOut;

            if (form.BreakInChecked)
                form.TimeEntryLogModel.Type = TimeEntryTypeValue.BreakIn;

            if (form.BreakOutChecked)
                form.TimeEntryLogModel.Type = TimeEntryTypeValue.BreakOut;

            if (string.IsNullOrWhiteSpace(form.TimeEntryLogModel.Remark))
            {
                form.ErrorRemark = true;
                errors.Add(1);
            }

            if (string.IsNullOrWhiteSpace(form.TimeEntryLogModel.Type))
            {
                errors.Add(1);
                await dialogService_.AlertAsync("Please select log-type");
            }

            if (errors.Count == 0)
            {
                /*if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))*/
                if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                {
                    try
                    {
                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);

                        form.TimeEntryLogModel.TimeEntry = Convert.ToDateTime(form.LogDate).Date + form.LogTime;

                        var data = new R.Models.TimeEntryLog();
                        PropertyCopier<TimeEntryLogModel, R.Models.TimeEntryLog>.Copy(form.TimeEntryLogModel, data);

                        var param = new R.Requests.SubmitTimeEntryLogRequest()
                        {
                            Data = data
                        };

                        //validate
                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.SubmitTimeEntryRequest}/validate"
                        };

                        var validate_response = new R.Responses.ValidateTimeLogResponse();

                        using (UserDialogs.Instance.Loading())
                        {
                            await Task.Delay(500);

                            var validation_param = new R.Requests.SubmitTimeLogValidationRequest()
                            {
                                LogType = data.Type,
                                ProfileId = data.ProfileId.GetValueOrDefault(),
                                TimeEntry = data.TimeEntry.GetValueOrDefault(),
                                /* Added Nash 4/11/2025 Bugs 5864 */
                                HasAttachment = form.FileAttachments.Count() > 0,
                            };

                            validate_response = await genericRepository_.PostAsync<R.Requests.SubmitTimeLogValidationRequest, R.Responses.ValidateTimeLogResponse>(builder.ToString(), validation_param);
                        }

                        if (validate_response.HasConflict)
                        {
                            if (validate_response.WarningOnly)
                            {
                                if (await dialogService_.ConfirmDialogAsync(validate_response.ResponseMessage))
                                {
                                    form = await SaveRequest(form, url, param, builder);
                                }
                            }
                            else if (!validate_response.WarningOnly)
                            {
                                /*
                                await dialogService_.AlertAsync(validate_response.ResponseMessage);
                                return form;
                                */
                                var page = new EatWork.Mobile.Views.Shared.ErrorPage2(Messages.ValidationHeaderMessage, validate_response.ResponseMessage);
                                await Application.Current.MainPage.Navigation.PushModalAsync(page);
                            }
                        }
                        else
                        {
                            form = await SaveRequest(form, url, param, builder);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                        throw;
                    }
                    finally
                    {
                        form.OverrideLeaveConflict = true;
                    }
                }
            }

            return form;
        }

        private async Task<TimeEntryHolder> SaveRequest(TimeEntryHolder form, string url, SubmitTimeEntryLogRequest param, UriBuilder builder)
        {
            try
            {
                using (UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);

                    builder = new UriBuilder(url)
                    {
                        Path = ApiConstants.SubmitTimeEntryRequest
                    };

                    var response = await genericRepository_.PostAsync<R.Requests.SubmitTimeEntryLogRequest, R.Responses.BaseResponse<R.Models.TimeEntryLog>>(builder.ToString(), param);
                    if (response.Model.TimeEntryLogId > 0)
                    {
                        /*
                        if (!string.IsNullOrEmpty(form.FileData.FileName))
                            await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.TimeEntryLogsList, response.Model.TimeEntryLogId);
                        */

                        #region save file attachments

                        if (form.FileAttachments.Count > 0)
                        {
                            var files = new List<FileAttachmentParamsDto>(
                                    form.FileAttachments.Select(x => new FileAttachmentParamsDto()
                                    {
                                        FileDataArray = x.FileDataArray,
                                        FileName = x.FileName,
                                        FileSize = x.FileSize,
                                        FileTags = "TIMELOG",
                                        FileType = x.FileType,
                                        MimeType = x.MimeType,
                                        RawFileSize = x.RawFileSize,
                                        ModuleFormId = ModuleForms.TimeEntryLogsList,
                                        TransactionId = response.Model.TimeEntryLogId,
                                    })
                                );

                            await commonDataService_.SaveFileAttachmentsAsync(files);
                        }

                        #endregion save file attachments

                        form.Success = true;
                        FormSession.IsSubmitted = true;
                    }
                }

                return form;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
        }

        public async Task<TimeEntryHolder> WorkflowTransactionRequest(TimeEntryHolder form)
        {
            form.Success = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.ActionTypeId, form.Msg);
                if (confirm.Continue)
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.TimeEntryLogModel.TimeEntryLogId, TransactionType.TimeLog);

                        var response = await workflowDataService_.CancelWorkFlowByRecordId(form.TimeEntryLogModel.TimeEntryLogId,
                                             TransactionType.TimeLog,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.TimeEntryLogApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.TimeEntryLogModel.TimeEntryLogId);

                            FormSession.IsSubmitted = response.IsSuccess;
                            form.Success = response.IsSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return form;
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

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
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
                            retValue.TimeEntryType = retValue.TimeEntryModel.Type;
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

                    if (retValue.TimeEntryModel.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
                    {
                        retValue.WorkflowActions.Add(new WorkflowAction()
                        {
                            ActionMessage = Messages.Cancel,
                            ActionTriggeredId = ActionTypeId.Cancel,
                            ActionType = "Cancel",
                            CurrentStageId = WFDetail.WorkflowDetail.CurrentStageId,
                            TransactionId = transactionId,
                            TransactionTypeId = transactionTypeId,
                        });
                    }

                    retValue.ImageSource = await employeeProfileDataService_.GetProfileImage(response.EmployeeInfo.ProfileId);
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
            form.IsSuccess = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.SelectedWorkflowAction.ActionTriggeredId, form.SelectedWorkflowAction.ActionMessage);
                if (confirm.Continue)
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);

                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);

                        var response = new R.Responses.WFTransactionResponse();

                        if (form.TimeEntryModel.StatusId == RequestStatusValue.Approved && form.SelectedWorkflowAction.ActionTriggeredId == ActionTypeId.Cancel)
                        {
                            response = await workflowDataService_.CancelWorkFlowByRecordId(form.TransactionId,
                               form.TransactionTypeId,
                               form.SelectedWorkflowAction.ActionTriggeredId,
                               confirm.ResponseText,
                               form.StageId);
                        }
                        else
                        {
                            response = await workflowDataService_.ProcessWorkflowByRecordId(form.TransactionId,
                                form.TransactionTypeId,
                                form.SelectedWorkflowAction.ActionTriggeredId,
                                confirm.ResponseText,
                                form.StageId);
                        }

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.TimeEntryLogApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.TransactionId);

                            form.IsSuccess = response.IsSuccess;
                            FormSession.MyApprovalSelectedItemUpdated = response.IsSuccess;
                            FormSession.MyApprovalSelectedItemStatus = FormSession.SetDefaultMyApprovalSelectedItemStatus(form.SelectedWorkflowAction.ActionType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return form;
        }
    }
}