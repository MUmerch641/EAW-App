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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class ChangeWorkScheduleDataService : IChangeWorkScheduleDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;
        private readonly StringHelper string_;

        public ChangeWorkScheduleDataService(IGenericRepository genericRepository,
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

        public async Task<ChangeWorkScheduleHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new ChangeWorkScheduleHolder()
            {
                ChangeWorkScheduleModel = new ChangeWorkScheduleModel(),
                ShiftList = new ObservableCollection<ShiftDto>(),
                ShiftSelectedItem = new ShiftDto(),
                ReasonList = new ObservableCollection<ComboBoxObject>(),
                ReasonSelectedItem = new ComboBoxObject(),
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);
            try
            {
                /*
                var shiftUrl = new UriBuilder(url)
                {
                    Path = ApiConstants.GetShiftList
                };
                */

                /*
                var shiftlist = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.Shift>>(shiftUrl.ToString());

                */

                /*
                foreach (var item in shiftlist.ListData)
                    retValue.ShiftList.Add(new SelectableListModel() { Id = Convert.ToInt64(item.ShiftId), DisplayText = item.Code, IsChecked = false });

                retValue.ShiftList.Add(new SelectableListModel() { Id = -1, DisplayText = Constants.OptionOthers, IsChecked = false });
                */

                var enumUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{EnumValues.ChangeWorkScheduleReason}"
                };

                var enumlist = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                foreach (var item in enumlist)
                    retValue.ReasonList.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetChangeWorkSchedRequestDetail}{recordId}"
                    };

                    var shift = (new R.Responses.ShiftDetailResponse()).Shift;

                    var response = await genericRepository_.GetAsync<R.Responses.ChangeWorkSchedRequestDetailResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        retValue.ChangeWorkScheduleModel = new ChangeWorkScheduleModel();
                        PropertyCopier<R.Models.ChangeWorkSchedule, ChangeWorkScheduleModel>.Copy(response.ChangeWorkSchedule, retValue.ChangeWorkScheduleModel);

                        #region old schedule

                        retValue.OriginalShiftCode = string.Empty;
                        if (retValue.ChangeWorkScheduleModel.OriginalShiftId > 0)
                        {
                            var shiftUrl = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.GetShiftById}{retValue.ChangeWorkScheduleModel.OriginalShiftId}"
                            };

                            retValue.OriginalShiftCode = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(shiftUrl.ToString())).Shift.Code;
                        }

                        retValue.OriginalShiftCode = (retValue.ChangeWorkScheduleModel.OriginalShiftId < 0 ? Constants.OptionOthers : retValue.OriginalShiftCode);

                        #endregion old schedule

                        #region get shift

                        if (retValue.ChangeWorkScheduleModel.ShiftId > 0)
                        {
                            var shiftUrl = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.GetShiftById}{retValue.ChangeWorkScheduleModel.ShiftId}"
                            };

                            shift = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(shiftUrl.ToString())).Shift;

                            retValue.ShiftSelectedItem = new ShiftDto();
                            PropertyCopier<R.Models.Shift, ShiftDto>.Copy(shift, retValue.ShiftSelectedItem);
                        }
                        else
                        {
                            retValue.ShiftSelectedItem = new ShiftDto()
                            {
                                ShiftId = -1,
                                Code = Constants.OptionOthers
                            };
                        }

                        #endregion get shift

                        retValue.WorkDate = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.WorkDate);
                        retValue.ScheduleStartTime = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.RequestedStartTime).TimeOfDay;
                        retValue.ScheduleEndTime = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.RequestedEndTime).TimeOfDay;
                        retValue.LunchStartTime = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.LunchBreakStartTime).TimeOfDay;
                        retValue.LunchEndTime = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.LunchBreakEndTime).TimeOfDay;

                        if (Convert.ToDateTime(retValue.ChangeWorkScheduleModel.RequestedStartTime) != Constants.NullDate)
                        {
                            retValue.StartTimeString = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.RequestedStartTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        if (Convert.ToDateTime(retValue.ChangeWorkScheduleModel.RequestedEndTime) != Constants.NullDate)
                        {
                            retValue.EndTimeString = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.RequestedEndTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        if (Convert.ToDateTime(retValue.ChangeWorkScheduleModel.LunchBreakStartTime) != Constants.NullDate)
                        {
                            retValue.LunchStartTimeString = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.LunchBreakStartTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        if (Convert.ToDateTime(retValue.ChangeWorkScheduleModel.LunchBreakEndTime) != Constants.NullDate)
                        {
                            retValue.LunchEndTimeString = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.LunchBreakEndTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        retValue.OriginalSchedule = string.Format("Shift: {0}{3}Start: {1}{3}End: {2}", retValue.OriginalShiftCode,
                            Convert.ToDateTime(retValue.ChangeWorkScheduleModel.WorkScheduleStartTime).ToString(Constants.DateFormatMMDDYYYYHHMMTT),
                            Convert.ToDateTime(retValue.ChangeWorkScheduleModel.WorkScheduleEndTime).ToString(Constants.DateFormatMMDDYYYYHHMMTT),
                            Constants.NextLine);
                    }
                    else
                        throw new Exception(response.ErrorMessage);

                    retValue.IsEnabled = (retValue.ChangeWorkScheduleModel.StatusId == RequestStatusValue.Draft);
                    retValue.ShowCancelButton = (retValue.ChangeWorkScheduleModel.StatusId == RequestStatusValue.ForApproval);

                    retValue.IsEditable = Constants.EditableStatusLookup.Split(',')
                              .Where(x => string.Compare(retValue.ChangeWorkScheduleModel.StatusId.ToString(), x, true) == 0)
                              .Count() > 0;
                }
                else
                {
                    retValue.ChangeWorkScheduleModel = new ChangeWorkScheduleModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        WorkDate = selectedDate ?? DateTime.Now.Date
                    };

                    retValue.WorkDate = selectedDate ?? DateTime.Now.Date;

                    retValue.IsEnabled = true;

                    retValue = await GetEmployeeSchedule(retValue, 1);

                    retValue.IsEditable = true;
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

        public async Task<ChangeWorkScheduleHolder> GetEmployeeSchedule(ChangeWorkScheduleHolder form, int option)
        {
            try
            {
                if (form.ChangeWorkScheduleModel.ChangeWorkScheduleId == 0)
                {
                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);
                    form.ShiftList = new ObservableCollection<ShiftDto>();

                    //await Task.Delay(500);
                    //using (UserDialogs.Instance.Loading())
                    //{
                    //by work date
                    if (option == 1)
                    {
                        var scheduleUrl = new UriBuilder(url)
                        {
                            Path = string.Format(ApiConstants.GetEmployeScheduleByWorkDate, form.ChangeWorkScheduleModel.ProfileId)
                        };

                        var param = new R.Requests.GetEmployeeWorkScheduleRequest()
                        {
                            WorkDate = Convert.ToDateTime(form.WorkDate)
                        };

                        var request = string_.CreateUrl<R.Requests.GetEmployeeWorkScheduleRequest>(scheduleUrl.ToString(), param);

                        var schedule = (await genericRepository_.GetAsync<R.Responses.GetEmployeeWorkScheduleResponse>(request)).Model;

                        if (schedule != null)
                        {
                            /*PropertyCopier<R.Models.WorkSchedule, ChangeWorkScheduleModel>.Copy(schedule, form.ChangeWorkScheduleModel);*/
                            form.OriginalShiftCode = schedule.Shift.Code;
                            form.ChangeWorkScheduleModel.OriginalShiftId = schedule.ShiftId;

                            form.ChangeWorkScheduleModel.WorkScheduleStartTime = schedule.StartTime;
                            form.ChangeWorkScheduleModel.WorkScheduleEndTime = schedule.EndTime;

                            form.OriginalSchedule = string.Format("Shift: {0}{3}Start: {1}{3}End: {2}", form.OriginalShiftCode,
                                Convert.ToDateTime(form.ChangeWorkScheduleModel.WorkScheduleStartTime).ToString(Constants.DateFormatMMDDYYYYHHMMTT),
                                Convert.ToDateTime(form.ChangeWorkScheduleModel.WorkScheduleEndTime).ToString(Constants.DateFormatMMDDYYYYHHMMTT),
                                Constants.NextLine);

                            foreach (var shift in schedule.ShiftListByGroup)
                            {
                                var data = new ShiftDto();
                                PropertyCopier<R.Models.Shift, ShiftDto>.Copy(shift, data);
                                data.WorkSchedule = string.Format("{0} - {1}", shift.StartTime.GetValueOrDefault().ToString(Constants.TimeFormatHHMMTT),
                                    shift.EndTime.GetValueOrDefault().ToString(Constants.TimeFormatHHMMTT));
                                form.ShiftList.Add(data);
                            }
                        }

                        form.ShiftList.Add(new ShiftDto() { ShiftId = -1, Code = Constants.OptionOthers });

                        //incase the selected workdate changed
                        if (form.ShiftSelectedItem.ShiftId > 0)
                            form = await GetShiftSchedule(form, url);
                    }

                    //by shift
                    if (option == 2)
                    {
                        form = await GetShiftSchedule(form, url);
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return form;
        }

        public async Task<ChangeWorkScheduleHolder> SubmitRequest(ChangeWorkScheduleHolder form)
        {
            var errors = new List<int>();
            form.ErrorReason = false;
            form.ErrorRequestedEndTime = false;
            form.ErrorRequestedStartTime = false;
            form.ErrorShift = false;
            form.ErrorWorkDate = false;

            if (string.IsNullOrWhiteSpace(form.ShiftSelectedItem.Code))
            {
                form.ErrorShift = true;
                errors.Add(1);
            }

            if (form.ChangeWorkScheduleModel.Reason == 0)
            {
                form.ErrorReason = true;
                errors.Add(1);
            }

            if (form.ReasonSelectedItem.Id == 0)
            {
                form.ErrorReason = true;
                errors.Add(1);
            }

            if (form.ScheduleStartTime == Constants.NullDate.TimeOfDay || form.ScheduleEndTime == Constants.NullDate.TimeOfDay)
            {
                form.ErrorRequestedStartTime = true;
                form.ErrorRequestedEndTime = true;
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
                            form.ChangeWorkScheduleModel.Reason = (short)form.ReasonSelectedItem.Id;
                            form.ChangeWorkScheduleModel.ShiftId = form.ShiftSelectedItem.ShiftId;
                            form.ChangeWorkScheduleModel.WorkDate = Convert.ToDateTime(form.WorkDate.ToString(Constants.DateFormatMMDDYYYY));

                            if (form.ShiftSelectedItem.ShiftId < 0)
                            {
                                var startime = Convert.ToDateTime(form.ChangeWorkScheduleModel.WorkDate.Value.Date.ToString(Constants.DateFormatMMDDYYYY) + " " + form.ScheduleStartTime);
                                var endtime = Convert.ToDateTime(form.ChangeWorkScheduleModel.WorkDate.Value.Date.ToString(Constants.DateFormatMMDDYYYY) + " " + form.ScheduleEndTime);
                                var lunchstartime = Convert.ToDateTime(form.ChangeWorkScheduleModel.WorkDate.Value.Date.ToString(Constants.DateFormatMMDDYYYY) + " " + form.LunchStartTime);
                                var lunchendtime = Convert.ToDateTime(form.ChangeWorkScheduleModel.WorkDate.Value.Date.ToString(Constants.DateFormatMMDDYYYY) + " " + form.LunchEndTime);

                                form.ChangeWorkScheduleModel.RequestedStartTime = (form.ChangeWorkScheduleModel.WorkingHours != 0 ? startime : Constants.NullDate);
                                form.ChangeWorkScheduleModel.RequestedEndTime = (form.ChangeWorkScheduleModel.WorkingHours != 0 ? endtime : Constants.NullDate);
                                form.ChangeWorkScheduleModel.LunchBreakStartTime = (form.ChangeWorkScheduleModel.LunchDuration != 0 ? lunchstartime : Constants.NullDate);
                                form.ChangeWorkScheduleModel.LunchBreakEndTime = (form.ChangeWorkScheduleModel.LunchDuration != 0 ? lunchendtime : Constants.NullDate);

                                if (form.ChangeWorkScheduleModel.StartTimePreviousDay.GetValueOrDefault())
                                    form.ChangeWorkScheduleModel.RequestedStartTime = form.ChangeWorkScheduleModel.RequestedStartTime.GetValueOrDefault().AddDays(-1);

                                if (form.ChangeWorkScheduleModel.EndTimeNextDay.GetValueOrDefault())
                                    form.ChangeWorkScheduleModel.RequestedEndTime = form.ChangeWorkScheduleModel.RequestedEndTime.GetValueOrDefault().AddDays(1);
                            }

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitChangeWorkScheduleRequest
                            };

                            var data = new R.Models.ChangeWorkSchedule();
                            PropertyCopier<ChangeWorkScheduleModel, R.Models.ChangeWorkSchedule>.Copy(form.ChangeWorkScheduleModel, data);

                            var param = new R.Requests.SubmitChangeWorkScheduleRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitChangeWorkScheduleRequest, R.Responses.BaseResponse<R.Models.ChangeWorkSchedule>>(builder.ToString(), param);

                            if (response.Model.ChangeWorkScheduleId > 0)
                            {
                                /*
                                if (!string.IsNullOrEmpty(form.FileData.FileName))
                                    await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.ChangeWorkScheduleRequest, response.Model.ChangeWorkScheduleId);
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
                                                FileTags = "CWS",
                                                FileType = x.FileType,
                                                MimeType = x.MimeType,
                                                RawFileSize = x.RawFileSize,
                                                ModuleFormId = ModuleForms.ChangeWorkScheduleRequest,
                                                TransactionId = response.Model.ChangeWorkScheduleId,
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

        public async Task<ChangeWorkScheduleApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new ChangeWorkScheduleApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetChangeWorkSchedRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.ChangeWorkSchedRequestDetailResponse>(builder.ToString());

                if (response.IsSuccess)
                {
                    var employee = new R.Responses.BaseResponse<R.Models.EmployeeInformation>();

                    if (response.ChangeWorkSchedule.SwapWithProfileId > 0)
                    {
                        builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEmployeeById}{response.ChangeWorkSchedule.SwapWithProfileId}"
                        };

                        employee = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.EmployeeInformation>>(builder.ToString());
                    }

                    var enumVal = new R.Models.Enums();

                    if (response.ChangeWorkSchedule.Reason == 0)
                    {
                        var enumUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.ChangeWorkScheduleReason}/{response.ChangeWorkSchedule.Reason}"
                        };

                        enumVal = await genericRepository_.GetAsync<R.Models.Enums>(enumUrl.ToString());
                    }

                    retValue.ChangeWorkScheduleModel = new ChangeWorkScheduleModel();

                    PropertyCopier<R.Models.ChangeWorkSchedule, ChangeWorkScheduleModel>.Copy(response.ChangeWorkSchedule, retValue.ChangeWorkScheduleModel);
                    PropertyCopier<R.Models.ChangeWorkSchedule, ChangeWorkScheduleApprovalHolder>.Copy(response.ChangeWorkSchedule, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = Convert.ToDateTime(response.ChangeWorkSchedule.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.WorkDate = Convert.ToDateTime(response.ChangeWorkSchedule.WorkDate).ToString(FormHelper.DateFormat);
                    retValue.WorkingHours = string.Format("{0:0.00} hrs", response.ChangeWorkSchedule.WorkingHours);
                    retValue.Reason = response.ChangeWorkSchedule.ReasonDisplay;

                    retValue.OriginalSchedule = string.Empty.ConcatDateTime((DateTime)response.ChangeWorkSchedule.WorkDate,
                        (DateTime)response.ChangeWorkSchedule.WorkScheduleStartTime,
                        (DateTime)response.ChangeWorkSchedule.WorkScheduleEndTime);

                    retValue.RequestedSchedule = string.Empty.ConcatDateTime((DateTime)response.ChangeWorkSchedule.WorkDate,
                        (DateTime)response.ChangeWorkSchedule.RequestedStartTime,
                        (DateTime)response.ChangeWorkSchedule.RequestedEndTime);

                    retValue.LunchSchedule = string.Empty.ConcatDateTime((DateTime)response.ChangeWorkSchedule.WorkDate,
                        (DateTime)response.ChangeWorkSchedule.LunchBreakStartTime,
                        (DateTime)response.ChangeWorkSchedule.LunchBreakEndTime);

                    retValue.SwapWithEmployeeName = (response.ChangeWorkSchedule.SwapWithProfileId > 0 ? employee.Model.EmployeeName : "");

                    retValue.Reason = (response.ChangeWorkSchedule.Reason == 0 ? enumVal.DisplayText : string.Empty);

                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }

                    if (retValue.ChangeWorkScheduleModel.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
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

                retValue.IsSuccess = response.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<ChangeWorkScheduleApprovalHolder> WorkflowTransaction(ChangeWorkScheduleApprovalHolder form)
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

                        if (form.ChangeWorkScheduleModel.StatusId == RequestStatusValue.Approved && form.SelectedWorkflowAction.ActionTriggeredId == ActionTypeId.Cancel)
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
                                Path = $"{ApiConstants.ChangeWorkScheduleApi}/update-wfsource"
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

        public async Task<ChangeWorkScheduleHolder> WorkflowTransactionRequest(ChangeWorkScheduleHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.ChangeWorkScheduleModel.ChangeWorkScheduleId, TransactionType.ChangeWorkSchedule);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(form.ChangeWorkScheduleModel.ChangeWorkScheduleId,
                                             TransactionType.ChangeWorkSchedule,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.ChangeWorkScheduleApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.ChangeWorkScheduleModel.ChangeWorkScheduleId);

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

        private async Task<ChangeWorkScheduleHolder> GetShiftSchedule(ChangeWorkScheduleHolder form, string url)
        {
            if (form.ShiftSelectedItem.ShiftId <= 0)
            {
                form.EnableCustomSched = true;
                form.ChangeWorkScheduleModel.RequestedStartTime = null;
                form.ChangeWorkScheduleModel.RequestedEndTime = null;
                form.ScheduleStartTime = null;
                form.ScheduleEndTime = null;
                form.LunchStartTime = null;
                form.LunchEndTime = null;
                form.LunchEndTime = null;
                form.ChangeWorkScheduleModel.StartTimePreviousDay = false;
                form.ChangeWorkScheduleModel.EndTimeNextDay = false;
                form.ChangeWorkScheduleModel.LunchDuration = 0;
                form.ChangeWorkScheduleModel.WorkingHours = 0;
            }
            else
            {
                var shiftUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetShiftById}get-shift-workdate"
                };

                var requestUrl = string.Format("{0}?id={1}&date={2}", shiftUrl.ToString(), form.ShiftSelectedItem.ShiftId, form.WorkDate.ToString("MM-dd-yyyy"));

                var shift = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(requestUrl)).Shift;

                if (shift != null)
                {
                    PropertyCopier<R.Models.Shift, ChangeWorkScheduleModel>.Copy(shift, form.ChangeWorkScheduleModel);

                    form.ChangeWorkScheduleModel.RequestedStartTime = shift.StartTime;
                    form.ChangeWorkScheduleModel.RequestedEndTime = shift.EndTime;
                    form.ScheduleStartTime = Convert.ToDateTime(shift.StartTime).TimeOfDay;
                    form.ScheduleEndTime = Convert.ToDateTime(shift.EndTime).TimeOfDay;
                    form.LunchStartTime = Convert.ToDateTime(shift.LunchBreakStartTime).TimeOfDay;
                    form.LunchEndTime = Convert.ToDateTime(shift.LunchBreakEndTime).TimeOfDay;
                    form.ChangeWorkScheduleModel.StartTimePreviousDay = Convert.ToBoolean(shift.StartTimePreviousDay);
                    form.ChangeWorkScheduleModel.EndTimeNextDay = Convert.ToBoolean(shift.EndTimeNextDay);

                    form.EnableCustomSched = false;
                }
            }

            return form;
        }
    }
}