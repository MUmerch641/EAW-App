using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
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
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class OvertimeRequestDataService : IOvertimeRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;
        private readonly IMyScheduleDataService myScheduleDataService_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;
        private readonly StringHelper string_;

        public OvertimeRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            IDialogService dialogService,
            StringHelper stringHelper,
            IMyScheduleDataService myScheduleDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            dialogService_ = dialogService;
            string_ = stringHelper;
            myScheduleDataService_ = myScheduleDataService;
            employeeProfileDataService_ = AppContainer.Resolve<IEmployeeProfileDataService>();
        }

        public async Task<OvertimeRequestHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new OvertimeRequestHolder()
            {
                OvertimeReasonSelectedItem = new ComboBoxObject(),
                OvertimeReason = new ObservableCollection<ComboBoxObject>() { new ComboBoxObject() { Id = 0, Value = "" } },
                PreshiftOption = new ObservableCollection<string>() { "No", "Yes" }
            };
            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();

            try
            {
                var enumUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{EnumValues.OvertimeType}"
                };

                var enums = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                if (enums.Count() > 0)
                {
                    foreach (var item in enums)
                        retValue.OvertimeReason.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });
                }

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetOvertimeRequestDetail}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.OvertimeRequestDetailResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        if (response.Overtime != null)
                        {
                            retValue.OvertimeModel = new OvertimeModel();
                            PropertyCopier<R.Models.Overtime, OvertimeModel>.Copy(response.Overtime, retValue.OvertimeModel);
                            PropertyCopier<R.Models.Overtime, OvertimeRequestHolder>.Copy(response.Overtime, retValue);

                            retValue.StartTime = Convert.ToDateTime(retValue.OvertimeModel.StartTime).TimeOfDay;
                            if (Convert.ToDateTime(retValue.OvertimeModel.StartTime) != Constants.NullDate)
                            {
                                retValue.StartTimeString = Convert.ToDateTime(retValue.OvertimeModel.StartTime).ToString(Constants.TimeFormatHHMMTT);
                            }

                            retValue.EndTime = Convert.ToDateTime(retValue.OvertimeModel.EndTime).TimeOfDay;
                            if (Convert.ToDateTime(retValue.OvertimeModel.EndTime) != Constants.NullDate)
                            {
                                retValue.EndTimeString = Convert.ToDateTime(retValue.OvertimeModel.EndTime).ToString(Constants.TimeFormatHHMMTT);
                            }

                            retValue.IsOffSetting = retValue.OvertimeModel.ForOffsetting.GetValueOrDefault();
                            retValue.OffSetExpirationDate = retValue.OvertimeModel.OffsettingExpirationDate.GetValueOrDefault();

                            retValue.IsEnabled = (retValue.OvertimeModel.StatusId == RequestStatusValue.Draft);
                            retValue.ShowCancelButton = (retValue.OvertimeModel.StatusId == RequestStatusValue.ForApproval);
                            retValue.IsPreshift = (short)(retValue.OvertimeModel.PreShiftOT != null ? retValue.OvertimeModel.PreShiftOT : 0);
                        }
                    }
                }
                else
                {
                    retValue.OvertimeModel = new OvertimeModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        OvertimeDate = selectedDate ?? DateTime.Now.Date
                    };

                    retValue.IsEnabled = true;
                    retValue.StartDateTime = DateHelper.SetObjectValue(retValue.StartDateTime);
                    retValue.EndDateTime = DateHelper.SetObjectValue(retValue.EndDateTime);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FormSession.IsMySchedule = false;
            }

            return retValue;
        }

        public async Task<OvertimeRequestHolder> SubmitRequest(OvertimeRequestHolder form)
        {
            var errors = new List<int>();
            form.ErrorOTHours = false;
            form.ErrorReason = false;
            form.ErrorStartTime = false;
            form.ErrorEndTime = false;

            if (form.OvertimeModel.OROTHrs == 0)
            {
                form.ErrorOTHours = true;
                errors.Add(1);
            }

            if (string.IsNullOrWhiteSpace(form.OvertimeModel.Reason))
            {
                form.ErrorReason = true;
                errors.Add(1);
            }

            if (!string.IsNullOrWhiteSpace(form.EndTimeString) && string.IsNullOrWhiteSpace(form.StartTimeString))
            {
                form.ErrorStartTime = true;
                errors.Add(1);
            }

            if (!string.IsNullOrWhiteSpace(form.StartTimeString) && string.IsNullOrWhiteSpace(form.EndTimeString))
            {
                form.ErrorEndTime = true;
                errors.Add(1);
            }

            if (!form.IsContinue)
            {
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                /*if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))*/
                if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        try
                        {
                            await Task.Delay(500);
                            var startime = Convert.ToDateTime(form.OvertimeModel.OvertimeDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.StartTimeString);
                            var endtime = Convert.ToDateTime(form.OvertimeModel.OvertimeDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.EndTimeString);

                            endtime = endtime.AddDays((endtime <= startime ? 1 : 0));

                            form.OvertimeModel.StartTime = (!string.IsNullOrWhiteSpace(form.StartTimeString) ? startime : Constants.NullDate);
                            form.OvertimeModel.EndTime = (!string.IsNullOrWhiteSpace(form.EndTimeString) ? endtime : Constants.NullDate);

                            form.OvertimeModel.ForOffsetting = form.IsOffSetting;

                            if (form.IsOffSetting)
                                form.OvertimeModel.OffsettingExpirationDate = form.OffSetExpirationDate;
                            else
                                form.OvertimeModel.OffsettingExpirationDate = Constants.NullDate;

                            form.OvertimeModel.PreShiftOT = form.IsPreshift;
                            form.OvertimeModel.OffsettingExpirationDate = form.OffSetExpirationDate;

                            form = await SaveRecordAsync(form);

                            /*
                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitOvertimeRequest
                            };

                            var data = new R.Models.Overtime();
                            PropertyCopier<OvertimeModel, R.Models.Overtime>.Copy(form.OvertimeModel, data);

                            var param = new R.Requests.SubmitOvertimeRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitOvertimeRequest, R.Responses.BaseResponse<R.Models.Overtime>>(builder.ToString(), param);

                            if (response.Model.OvertimeId > 0)
                            {
                                if (!string.IsNullOrEmpty(form.FileData.FileName))
                                    await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.OvertimeRequest, response.Model.OvertimeId);

                                retValue.Success = true;
                                FormSession.IsSubmitted = true;
                            }
                            */
                        }
                        catch (HttpRequestExceptionEx ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }

            return form;
        }

        public async Task<OvertimeApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new OvertimeApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetOvertimeRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.OvertimeRequestDetailResponse>(builder.ToString());
                if (response.IsSuccess)
                {
                    retValue.OvertimeModel = new OvertimeModel();

                    PropertyCopier<R.Models.Overtime, OvertimeModel>.Copy(response.Overtime, retValue.OvertimeModel);
                    PropertyCopier<R.Models.Overtime, OvertimeApprovalHolder>.Copy(response.Overtime, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = Convert.ToDateTime(response.Overtime.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.OvertimeDate = Convert.ToDateTime(response.Overtime.OvertimeDate).ToString(FormHelper.DateFormat);
                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    retValue.ShowOffsetting = response.Overtime.ForOffsetting.GetValueOrDefault();
                    retValue.OffsettingExpirationDate = Convert.ToDateTime(response.Overtime.OffsettingExpirationDate).ToString(FormHelper.DateFormat);
                    if (retValue.ShowOffsetting)
                        retValue.IsOffSetting = "Yes";
                    else
                        retValue.IsOffSetting = "No";

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }

                    if (retValue.OvertimeModel.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
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

                    if (response.Overtime.StartTime != null && response.Overtime.EndTime != null)
                    {
                        retValue.TimeRange = string.Empty.ConcatDateTime((DateTime)response.Overtime.OvertimeDate,
                            (DateTime)response.Overtime.StartTime,
                            (DateTime)response.Overtime.EndTime);
                    }

                    retValue.OvertimeHours = string.Format("{0:0.00} hrs", response.Overtime.OROTHrs + response.Overtime.NSOTHrs);
                    retValue.IsPreshift = (response.Overtime.PreShiftOT == 1 ? "Yes" : "No");
                    retValue.ImageSource = await employeeProfileDataService_.GetProfileImage(response.EmployeeInfo.ProfileId);
                    retValue.ApprovedHours = response.Overtime.ApprovedOROTHrs.GetValueOrDefault();
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

        public async Task<OvertimeApprovalHolder> WorkflowTransaction(OvertimeApprovalHolder form)
        {
            form.IsSuccess = false;

            try
            {
                var approvedOTHours = (form.ApprovedHours != 0 ? form.ApprovedHours : form.OvertimeModel.OROTHrs);

                var confirm = await workflowDataService_.ConfirmationMessage(form.SelectedWorkflowAction.ActionTriggeredId, form.SelectedWorkflowAction.ActionMessage);
                if (confirm.Continue)
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);

                        var values = new
                        {
                            EnterApprovedHours = new
                            {
                                txtApprovedOROTHrs = approvedOTHours,
                                dtpOffsettingExpirationDate = form.OffsettingExpirationDate,
                            }
                        };

                        var formData = JsonConvert.SerializeObject(values);

                        var response = new R.Responses.WFTransactionResponse();

                        if (form.OvertimeModel.StatusId == RequestStatusValue.Approved && form.SelectedWorkflowAction.ActionTriggeredId == ActionTypeId.Cancel)
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
                                form.StageId,
                                formData);
                        }

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.OvertimeApi}/update-wfsource"
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

        public async Task<OvertimeRequestHolder> WorkflowTransactionRequest(OvertimeRequestHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.OvertimeModel.OvertimeId, TransactionType.Overtime);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(form.OvertimeModel.OvertimeId,
                                             TransactionType.Overtime,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.OvertimeApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.OvertimeModel.OvertimeId);

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

        public async Task<OvertimeRequestHolder> PreOTValidation(OvertimeRequestHolder form)
        {
            form.IsContinue = true;
            form.ErrorDate = false;
            form.WSStartTime = null;
            form.WSEndTime = null;

            form.StartTime = null;
            form.EndTime = null;

            try
            {
                var response = new R.Responses.OTPreValidationResponse();
                var userInfo = PreferenceHelper.UserInfo();
                var url = await commonDataService_.RetrieveClientUrl();
                var paramDate = form.OvertimeModel.OvertimeDate.Value.ToString("yyyy-MM-dd");

                var scheduleUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetPreOTValidations}{userInfo.ProfileId}/{paramDate}"
                };

                var schedule = new MyScheduleListModel();

                using (UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);

                    schedule = await myScheduleDataService_.RetrieveCurrentSchedule(new ListParam()
                    {
                        Count = 10,
                        StartDate = form.OvertimeModel.OvertimeDate.GetValueOrDefault().ToString(Constants.DateFormatMMDDYYYY),
                        EndDate = form.OvertimeModel.OvertimeDate.GetValueOrDefault().ToString(Constants.DateFormatMMDDYYYY),
                        ListCount = 10,
                    });

                    if (schedule != null)
                    {
                        response = await genericRepository_.GetAsync<R.Responses.OTPreValidationResponse>(scheduleUrl.ToString());

                        form.ShowWSField = schedule.HasSchedule;

                        if (schedule.HasSchedule)
                        {
                            form.WSStartTime = schedule.WSStartTime.GetValueOrDefault(Constants.NullDate).TimeOfDay;
                            form.WSEndTime = schedule.WSEndTime.GetValueOrDefault(Constants.NullDate).TimeOfDay;
                        }

                        if (response != null)
                        {
                            form.EnablePreshiftOT = (!response.DisablePreshiftOT);
                            form.TimeInLimit = response.TimeInLimit;
                            form.TimeOutLimit = response.TimeOutLimit;
                            form.AllowEarlyTimeIn = response.AllowEarlyTimeIn;
                            form.EarlyTimeInLimit = response.EarlyTimeInLimit;
                            form.EarlyTimeInIsOvertime = response.EarlyTimeInIsOvertime;
                            form.MinimumOT = response.MinimumOT;
                            form.IncludePreshiftInMinOT = response.IncludePreshiftInMinOT;
                            form.IsOffsetEnabled = response.IsOffsetEnabled;
                            form.OffSetExpirationDate = new DateTime(response.OffSetExpirationDate.Year, response.OffSetExpirationDate.Month, response.OffSetExpirationDate.Day);
                        }
                    }
                }

                if (form.OvertimeModel.OvertimeId == 0)
                {
                    if (schedule != null)
                    {
                        if (schedule.HasSchedule)
                        {
                            form.StartTimeString = string.Empty;
                            form.EndTimeString = string.Empty;

                            if (form.IsPreshift == 0)
                            {
                                if (schedule.WSStartTime.GetValueOrDefault(Constants.NullDate) > Constants.NullDate)
                                {
                                    form.StartTime = schedule.WSEndTime.GetValueOrDefault(Constants.NullDate).TimeOfDay;
                                    form.StartTimeString = schedule.WSEndTime.GetValueOrDefault(Constants.NullDate).ToString(Constants.TimeFormatHHMMTT);
                                }
                            }
                            else
                            {
                                if (schedule.WSStartTime.GetValueOrDefault(Constants.NullDate) > Constants.NullDate)
                                {
                                    form.EndTime = schedule.WSStartTime.GetValueOrDefault(Constants.NullDate).TimeOfDay;
                                    form.EndTimeString = schedule.WSStartTime.GetValueOrDefault(Constants.NullDate).ToString(Constants.TimeFormatHHMMTT);
                                }
                            }
                        }
                    }

                    if (response != null)
                    {
                        if (response.NoWorkSchedule)
                        {
                            form.IsContinue = true;
                            form.ErrorDate = true;
                            form.ErrorDateMessage = Messages.NoWorkSchedule;
                        }

                        if (response.NoTimeEntryLogs)
                        {
                            form.IsContinue = true;
                            form.ErrorDate = true;
                            form.ErrorDateMessage = Messages.NoTimeEntryLogs;
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

        private async Task<OvertimeRequestHolder> SaveRecordAsync(OvertimeRequestHolder form)
        {
            var response = new R.Responses.SubmitOvertimeRequestResponse();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);
            try
            {
                var data = new R.Models.Overtime();
                PropertyCopier<OvertimeModel, R.Models.Overtime>.Copy(form.OvertimeModel, data);

                var param = new R.Requests.SubmitOvertimeRequest()
                {
                    Data = data,
                    MinimumOTHoursToggle = form.MinimumOTHoursToggle,
                    OverrideMinimumOT = form.OverrideMinimumOT,
                };

                using (UserDialogs.Instance.Loading())
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = ApiConstants.ValidateOvertimeRequest
                    };

                    await Task.Delay(500);
                    response = await genericRepository_.PostAsync<R.Requests.SubmitOvertimeRequest, R.Responses.SubmitOvertimeRequestResponse>(builder.ToString(), param);
                }

                if (response.ErrorMessages.Count > 0)
                {
                    form.MinimumOTHoursToggle = response.MinimumOTHoursToggle;

                    if (response.MinimumOTHoursToggle)
                    {
                        var list = new ObservableCollection<string>(response.ErrorMessages);
                        if (await dialogService_.ConfirmDialog2Async(list, Messages.ValidationHeaderMessage))
                        {
                            form.OverrideMinimumOT = true;
                            await SaveRecordAsync(form);
                        }
                    }
                    else
                    {
                        var colletion = new ObservableCollection<string>(response.ErrorMessages);
                        var page = new EatWork.Mobile.Views.Shared.ErrorPage(colletion, Messages.ValidationHeaderMessage);
                        await Application.Current.MainPage.Navigation.PushModalAsync(page);
                    }
                }
                else
                {
                    //save record

                    var builder = new UriBuilder(url)
                    {
                        Path = ApiConstants.SubmitOvertimeRequest
                    };

                    var retVal = await genericRepository_.PostAsync<R.Requests.SubmitOvertimeRequest, R.Responses.BaseResponse<R.Models.Overtime>>(builder.ToString(), param);

                    if (retVal.Model.OvertimeId > 0)
                    {
                        /*
                        if (!string.IsNullOrEmpty(form.FileData.FileName))
                            await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.OvertimeRequest, retVal.Model.OvertimeId);
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
                                        FileTags = "OVERTIME",
                                        FileType = x.FileType,
                                        MimeType = x.MimeType,
                                        RawFileSize = x.RawFileSize,
                                        ModuleFormId = ModuleForms.OvertimeRequest,
                                        TransactionId = retVal.Model.OvertimeId,
                                    })
                                );

                            await commonDataService_.SaveFileAttachmentsAsync(files);
                        }

                        #endregion save file attachments

                        form.Success = true;
                        FormSession.IsSubmitted = true;
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