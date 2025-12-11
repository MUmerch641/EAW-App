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
    public class SpecialWorkScheduleDataService : ISpecialWorkScheduleDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly StringHelper string_;

        public SpecialWorkScheduleDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            StringHelper url)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            string_ = url;
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
                    retValue.ShiftList.Add(new ShiftDto() { ShiftId = Convert.ToInt64(item.ShiftId), Code = item.Code });

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

                        retValue.ScheduleStartTime = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.StartTime).TimeOfDay;
                        retValue.ScheduleEndTime = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.EndTime).TimeOfDay;
                        retValue.LunchStartTime = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.LunchBreakStartTime).TimeOfDay;
                        retValue.LunchEndTime = Convert.ToDateTime(retValue.SpecialWorkScheduleRequestModel.LunchBreakEndTime).TimeOfDay;
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

                    retValue.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<SpecialWorkScheduleHolder> GetShiftSchedule(SpecialWorkScheduleHolder form)
        {
            var retValue = form;
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            if (retValue.ShiftSelectedItem.ShiftId < 0)
                retValue.EnableCustomSched = true;
            else
            {
                var shiftUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetShiftById}{form.ShiftSelectedItem.ShiftId}"
                };

                var shift = (await genericRepository_.GetAsync<R.Responses.ShiftDetailResponse>(shiftUrl.ToString())).Shift;
                PropertyCopier<R.Models.Shift, SpecialWorkScheduleRequestModel>.Copy(shift, form.SpecialWorkScheduleRequestModel);

                form.ScheduleStartTime = Convert.ToDateTime(shift.StartTime).TimeOfDay;
                form.ScheduleEndTime = Convert.ToDateTime(shift.EndTime).TimeOfDay;
                form.LunchStartTime = Convert.ToDateTime(shift.LunchBreakStartTime).TimeOfDay;
                form.LunchStartTime = Convert.ToDateTime(shift.LunchBreakStartTime).TimeOfDay;
                form.LunchEndTime = Convert.ToDateTime(shift.LunchBreakEndTime).TimeOfDay;

                retValue.EnableCustomSched = false;
            }

            return retValue;
        }

        public async Task<SpecialWorkScheduleHolder> SubmitRequest(SpecialWorkScheduleHolder form)
        {
            var retValue = form;
            var errors = new List<int>();
            retValue.ErrorReason = false;
            retValue.ErrorShift = false;
            retValue.ErrorWorkDate = false;

            if (string.IsNullOrWhiteSpace(form.ShiftSelectedItem.Code))
            {
                retValue.ErrorShift = true;
                errors.Add(1);
            }

            if (form.WorkDate == Constants.NullDate)
            {
                retValue.ErrorWorkDate = true;
                errors.Add(1);
            }

            if (string.IsNullOrWhiteSpace(form.SpecialWorkScheduleRequestModel.Reason))
            {
                retValue.ErrorReason = true;
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        try
                        {
                            retValue.SpecialWorkScheduleRequestModel.WorkDate = form.WorkDate;
                            retValue.SpecialWorkScheduleRequestModel.ShiftId = form.ShiftSelectedItem.ShiftId;

                            if (form.ShiftSelectedItem.ShiftId < 0)
                            {
                                retValue.SpecialWorkScheduleRequestModel.StartTime = form.WorkDate + form.ScheduleStartTime;
                                retValue.SpecialWorkScheduleRequestModel.EndTime = form.WorkDate + form.ScheduleEndTime;
                                retValue.SpecialWorkScheduleRequestModel.LunchBreakStartTime = form.WorkDate + form.LunchStartTime;
                                retValue.SpecialWorkScheduleRequestModel.LunchBreakEndTime = form.WorkDate + form.LunchEndTime;
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
                    retValue.SpecialWorkScheduleRequestModel = new SpecialWorkScheduleRequestModel();

                    PropertyCopier<R.Models.WorkScheduleRequest, SpecialWorkScheduleRequestModel>.Copy(response.WorkScheduleRequest, retValue.SpecialWorkScheduleRequestModel);
                    PropertyCopier<R.Models.WorkScheduleRequest, SpecialWorkScheduleApprovalHolder>.Copy(response.WorkScheduleRequest, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.EmployeeName;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = response.WorkScheduleRequest.DateFiledDisplay;
                    retValue.RequestType = response.WorkScheduleRequest.RequestTypeDisplay;
                    retValue.WorkDate = response.WorkScheduleRequest.WorkDateDisplay;
                    retValue.ShiftCode = response.WorkScheduleRequest.ShiftCode;
                    retValue.LunchSchedule = response.WorkScheduleRequest.LunchSchedule;
                    retValue.WorkingHours = response.WorkScheduleRequest.WorkingHoursDisplay;
                    retValue.LunchSchedule = response.WorkScheduleRequest.LunchSchedule;
                    retValue.LunchDuration = response.WorkScheduleRequest.LunchDurationDisplay;

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
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<SpecialWorkScheduleApprovalHolder> WorkflowTransaction(SpecialWorkScheduleApprovalHolder form)
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

                        var formData = JsonConvert.SerializeObject(new
                        {
                            txtAllowFlexiTimeLimit = 0,
                            txtAllowEarlyTimeInLimit = 0,
                            chkEarlyTimeInOvertime_String = "false",
                            dtpOffsettingExpirationDate = Constants.NullDate.ToString(Constants.DateFormatMMDDYYYY)
                        });

                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.TransactionId,
                            form.TransactionTypeId,
                            form.SelectedWorkflowAction.ActionTriggeredId,
                            confirm.ResponseText,
                            form.StageId,
                            formData);

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

        public async Task<SpecialWorkScheduleHolder> WorkflowTransactionRequest(SpecialWorkScheduleHolder form)
        {
            var retValue = form;
            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.ActionTypeId, form.Msg);
                if (confirm.Continue)
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.SpecialWorkScheduleRequestModel.WorkScheduleRequestId, TransactionType.WorkScheduleRequest);
                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.SpecialWorkScheduleRequestModel.WorkScheduleRequestId,
                                             TransactionType.WorkScheduleRequest,
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

        public async Task<SpecialWorkScheduleHolder> GetDateSchedule(SpecialWorkScheduleHolder form)
        {
            throw new NotImplementedException();
        }
    }
}