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
    public class ChangeRestdayScheduleDataService : IChangeRestdayScheduleDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;
        private readonly StringHelper string_;

        public ChangeRestdayScheduleDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            IDialogService dialogService,
            StringHelper stringHelper)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            dialogService_ = dialogService;
            string_ = stringHelper;
            employeeProfileDataService_ = AppContainer.Resolve<IEmployeeProfileDataService>();
        }

        public async Task<ChangeRestdayHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new ChangeRestdayHolder
            {
                RestDayList = new ObservableCollection<ChangeRestday>(),
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();

            try
            {
                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetChangeRestdayRequestDetail}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.ChangeRestdayRequestDetailResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        var employee = new R.Responses.BaseResponse<R.Models.EmployeeInformation>();

                        if (response.ChangeRestDay.SwapWithProfileId > 0)
                        {
                            builder = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.GetEmployeeById}{response.ChangeRestDay.SwapWithProfileId}"
                            };

                            employee = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.EmployeeInformation>>(builder.ToString());
                        }

                        retValue.ChangeRestdayModel = new ChangeRestdayModel();

                        PropertyCopier<R.Models.ChangeRestDay, ChangeRestdayModel>.Copy(response.ChangeRestDay, retValue.ChangeRestdayModel);
                        PropertyCopier<R.Models.ChangeRestDay, ChangeRestdayHolder>.Copy(response.ChangeRestDay, retValue);

                        retValue.SwapWith = (response.ChangeRestDay.SwapWithProfileId > 0 ? employee.Model.FullNameMiddleInitialOnly : "");
                    }
                    else
                        throw new Exception(response.ErrorMessage);

                    retValue.IsEnabled = (retValue.ChangeRestdayModel.StatusId == RequestStatusValue.Draft);
                    retValue.ShowCancelButton = (retValue.ChangeRestdayModel.StatusId == RequestStatusValue.ForApproval);
                }
                else
                {
                    retValue.ChangeRestdayModel = new ChangeRestdayModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        RequestDate = selectedDate ?? DateTime.Now.Date,
                        RestDayDate = selectedDate ?? DateTime.Now.Date
                    };

                    retValue.IsEnabled = true;
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
            finally
            {
                FormSession.IsMySchedule = false;
            }

            return retValue;
        }

        public async Task<ChangeRestdayHolder> GetEmployeeSchedule(ChangeRestdayHolder form)
        {
            form.IsContinue = true;
            form.ErrorOriginalDate = false;
            form.ErrorRequestedDate = false;

            try
            {
                if (form.ChangeRestdayModel.ChangeRestDayId == 0)
                {
                    var response = new R.Responses.ListResponse<R.Models.ChangeRestDayDetailList>();
                    var userInfo = PreferenceHelper.UserInfo();
                    var url = await commonDataService_.RetrieveClientUrl();

                    var builder = new UriBuilder(url)
                    {
                        Path = ApiConstants.GetChangeRestdayScheduleList
                    };

                    var param = new R.Requests.GetChangeRestDayDetailListRequest
                    {
                        ProfileId = userInfo.ProfileId,
                        StartDate = form.ChangeRestdayModel.RestDayDate.Value,
                        EndDate = form.ChangeRestdayModel.RestDayDate.Value,
                        SwapWith = form.ChangeRestdayModel.SwapWithProfileId.Value,
                    };

                    var request = string_.CreateUrl<R.Requests.GetChangeRestDayDetailListRequest>(builder.ToString(), param);

                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.ChangeRestDayDetailList>>(request);
                    }

                    if (response.ListData.Count == 0)
                    {
                        var data = response.ListData.FirstOrDefault();

                        if (data == null)
                        {
                            form.ErrorOriginalDate = true;
                            form.ErrorOriginalDateMessage = Messages.NoRestdaySchedule;
                            form.IsContinue = false;
                        }
                        else
                        {
                            foreach (var item in response.ListData)
                            {
                                form.ChangeRestDayDetailList.Add(new ChangeRestDayDetailList()
                                {
                                    DayOfWeek = item.DayOfWeek,
                                    DayOfWeekRequest = item.DayOfWeekRequest,
                                    HasAttendance = item.HasAttendance,
                                    RequestDate = item.RequestDate,
                                    RestDayDate = item.RestDayDate,
                                    RowId = item.RowId
                                });
                            }
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

        public async Task<ChangeRestdayHolder> SubmitRequest(ChangeRestdayHolder form)
        {
            var errors = new List<int>();
            form.ErrorReason = false;

            if (string.IsNullOrWhiteSpace(form.ChangeRestdayModel.Reason))
            {
                form.ErrorReason = true;
                errors.Add(1);
            }

            if (form.ChangeRestdayModel.RestDayDate <= Convert.ToDateTime(Constants.NullDate))
            {
                form.ErrorOriginalDate = true;
                errors.Add(1);
            }

            if (form.ChangeRestdayModel.RequestDate <= Convert.ToDateTime(Constants.NullDate))
            {
                form.ErrorRequestedDate = true;
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

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitChangeRestdayRequest
                            };

                            var detailsList = new List<R.Models.ChangeRestDayDetailList>();
                            foreach (var item in form.ChangeRestDayDetailList)
                            {
                                detailsList.Add(new R.Models.ChangeRestDayDetailList()
                                {
                                    DayOfWeek = item.DayOfWeek,
                                    DayOfWeekRequest = item.DayOfWeekRequest,
                                    HasAttendance = item.HasAttendance,
                                    RequestDate = item.RequestDate,
                                    RestDayDate = item.RestDayDate,
                                    RowId = item.RowId
                                });
                            }

                            var data = new R.Models.ChangeRestDay();
                            PropertyCopier<ChangeRestdayModel, R.Models.ChangeRestDay>.Copy(form.ChangeRestdayModel, data);

                            var param = new R.Requests.SubmitChangRestdayRequest()
                            {
                                Data = data,
                                ChangeRestDayDetailList = detailsList
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitChangRestdayRequest, R.Responses.BaseResponse<R.Models.ChangeRestDay>>(builder.ToString(), param);

                            if (response.Model.ChangeRestDayId > 0)
                            {
                                /*
                                if (!string.IsNullOrEmpty(form.FileData.FileName))
                                    await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.ChangeRestDaySchedule_Schedule, response.Model.ChangeRestDayId);
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
                                                FileTags = "CHANGE RESTDAY",
                                                FileType = x.FileType,
                                                MimeType = x.MimeType,
                                                RawFileSize = x.RawFileSize,
                                                ModuleFormId = ModuleForms.ChangeRestDaySchedule_Schedule,
                                                TransactionId = response.Model.ChangeRestDayId,
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
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }

            return form;
        }

        public async Task<ChangeRestdayScheduleApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new ChangeRestdayScheduleApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetChangeRestdayRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.ChangeRestdayRequestDetailResponse>(builder.ToString());

                if (response.IsSuccess)
                {
                    var employee = new R.Responses.BaseResponse<R.Models.EmployeeInformation>();

                    if (response.ChangeRestDay.SwapWithProfileId > 0)
                    {
                        builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEmployeeById}{response.ChangeRestDay.SwapWithProfileId}"
                        };

                        employee = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.EmployeeInformation>>(builder.ToString());
                    }

                    retValue.ChangeRestdayModel = new ChangeRestdayModel();

                    PropertyCopier<R.Models.ChangeRestDay, ChangeRestdayModel>.Copy(response.ChangeRestDay, retValue.ChangeRestdayModel);
                    PropertyCopier<R.Models.ChangeRestDay, ChangeRestdayScheduleApprovalHolder>.Copy(response.ChangeRestDay, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = Convert.ToDateTime(response.ChangeRestDay.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.RestdayDate = Convert.ToDateTime(response.ChangeRestDay.RestDayDate).ToString(FormHelper.DateFormat);
                    retValue.RequestedDate = Convert.ToDateTime(response.ChangeRestDay.RequestDate).ToString(FormHelper.DateFormat);
                    retValue.SwapWithEmployeeName = (response.ChangeRestDay.SwapWithProfileId > 0 ? employee.Model.FullNameMiddleInitialOnly : "");

                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }

                    if (retValue.ChangeRestdayModel.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
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

        public async Task<ChangeRestdayScheduleApprovalHolder> WorkflowTransaction(ChangeRestdayScheduleApprovalHolder form)
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

                        if (form.ChangeRestdayModel.StatusId == RequestStatusValue.Approved && form.SelectedWorkflowAction.ActionTriggeredId == ActionTypeId.Cancel)
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
                                Path = $"{ApiConstants.ChangeRestdayApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.ChangeRestdayModel.ChangeRestDayId);

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

        public async Task<ChangeRestdayHolder> WorkflowTransactionRequest(ChangeRestdayHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.ChangeRestdayModel.ChangeRestDayId, TransactionType.ChangeRestDay);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(form.ChangeRestdayModel.ChangeRestDayId,
                                             TransactionType.ChangeRestDay,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.ChangeRestdayApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.ChangeRestdayModel.ChangeRestDayId);

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
    }
}