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
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class ChangeWorkScheduleDataService : IChangeWorkScheduleDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly StringHelper string_;

        public ChangeWorkScheduleDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            StringHelper url)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            string_ = url;
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
                var enumUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{EnumValues.ChangeWorkScheduleReason}"
                };

                var shiftUrl = new UriBuilder(url)
                {
                    Path = ApiConstants.GetShiftList
                };

                var enumlist = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());
                var shiftlist = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.Shift>>(shiftUrl.ToString());

                foreach (var item in enumlist)
                    retValue.ReasonList.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });

                foreach (var item in shiftlist.ListData)
                    retValue.ShiftList.Add(new ShiftDto() { ShiftId = Convert.ToInt64(item.ShiftId), Code = item.Code });

                retValue.ShiftList.Add(new ShiftDto() { ShiftId = -1, Code = Constants.OptionOthers });

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
                        #region old schedule

                        retValue.OriginalShiftCode = string.Empty;
                        if (retValue.ChangeWorkScheduleModel.OriginalShiftId > 0)
                        {
                            shiftUrl = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.GetShiftById}{retValue.ChangeWorkScheduleModel.OriginalShiftId}"
                            };

                            retValue.OriginalShiftCode = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(shiftUrl.ToString())).Shift.Code;
                        }
                        else
                            retValue.OriginalShiftCode = Constants.OptionOthers;

                        #endregion old schedule

                        #region get enum val

                        var enumVal = new R.Models.Enums();
                        if (response.ChangeWorkSchedule.Reason == 0)
                        {
                            enumUrl = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.GetEnums}{EnumValues.ChangeWorkScheduleReason}/{response.ChangeWorkSchedule.Reason}"
                            };

                            enumVal = await genericRepository_.GetAsync<R.Models.Enums>(enumUrl.ToString());

                            retValue.ReasonSelectedItem = new ComboBoxObject()
                            {
                                Id = Convert.ToInt64(enumVal.Value),
                                Value = enumVal.DisplayText
                            };
                        }

                        #endregion get enum val

                        #region get shift

                        shiftUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetShiftById}{retValue.ChangeWorkScheduleModel.ShiftId}"
                        };

                        shift = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(shiftUrl.ToString())).Shift;

                        retValue.ShiftSelectedItem = new ShiftDto()
                        {
                            ShiftId = Convert.ToInt64(retValue.ChangeWorkScheduleModel.ShiftId),
                            Code = (Convert.ToInt64(retValue.ChangeWorkScheduleModel.ShiftId) > 0 ? shift.Code : Constants.OptionOthers)
                        };

                        #endregion get shift

                        retValue.ChangeWorkScheduleModel = new ChangeWorkScheduleModel();
                        PropertyCopier<R.Models.ChangeWorkSchedule, ChangeWorkScheduleModel>.Copy(response.ChangeWorkSchedule, retValue.ChangeWorkScheduleModel);

                        retValue.WorkDate = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.WorkDate);
                        retValue.ScheduleStartTime = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.RequestedStartTime).TimeOfDay;
                        retValue.ScheduleEndTime = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.RequestedEndTime).TimeOfDay;
                        retValue.LunchStartTime = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.LunchBreakStartTime).TimeOfDay;
                        retValue.LunchEndTime = Convert.ToDateTime(retValue.ChangeWorkScheduleModel.LunchBreakEndTime).TimeOfDay;

                        retValue.OriginalSchedule = string.Format("{0} - ({1} - {2})", retValue.OriginalShiftCode,
                            Convert.ToDateTime(retValue.ChangeWorkScheduleModel.WorkScheduleStartTime).ToString(FormHelper.DateFormat),
                            Convert.ToDateTime(retValue.ChangeWorkScheduleModel.WorkScheduleEndTime).ToString(FormHelper.DateFormat));
                    }
                    else
                        throw new Exception(response.ErrorMessage);

                    retValue.IsEnabled = (retValue.ChangeWorkScheduleModel.StatusId == RequestStatusValue.Draft);
                    retValue.ShowCancelButton = (retValue.ChangeWorkScheduleModel.StatusId == RequestStatusValue.ForApproval);
                }
                else
                {
                    retValue.ChangeWorkScheduleModel = new ChangeWorkScheduleModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        WorkDate = DateTime.Now
                    };

                    retValue.IsEnabled = true;

                    retValue = await GetEmployeeSchedule(retValue, 1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<ChangeWorkScheduleHolder> GetEmployeeSchedule(ChangeWorkScheduleHolder form, int option)
        {
            var retValue = form;
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);
            try
            {
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

                    /*
                    var shiftUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetShiftById}{form.ShiftSelectedItem.Id}"
                    };

                    var shift = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(shiftUrl.ToString())).Shift;
                    */
                    var schedule = (await genericRepository_.GetAsync<R.Responses.GetEmployeeWorkScheduleResponse>(request)).Model;
                    PropertyCopier<R.Models.WorkSchedule, ChangeWorkScheduleModel>.Copy(schedule, form.ChangeWorkScheduleModel);

                    retValue.OriginalShiftCode = schedule.Shift.Code;
                    retValue.ChangeWorkScheduleModel.OriginalShiftId = schedule.ShiftId;

                    retValue.ChangeWorkScheduleModel.WorkScheduleStartTime = schedule.StartTime;
                    retValue.ChangeWorkScheduleModel.WorkScheduleEndTime = schedule.EndTime;
                    //retValue.OriginalShiftCode = (string.IsNullOrEmpty(schedule.ShiftCode) ? "Others" : schedule.ShiftCode);

                    retValue.OriginalSchedule = string.Format("{0} - ({1} - {2})", retValue.OriginalShiftCode,
                        Convert.ToDateTime(retValue.ChangeWorkScheduleModel.WorkScheduleStartTime).ToString(FormHelper.DateFormat),
                        Convert.ToDateTime(retValue.ChangeWorkScheduleModel.WorkScheduleEndTime).ToString(FormHelper.DateFormat));

                    /*
                    retValue.OriginalShiftCode = (schedule.ShiftId < 0 ? "Others" : shift.Code);

                    if (Convert.ToBoolean(schedule.FixedSchedule))
                    {
                        if (Convert.ToBoolean(shift.EndTimeNextDay))
                        {
                            var adjustedDate = Convert.ToDateTime(schedule.WorkDate).AdjustTime(Convert.ToDateTime(schedule.EndTime));
                            retValue.ChangeWorkScheduleModel.WorkScheduleEndTime = adjustedDate.AddDays(1);

                            //var adjustedDate = startingDate.AdjustTime(Convert.ToDateTime(entity.EndTime));
                            //entity.EndTime = adjustedDate.AddDays(1);
                        }
                        else if (Convert.ToBoolean(shift.StartTimePreviousDay))
                        {
                            var adjustedDate = Convert.ToDateTime(schedule.WorkDate).AdjustTime(Convert.ToDateTime(schedule.StartTime));
                            retValue.ChangeWorkScheduleModel.WorkScheduleEndTime = adjustedDate.AddDays(-1);

                            //var adjustedDate = startingDate.AdjustTime(Convert.ToDateTime(entity.StartTime));
                            //entity.StartTime = adjustedDate.AddDays(-1);
                        }
                    }
                    else
                    {
                    }
                    */
                }

                //by shift
                if (option == 2)
                {
                    if (form.ShiftSelectedItem.ShiftId <= 0)
                        form.EnableCustomSched = true;
                    else
                    {
                        var shiftUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetShiftById}{form.ShiftSelectedItem.ShiftId}"
                        };

                        var shift = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(shiftUrl.ToString())).Shift;
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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<ChangeWorkScheduleHolder> SubmitRequest(ChangeWorkScheduleHolder form)
        {
            var retValue = form;
            var errors = new List<int>();
            retValue.ErrorReason = false;
            retValue.ErrorRequestedEndTime = false;
            retValue.ErrorRequestedStartTime = false;
            retValue.ErrorShift = false;
            retValue.ErrorWorkDate = false;

            if (retValue.ShiftSelectedItem.ShiftId == 0)
            {
                retValue.ErrorShift = true;
                errors.Add(1);
            }

            if (retValue.ChangeWorkScheduleModel.Reason == 0)
            {
                retValue.ErrorReason = true;
                errors.Add(1);
            }

            if (retValue.ReasonSelectedItem.Id == 0)
            {
                retValue.ErrorReason = true;
                errors.Add(1);
            }

            if (retValue.ScheduleStartTime == Constants.NullDate.TimeOfDay || retValue.ScheduleEndTime == Constants.NullDate.TimeOfDay)
            {
                retValue.ErrorRequestedStartTime = true;
                retValue.ErrorRequestedEndTime = true;
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        try
                        {
                            retValue.ChangeWorkScheduleModel.Reason = (short)form.ReasonSelectedItem.Id;
                            retValue.ChangeWorkScheduleModel.ShiftId = form.ShiftSelectedItem.ShiftId;
                            retValue.ChangeWorkScheduleModel.WorkDate = form.WorkDate;

                            if (form.ShiftSelectedItem.ShiftId < 0)
                            {
                                retValue.ChangeWorkScheduleModel.RequestedStartTime = form.ChangeWorkScheduleModel.WorkDate + form.ScheduleStartTime;
                                retValue.ChangeWorkScheduleModel.RequestedEndTime = form.ChangeWorkScheduleModel.WorkDate + form.ScheduleEndTime;
                                retValue.ChangeWorkScheduleModel.LunchBreakStartTime = form.ChangeWorkScheduleModel.WorkDate + form.LunchStartTime;
                                retValue.ChangeWorkScheduleModel.LunchBreakEndTime = form.ChangeWorkScheduleModel.WorkDate + form.LunchEndTime;
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

                    retValue.EmployeeName = response.EmployeeInfo.EmployeeName;
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

        public async Task<ChangeWorkScheduleHolder> WorkflowTransactionRequest(ChangeWorkScheduleHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.ChangeWorkScheduleModel.ChangeWorkScheduleId, TransactionType.ChangeWorkSchedule);
                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.ChangeWorkScheduleModel.ChangeWorkScheduleId,
                                             TransactionType.ChangeWorkSchedule,
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
    }
}