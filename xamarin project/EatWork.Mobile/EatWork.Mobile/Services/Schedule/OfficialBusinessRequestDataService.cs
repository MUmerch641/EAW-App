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
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class OfficialBusinessRequestDataService : IOfficialBusinessRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;

        public OfficialBusinessRequestDataService(IGenericRepository genericRepository,
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

        public async Task<OfficialBusinessHolder> InitForm(long recordId, int obTypeId, DateTime? selectedDate)
        {
            var retValue = new OfficialBusinessHolder()
            {
                OBApplyToSelectedItem = new ComboBoxObject(),
                OBReasonSelectedItem = new ComboBoxObject(),
                OBReason = new ObservableCollection<ComboBoxObject>(),
                OBApplyTo = new ObservableCollection<ComboBoxObject>()
            };
            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();

            retValue.StartCheckboxEnabled = true;
            retValue.EndCheckboxEnabled = true;

            try
            {
                var enumType = (obTypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessType : EnumValues.TimeOffType);
                var applyAgainst = (obTypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessApplyAgainst : EnumValues.TimeOffApplyAgainst);

                var enumTypeUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{enumType}"
                };

                var applyAgainstUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{applyAgainst}"
                };

                var reason = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumTypeUrl.ToString());
                var applyTo = await genericRepository_.GetAsync<List<R.Models.Enums>>(applyAgainstUrl.ToString());

                foreach (var item in reason)
                {
                    if (!string.IsNullOrWhiteSpace(item.DisplayText))
                        retValue.OBReason.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });
                }

                foreach (var item in applyTo)
                {
                    if (!string.IsNullOrWhiteSpace(item.DisplayText))
                        retValue.OBApplyTo.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });
                }

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetOfficialBusinessRequestDetail}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.OfficialBusinessRequestDetailResponse>(builder.ToString());
                    if (response.IsSuccess)
                    {
                        retValue.OfficialBusinessModel = new OfficialBusinessModel();

                        PropertyCopier<R.Models.OfficialBusiness, OfficialBusinessModel>.Copy(response.OfficialBusiness, retValue.OfficialBusinessModel);
                        PropertyCopier<R.Models.OfficialBusiness, OfficialBusinessHolder>.Copy(response.OfficialBusiness, retValue);

                        retValue.OBStartDate = response.OfficialBusiness.OfficialBusinessDate;
                        retValue.OBEndDate = response.OfficialBusiness.OfficialBusinessDate;
                        retValue.StartTime = Convert.ToDateTime(response.OfficialBusiness.StartTime).TimeOfDay;
                        retValue.EndTime = Convert.ToDateTime(response.OfficialBusiness.EndTime).TimeOfDay;

                        if (Convert.ToDateTime(retValue.OfficialBusinessModel.StartTime) != Constants.NullDate)
                        {
                            retValue.StartTimeString = Convert.ToDateTime(retValue.OfficialBusinessModel.StartTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        if (Convert.ToDateTime(retValue.OfficialBusinessModel.EndTime) != Constants.NullDate)
                        {
                            retValue.EndTimeString = Convert.ToDateTime(retValue.OfficialBusinessModel.EndTime).ToString(Constants.TimeFormatHHMMTT);
                        }
                    }
                    else
                        throw new Exception(response.ErrorMessage);

                    retValue.IsEnabled = (retValue.OfficialBusinessModel.StatusId == RequestStatusValue.Draft);
                    retValue.ShowCancelButton = (retValue.OfficialBusinessModel.StatusId == RequestStatusValue.ForApproval);
                }
                else
                {
                    retValue.OfficialBusinessModel = new OfficialBusinessModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        TypeId = Convert.ToByte(obTypeId)
                    };
                    retValue.OBStartDate = selectedDate ?? DateTime.Now.Date;
                    retValue.OBEndDate = selectedDate ?? DateTime.Now.Date;

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

        public async Task<OfficialBusinessApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new OfficialBusinessApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetOfficialBusinessRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.OfficialBusinessRequestDetailResponse>(builder.ToString());
                if (response.IsSuccess)
                {
                    var enumType = (response.OfficialBusiness.TypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessType : EnumValues.TimeOffType);
                    var applyAgainst = (response.OfficialBusiness.TypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessApplyAgainst : EnumValues.TimeOffApplyAgainst);

                    var enumTypeUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{enumType}/{response.OfficialBusiness.OBTypeId}"
                    };

                    var applyAgainstUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{applyAgainst}/{response.OfficialBusiness.ApplyTo}"
                    };

                    var reason = await genericRepository_.GetAsync<R.Models.Enums>(enumTypeUrl.ToString());
                    var applyTo = await genericRepository_.GetAsync<R.Models.Enums>(applyAgainstUrl.ToString());

                    retValue.OfficialBusinessModel = new OfficialBusinessModel();

                    PropertyCopier<R.Models.OfficialBusiness, OfficialBusinessModel>.Copy(response.OfficialBusiness, retValue.OfficialBusinessModel);
                    PropertyCopier<R.Models.OfficialBusiness, OfficialBusinessApprovalHolder>.Copy(response.OfficialBusiness, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = Convert.ToDateTime(response.OfficialBusiness.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.OBDate = Convert.ToDateTime(response.OfficialBusiness.OfficialBusinessDate).ToString(FormHelper.DateFormat);
                    retValue.OBTime = string.Empty.ConcatDateTime((DateTime)response.OfficialBusiness.OfficialBusinessDate,
                                       (DateTime)response.OfficialBusiness.StartTime,
                                       (DateTime)response.OfficialBusiness.EndTime);

                    retValue.OBHours = string.Format("{0:0.00} hrs", response.OfficialBusiness.NoOfHours);
                    retValue.Reason = reason.DisplayText;
                    retValue.ApplyAgainst = applyTo.DisplayText;

                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }

                    if (retValue.OfficialBusinessModel.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
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

        public async Task<OfficialBusinessApprovalHolder> WorkflowTransaction(OfficialBusinessApprovalHolder form)
        {
            form.IsSuccess = false;

            var confirm = await workflowDataService_.ConfirmationMessage(form.SelectedWorkflowAction.ActionTriggeredId, form.SelectedWorkflowAction.ActionMessage);
            if (confirm.Continue)
            {
                using (UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);

                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);

                    var response = new R.Responses.WFTransactionResponse();

                    if (form.OfficialBusinessModel.StatusId == RequestStatusValue.Approved && form.SelectedWorkflowAction.ActionTriggeredId == ActionTypeId.Cancel)
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
                        var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                        {
                            Path = $"{ApiConstants.OfficialBusinessApi}/update-wfsource"
                        };

                        await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.OfficialBusinessModel.OfficialBusinessId);

                        form.IsSuccess = response.IsSuccess;
                        FormSession.MyApprovalSelectedItemUpdated = response.IsSuccess;
                        FormSession.MyApprovalSelectedItemStatus = FormSession.SetDefaultMyApprovalSelectedItemStatus(form.SelectedWorkflowAction.ActionType);
                    }
                }
            }

            return form;
        }

        public async Task<OfficialBusinessHolder> WorkflowTransactionRequest(OfficialBusinessHolder form)
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
                        var transactionType = (form.OfficialBusinessModel.TypeId == Constants.OfficialBusiness ? TransactionType.OfficialBusiness : TransactionType.TimeOff);

                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.OfficialBusinessModel.OfficialBusinessId, transactionType);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(form.OfficialBusinessModel.OfficialBusinessId,
                                             transactionType,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.OfficialBusinessApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.OfficialBusinessModel.OfficialBusinessId);

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

        public async Task<OfficialBusinessHolder> SubmitRequest(OfficialBusinessHolder form)
        {
            var errors = new List<int>();
            form.ErrorOBReason = false;
            form.ErrorOBStartDate = false;
            form.ErrorRemarks = false;
            form.ErrorNoOfHours = false;
            form.ErrorOBApplyTo = false;

            if (form.OfficialBusinessModel.NoOfHours == 0)
            {
                form.ErrorNoOfHours = true;
                errors.Add(1);
            }

            if (form.OBReasonSelectedItem.Id == 0)
            {
                form.ErrorOBReason = true;
                errors.Add(1);
            }

            if (string.IsNullOrWhiteSpace(form.OfficialBusinessModel.Remarks))
            {
                form.ErrorRemarks = true;
                errors.Add(1);
            }

            if (form.OBApplyToSelectedItem.Id == 0)
            {
                form.ErrorOBApplyTo = true;
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                {
                    try
                    {
                        form.OfficialBusinessModel.OfficialBusinessDate = form.OBStartDate;

                        /*
                        retValue.OfficialBusinessModel.StartTime = form.OBStartDate + form.StartTime;
                        retValue.OfficialBusinessModel.EndTime = form.OBStartDate + form.EndTime;
                        */

                        var startime = Convert.ToDateTime(form.OBStartDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.StartTimeString);
                        var endtime = Convert.ToDateTime(form.OBStartDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.EndTimeString);

                        form.OfficialBusinessModel.StartTime = startime;
                        form.OfficialBusinessModel.EndTime = endtime;

                        form.OfficialBusinessModel.OBTypeId = form.OBReasonSelectedItem.Id;
                        form.OfficialBusinessModel.ApplyTo = Convert.ToByte(form.OBApplyToSelectedItem.Id);
                        form.OfficialBusinessModel.OfficialBusinessDate = Convert.ToDateTime(form.OfficialBusinessModel.OfficialBusinessDate.Value.ToString("MM/dd/yyyy"));
                        form.OBStartDate = Convert.ToDateTime(form.OBStartDate.Value.ToString("MM/dd/yyyy"));
                        form.OBEndDate = Convert.ToDateTime(form.OBEndDate.Value.ToString("MM/dd/yyyy"));

                        var response = await GenerateSchedule(form);
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

            return form;
        }

        public async Task<OfficialBusinessHolder> SaveRecord(OfficialBusinessHolder form, R.Requests.SubmitOfficialBusinessRequest request)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);
                var response = new R.Responses.BaseResponse<R.Models.OfficialBusiness>();

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.SubmitOfficialBusinessRequest
                };

                //OB
                if (form.OfficialBusinessModel.TypeId == 1)
                {
                    if (request.OfficialBusinessToSave.Count > 0)
                    {
                        using (UserDialogs.Instance.Loading())
                        {
                            await Task.Delay(500);
                            response = await genericRepository_.PostAsync<R.Requests.SubmitOfficialBusinessRequest, R.Responses.BaseResponse<R.Models.OfficialBusiness>>(builder.ToString(), request);
                        }
                    }
                    else
                        await dialogService_.AlertAsync("No record/s to save");
                }
                else
                {
                    var param = new R.Requests.SubmitOfficialBusinessRequest()
                    {
                        Data = request.Data,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                    };

                    response = await genericRepository_.PostAsync<R.Requests.SubmitOfficialBusinessRequest, R.Responses.BaseResponse<R.Models.OfficialBusiness>>(builder.ToString(), param);
                }

                if (response.Model != null)
                {
                    /*var moduleFormId = (form.OfficialBusinessModel.TypeId == Constants.TimeOff ? ModuleForms.TimeOffRequest : ModuleForms.OfficialBusiness);*/
                    var typeString = (form.OfficialBusinessModel.TypeId == Constants.TimeOff ? "TIMEOFF" : "OB");

                    /*
                    if (!string.IsNullOrEmpty(form.FileData.FileName))
                        await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, moduleFormId, response.Model.OfficialBusinessId);
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
                                    FileTags = typeString,
                                    FileType = x.FileType,
                                    MimeType = x.MimeType,
                                    RawFileSize = x.RawFileSize,
                                    ModuleFormId = ModuleForms.OfficialBusiness,
                                    TransactionId = response.Model.OfficialBusinessId,
                                })
                            );

                        await commonDataService_.SaveFileAttachmentsAsync(files);
                    }

                    #endregion save file attachments

                    form.Success = true;
                    FormSession.IsSubmitted = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return form;
        }

        private async Task<R.Responses.ValidateOfficialBusinessResponse> GenerateSchedule(OfficialBusinessHolder form, R.Responses.ValidateOfficialBusinessResponse request = null)
        {
            //var response = (request ?? new R.Responses.ValidateOfficialBusinessResponse());
            var response = new R.Responses.ValidateOfficialBusinessResponse();

            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            var builder = new UriBuilder(url)
            {
                Path = ApiConstants.ValidateOBRequest
            };

            var data = new R.Models.OfficialBusiness();
            PropertyCopier<OfficialBusinessModel, R.Models.OfficialBusiness>.Copy(form.OfficialBusinessModel, data);

            var param = new R.Requests.SubmitOfficialBusinessRequest()
            {
                Data = data,
                StartDate = (DateTime)form.OBStartDate,
                EndDate = (DateTime)(data.TypeId == 1 ? form.OBEndDate : form.OBStartDate),
                OverrideNoWorkSchedule = form.OverrideSchedule,
                SkipHolidayRequest = form.SkipHolidays,
                SkipRestdayRequest = form.SkipRestdays,
                IncludePreFilingDates = form.IncludePreFilingDates,
            };

            using (UserDialogs.Instance.Loading())
            {
                await Task.Delay(500);
                response = await genericRepository_.PostAsync<R.Requests.SubmitOfficialBusinessRequest, R.Responses.ValidateOfficialBusinessResponse>(builder.ToString(), param);
            }

            if (response.ValidationMessages.Count > 0)
            {
                if (response.Request.IsResubmit || response.Request.IsPostFiling)
                {
                    var list = new ObservableCollection<string>(response.ValidationMessages);
                    if (await dialogService_.ConfirmDialog2Async(list, Messages.ValidationHeaderMessage))
                    {
                        if (response.Request.IsPostFiling)
                            form.IncludePreFilingDates = true;

                        if (response.Request.IsNoWorkSched)
                            form.OverrideSchedule = true;

                        if (response.Request.IsRestday)
                            form.SkipRestdays = true;

                        if (response.Request.IsHoliday)
                            form.SkipHolidays = true;

                        await GenerateSchedule(form, response);
                    }
                }
                else
                {
                    var colletion = new ObservableCollection<string>(response.ValidationMessages);
                    var page = new EatWork.Mobile.Views.Shared.ErrorPage(colletion, Messages.ValidationHeaderMessage);
                    await Application.Current.MainPage.Navigation.PushModalAsync(page);
                }
            }
            else
                await SaveRecord(form, response.Request);

            return response;
        }
    }
}