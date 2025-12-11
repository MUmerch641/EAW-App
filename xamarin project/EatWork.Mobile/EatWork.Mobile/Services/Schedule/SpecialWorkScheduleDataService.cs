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
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class SpecialWorkScheduleDataService : ISpecialWorkScheduleDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;
        private readonly StringHelper string_;

        public SpecialWorkScheduleDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            IDialogService dialogService,
            StringHelper url)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            dialogService_ = dialogService;
            string_ = url;
            employeeProfileDataService_ = AppContainer.Resolve<IEmployeeProfileDataService>();
        }

        public async Task<SpecialWorkScheduleHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new SpecialWorkScheduleHolder()
            {
                ShiftList = new ObservableCollection<ShiftDto>(),
                ShiftSelectedItem = new ShiftDto(),
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            try
            {
                var shiftUrl = new UriBuilder(url)
                {
                    Path = ApiConstants.GetShiftList
                };

                var shiftlist = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.Shift>>(shiftUrl.ToString());

                foreach (var item in shiftlist.ListData)
                {
                    var data = new ShiftDto()
                    {
                        ShiftId = Convert.ToInt64(item.ShiftId),
                        Code = item.Code,
                        WorkSchedule = string.Format("{0} - {1}", item.StartTime.GetValueOrDefault().ToString(Constants.TimeFormatHHMMTT), item.EndTime.GetValueOrDefault().ToString(Constants.TimeFormatHHMMTT))
                    };

                    retValue.ShiftList.Add(data);
                }

                retValue.ShiftList.Add(new ShiftDto() { ShiftId = -1, Code = Constants.OptionOthers });

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetSpecialWorkScheduleRequest}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.WorkScheduleRequestResponse>(builder.ToString());
                    if (response.IsSuccess)
                    {
                        retValue.SpecialWorkScheduleRequestModel = new SpecialWorkScheduleRequestModel();
                        PropertyCopier<R.Models.WorkScheduleRequest, SpecialWorkScheduleRequestModel>.Copy(response.WorkScheduleRequest, retValue.SpecialWorkScheduleRequestModel);

                        #region get shift

                        shiftUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetShiftById}{retValue.SpecialWorkScheduleRequestModel.ShiftId}"
                        };

                        var shift = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(shiftUrl.ToString())).Shift;

                        retValue.WorkDate = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.WorkDate);
                        retValue.ShiftSelectedItem = new ShiftDto()
                        {
                            ShiftId = Convert.ToInt64(retValue.SpecialWorkScheduleRequestModel.ShiftId),
                            Code = (Convert.ToInt64(retValue.SpecialWorkScheduleRequestModel.ShiftId) > 0 ? shift.Code : Constants.OptionOthers)
                        };

                        #endregion get shift

                        #region get request type

                        if (response.WorkScheduleRequest.RequestType > 0)
                        {
                            var enumUrl = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.GetEnums}{EnumValues.WorkScheduleRequestType}/{response.WorkScheduleRequest.RequestType}"
                            };

                            var enumVal = await genericRepository_.GetAsync<R.Models.Enums>(enumUrl.ToString());

                            retValue.RequestType = enumVal.DisplayText;
                        }

                        #endregion get request type

                        retValue.ScheduleStartTime = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.StartTime).TimeOfDay;
                        retValue.ScheduleEndTime = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.EndTime).TimeOfDay;
                        retValue.LunchStartTime = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.LunchBreakStartTime).TimeOfDay;
                        retValue.LunchEndTime = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.LunchBreakEndTime).TimeOfDay;

                        retValue.IsOffSetting = retValue.SpecialWorkScheduleRequestModel.ForOffsetting.GetValueOrDefault();
                        retValue.OffSetExpirationDate = retValue.SpecialWorkScheduleRequestModel.OffsettingExpirationDate.GetValueOrDefault();

                        if (Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.StartTime) != Constants.NullDate)
                        {
                            retValue.StartTimeString = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.StartTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        if (Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.EndTime) != Constants.NullDate)
                        {
                            retValue.EndTimeString = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.EndTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        if (Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.LunchBreakStartTime) != Constants.NullDate)
                        {
                            retValue.LunchStartTimeString = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.LunchBreakStartTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        if (Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.LunchBreakEndTime) != Constants.NullDate)
                        {
                            retValue.LunchEndTimeString = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.LunchBreakEndTime).ToString(Constants.TimeFormatHHMMTT);
                        }
                    }

                    retValue.IsEnabled = (retValue.SpecialWorkScheduleRequestModel.StatusId == RequestStatusValue.Draft);
                    retValue.ShowCancelButton = (retValue.SpecialWorkScheduleRequestModel.StatusId == RequestStatusValue.ForApproval);
                }
                else
                {
                    retValue.SpecialWorkScheduleRequestModel = new SpecialWorkScheduleRequestModel()
                    {
                        ProfileId = userInfo.ProfileId
                    };

                    retValue.WorkDate = selectedDate ?? DateTime.Now.Date;

                    retValue.IsEnabled = true;
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

        public async Task<SpecialWorkScheduleHolder> GetShiftSchedule(SpecialWorkScheduleHolder form)
        {
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            if (form.ShiftSelectedItem.ShiftId < 0)
                form.EnableCustomSched = true;
            else
            {
                var shiftUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetShiftById}get-shift-workdate"
                };

                var requestUrl = string.Format("{0}?id={1}&date={2}", shiftUrl.ToString(), form.ShiftSelectedItem.ShiftId, form.WorkDate.ToString("MM-dd-yyyy"));

                var shift = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(requestUrl)).Shift;
                PropertyCopier<R.Models.Shift, SpecialWorkScheduleRequestModel>.Copy(shift, form.SpecialWorkScheduleRequestModel);

                form.SpecialWorkScheduleRequestModel.StartTime = shift.StartTime;
                form.SpecialWorkScheduleRequestModel.EndTime = shift.EndTime;
                form.ScheduleStartTime = Convert.ToDateTime(shift.StartTime).TimeOfDay;
                form.ScheduleEndTime = Convert.ToDateTime(shift.EndTime).TimeOfDay;
                form.LunchStartTime = Convert.ToDateTime(shift.LunchBreakStartTime).TimeOfDay;
                form.LunchStartTime = Convert.ToDateTime(shift.LunchBreakStartTime).TimeOfDay;
                form.LunchEndTime = Convert.ToDateTime(shift.LunchBreakEndTime).TimeOfDay;

                form.EnableCustomSched = false;
            }

            return form;
        }

        public async Task<SpecialWorkScheduleHolder> SubmitRequest(SpecialWorkScheduleHolder form)
        {
            var errors = new List<int>();
            form.ErrorReason = false;
            form.ErrorShift = false;
            form.ErrorWorkDate = false;
            form.ErrorRequestType = false;

            if (string.IsNullOrWhiteSpace(form.ShiftSelectedItem.Code))
            {
                form.ErrorShift = true;
                errors.Add(1);
            }

            if (form.WorkDate == Constants.NullDate)
            {
                form.ErrorWorkDate = true;
                errors.Add(1);
            }

            if (string.IsNullOrWhiteSpace(form.SpecialWorkScheduleRequestModel.Reason))
            {
                form.ErrorReason = true;
                errors.Add(1);
            }

            if (string.IsNullOrWhiteSpace(form.RequestType))
            {
                form.ErrorRequestType = true;
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
                            form.SpecialWorkScheduleRequestModel.WorkDate = form.WorkDate;
                            form.SpecialWorkScheduleRequestModel.ShiftId = form.ShiftSelectedItem.ShiftId;

                            if (form.ShiftSelectedItem.ShiftId < 0)
                            {
                                var startime = Convert.ToDateTime(form.SpecialWorkScheduleRequestModel.WorkDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.ScheduleStartTime);
                                var endtime = Convert.ToDateTime(form.SpecialWorkScheduleRequestModel.WorkDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.ScheduleEndTime);
                                var lunchstartime = Convert.ToDateTime(form.SpecialWorkScheduleRequestModel.WorkDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.LunchStartTime);
                                var lunchendtime = Convert.ToDateTime(form.SpecialWorkScheduleRequestModel.WorkDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.LunchEndTime);

                                form.SpecialWorkScheduleRequestModel.StartTime = (form.SpecialWorkScheduleRequestModel.WorkingHours != 0 ? startime : Constants.NullDate);
                                form.SpecialWorkScheduleRequestModel.EndTime = (form.SpecialWorkScheduleRequestModel.WorkingHours != 0 ? endtime : Constants.NullDate);
                                form.SpecialWorkScheduleRequestModel.LunchBreakStartTime = (form.SpecialWorkScheduleRequestModel.LunchDuration != 0 ? lunchstartime : Constants.NullDate);
                                form.SpecialWorkScheduleRequestModel.LunchBreakEndTime = (form.SpecialWorkScheduleRequestModel.LunchDuration != 0 ? lunchendtime : Constants.NullDate);
                            }

                            form.SpecialWorkScheduleRequestModel.ForOffsetting = form.IsOffSetting;

                            if (form.IsOffSetting)
                            {
                                form.SpecialWorkScheduleRequestModel.OffsettingExpirationDate = form.OffSetExpirationDate;
                            }
                            else
                            {
                                form.SpecialWorkScheduleRequestModel.OffsettingExpirationDate = Constants.NullDate;
                            }

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitSpecialWorkScheduleRequest
                            };

                            var data = new R.Models.WorkScheduleRequest();
                            PropertyCopier<SpecialWorkScheduleRequestModel, R.Models.WorkScheduleRequest>.Copy(form.SpecialWorkScheduleRequestModel, data);

                            var param = new R.Requests.SubmitWorkScheduleRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitWorkScheduleRequest, R.Responses.BaseResponse<R.Models.WorkScheduleRequest>>(builder.ToString(), param);

                            if (response.Model.WorkScheduleRequestId > 0)
                            {
                                /*
                                if (!string.IsNullOrEmpty(form.FileData.FileName))
                                    await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.ChangeWorkScheduleRequest, response.Model.WorkScheduleRequestId);
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
                                                FileTags = "SWS",
                                                FileType = x.FileType,
                                                MimeType = x.MimeType,
                                                RawFileSize = x.RawFileSize,
                                                ModuleFormId = ModuleForms.ChangeWorkScheduleRequest,
                                                TransactionId = response.Model.WorkScheduleRequestId,
                                            })
                                        );

                                    await commonDataService_.SaveFileAttachmentsAsync(files);
                                }

                                #endregion save file attachments

                                form.Success = true;
                                FormSession.IsSubmitted = true;
                            }
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

        public async Task<SpecialWorkScheduleApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new SpecialWorkScheduleApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetSpecialWorkScheduleRequest}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.WorkScheduleRequestResponse>(builder.ToString());
                if (response.IsSuccess)
                {
                    var shift = new R.Models.Shift()
                    {
                        Code = string.Empty
                    };

                    #region get shift

                    if (response.WorkScheduleRequest.ShiftId > 0)
                    {
                        var shiftUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetShiftById}{response.WorkScheduleRequest.ShiftId}"
                        };

                        shift = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(shiftUrl.ToString())).Shift;
                    }

                    shift.Code = (response.WorkScheduleRequest.ShiftId < 0 ? Constants.OptionOthers : shift.Code);

                    #endregion get shift

                    #region get request type

                    if (response.WorkScheduleRequest.RequestType > 0)
                    {
                        var enumUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.WorkScheduleRequestType}/{response.WorkScheduleRequest.RequestType}"
                        };

                        var enumVal = await genericRepository_.GetAsync<R.Models.Enums>(enumUrl.ToString());

                        retValue.RequestType = enumVal.DisplayText;
                    }

                    #endregion get request type

                    retValue.SpecialWorkScheduleRequestModel = new SpecialWorkScheduleRequestModel();

                    PropertyCopier<R.Models.WorkScheduleRequest, SpecialWorkScheduleRequestModel>.Copy(response.WorkScheduleRequest, retValue.SpecialWorkScheduleRequestModel);
                    PropertyCopier<R.Models.WorkScheduleRequest, SpecialWorkScheduleApprovalHolder>.Copy(response.WorkScheduleRequest, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;

                    retValue.DateFiled = Convert.ToDateTime(response.WorkScheduleRequest.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.WorkDate = Convert.ToDateTime(response.WorkScheduleRequest.WorkDate).ToString(FormHelper.DateFormat);

                    retValue.ShiftCode = string.Format("{0} ({1})", shift.Code,
                        (string.Empty.ConcatDateTime(Convert.ToDateTime(response.WorkScheduleRequest.WorkDate),
                        Convert.ToDateTime(response.WorkScheduleRequest.StartTime),
                        Convert.ToDateTime(response.WorkScheduleRequest.EndTime))));

                    retValue.LunchSchedule = string.Empty.ConcatDateTime(Convert.ToDateTime(response.WorkScheduleRequest.WorkDate),
                        Convert.ToDateTime(response.WorkScheduleRequest.LunchBreakStartTime),
                        Convert.ToDateTime(response.WorkScheduleRequest.LunchBreakEndTime));

                    retValue.WorkingHours = string.Format("{0:0.00} hrs", response.WorkScheduleRequest.WorkingHours);
                    retValue.LunchDuration = string.Format("{0:0.00} hr/s", response.WorkScheduleRequest.LunchDuration);

                    retValue.ShowOffsetting = response.WorkScheduleRequest.ForOffsetting.GetValueOrDefault();
                    retValue.OffsettingExpirationDate = Convert.ToDateTime(response.WorkScheduleRequest.OffsettingExpirationDate).ToString(FormHelper.DateFormat);
                    if (retValue.ShowOffsetting)
                        retValue.IsOffSetting = "Yes";
                    else
                        retValue.IsOffSetting = "No";

                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }

                    if (retValue.SpecialWorkScheduleRequestModel.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
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
                    throw new ArgumentException(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<SpecialWorkScheduleApprovalHolder> WorkflowTransaction(SpecialWorkScheduleApprovalHolder form)
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

                        var values = new
                        {
                            CreateWorkSchedule = new
                            {
                                txtAllowFlexiTimeLimit = 0,
                                txtAllowEarlyTimeInLimit = 0,
                                chkEarlyTimeInOvertime_String = "false",
                                dtpOffsettingExpirationDate = form.OffsettingExpirationDate
                            }
                        };

                        var formData = JsonConvert.SerializeObject(values);

                        var response = new R.Responses.WFTransactionResponse();

                        if (form.SpecialWorkScheduleRequestModel.StatusId == RequestStatusValue.Approved && form.SelectedWorkflowAction.ActionTriggeredId == ActionTypeId.Cancel)
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
                                Path = $"{ApiConstants.SpecialWorkSchedApi}/update-wfsource"
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

        public async Task<SpecialWorkScheduleHolder> WorkflowTransactionRequest(SpecialWorkScheduleHolder form)
        {
            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.ActionTypeId, form.Msg);
                if (confirm.Continue)
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.SpecialWorkScheduleRequestModel.WorkScheduleRequestId, TransactionType.WorkScheduleRequest);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(form.SpecialWorkScheduleRequestModel.WorkScheduleRequestId,
                                             TransactionType.WorkScheduleRequest,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.SpecialWorkSchedApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.SpecialWorkScheduleRequestModel.WorkScheduleRequestId);

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

        public async Task<SpecialWorkScheduleHolder> GetDateSchedule(SpecialWorkScheduleHolder form)
        {
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            using (UserDialogs.Instance.Loading())
            {
                try
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = ApiConstants.GetWorkScheduleRequestByWorkDate
                    };

                    form.SpecialWorkScheduleRequestModel.WorkDate = Convert.ToDateTime(form.WorkDate.ToString(Constants.DateFormatMMDDYYYY));
                    form.SpecialWorkScheduleRequestModel.ShiftId = form.ShiftSelectedItem.ShiftId;

                    if (form.ShiftSelectedItem.ShiftId < 0)
                    {
                        var startime = Convert.ToDateTime(form.SpecialWorkScheduleRequestModel.WorkDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.ScheduleStartTime);
                        var endtime = Convert.ToDateTime(form.SpecialWorkScheduleRequestModel.WorkDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.ScheduleEndTime);
                        var lunchstartime = Convert.ToDateTime(form.SpecialWorkScheduleRequestModel.WorkDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.LunchStartTime);
                        var lunchendtime = Convert.ToDateTime(form.SpecialWorkScheduleRequestModel.WorkDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.LunchEndTime);

                        form.SpecialWorkScheduleRequestModel.StartTime = (form.SpecialWorkScheduleRequestModel.WorkingHours != 0 ? startime : Constants.NullDate);
                        form.SpecialWorkScheduleRequestModel.EndTime = (form.SpecialWorkScheduleRequestModel.WorkingHours != 0 ? endtime : Constants.NullDate);
                        form.SpecialWorkScheduleRequestModel.LunchBreakStartTime = (form.SpecialWorkScheduleRequestModel.LunchDuration != 0 ? lunchstartime : Constants.NullDate);
                        form.SpecialWorkScheduleRequestModel.LunchBreakEndTime = (form.SpecialWorkScheduleRequestModel.LunchDuration != 0 ? lunchendtime : Constants.NullDate);
                    }

                    var data = new R.Models.WorkScheduleRequest();
                    PropertyCopier<SpecialWorkScheduleRequestModel, R.Models.WorkScheduleRequest>.Copy(form.SpecialWorkScheduleRequestModel, data);

                    var param = new R.Requests.SubmitGetScheduleByDateRequest()
                    {
                        Data = data
                    };

                    var response = await genericRepository_.PostAsync<R.Requests.SubmitGetScheduleByDateRequest, R.Responses.WorkSchedRequestDateScheduleResponse>(builder.ToString(), param);

                    if (!string.IsNullOrWhiteSpace(response.RequestType.ToString()))
                    {
                        form.SpecialWorkScheduleRequestModel.RequestType = response.RequestType;
                        switch (response.RequestType)
                        {
                            case 1:
                                form.RequestType = response.HolidayName;
                                break;

                            case 2:
                                form.RequestType = "Restday";
                                break;

                            default:
                                form.RequestType = string.Empty;
                                break;
                        }
                    }

                    form.IsOffsetEnabled = response.IsOffsetEnabled;
                    form.OffSetExpirationDate = new DateTime(response.OffSetExpirationDate.Year, response.OffSetExpirationDate.Month, response.OffSetExpirationDate.Day);

                    //incase the selected workdate changed
                    if (form.ShiftSelectedItem.ShiftId > 0)
                        form = await GetShiftSchedule(form);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return form;
        }
    }
}