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
    public class UndertimeRequestDataService : IUndertimeRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;

        public UndertimeRequestDataService(IGenericRepository genericRepository,
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

        public async Task<UndertimeHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new UndertimeHolder()
            {
                UndertimeReasonSelectedItem = new ComboBoxObject(),
                UndertimeReason = new ObservableCollection<ComboBoxObject>()
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();

            try
            {
                var enumUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{EnumValues.UndertimeType}"
                };

                var enums = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                foreach (var item in enums)
                    retValue.UndertimeReason.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetUndertimeRequestDetail}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.UndertimeRequestDetailResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        retValue.UndertimeModel = new UndertimeModel();

                        PropertyCopier<R.Models.Undertime, UndertimeModel>.Copy(response.Undertime, retValue.UndertimeModel);
                        PropertyCopier<R.Models.Undertime, UndertimeHolder>.Copy(response.Undertime, retValue);

                        retValue.DepartureTime = Convert.ToDateTime(retValue.UndertimeModel.DepartureTime).TimeOfDay;
                        retValue.ArrivalTime = Convert.ToDateTime(retValue.UndertimeModel.ArrivalTime).TimeOfDay;

                        if (Convert.ToDateTime(retValue.UndertimeModel.DepartureTime) != Constants.NullDate)
                        {
                            retValue.StartTimeString = Convert.ToDateTime(retValue.UndertimeModel.DepartureTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        if (Convert.ToDateTime(retValue.UndertimeModel.ArrivalTime) != Constants.NullDate)
                        {
                            retValue.EndTimeString = Convert.ToDateTime(retValue.UndertimeModel.ArrivalTime).ToString(Constants.TimeFormatHHMMTT);
                        }

                        retValue.IsEnabled = (retValue.UndertimeModel.StatusId == RequestStatusValue.Draft);
                        retValue.ShowCancelButton = (retValue.UndertimeModel.StatusId == RequestStatusValue.ForApproval);
                    }
                }
                else
                {
                    retValue.UndertimeModel = new UndertimeModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        UTHrs = 0,
                        UndertimeDate = selectedDate ?? DateTime.Now.Date,
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

        public async Task<UndertimeHolder> SubmitRequest(UndertimeHolder form)
        {
            var errors = new List<int>();
            form.ErrorReason = false;
            form.ErrorUndertimeDate = false;
            form.ErrorUndertimeReason = false;
            form.ErrorUTHrs = false;

            if (form.UndertimeModel.UTHrs == 0)
            {
                form.ErrorUTHrs = true;
                errors.Add(1);
            }

            if (form.UndertimeReasonSelectedItem.Id == 0)
            {
                form.ErrorUndertimeReason = true;
                errors.Add(1);
            }

            if (string.IsNullOrWhiteSpace(form.UndertimeModel.Reason))
            {
                form.ErrorReason = true;
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
                            form.UndertimeModel.UndertimeTypeId = (short)form.UndertimeReasonSelectedItem.Id;
                            /*
                            form.UndertimeModel.DepartureTime = Convert.ToDateTime(form.UndertimeModel.UndertimeDate).Date + form.DepartureTime;
                            form.UndertimeModel.ArrivalTime = Convert.ToDateTime(form.UndertimeModel.UndertimeDate).Date + form.ArrivalTime;
                            */

                            var startime = Convert.ToDateTime(form.UndertimeModel.UndertimeDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.StartTimeString);
                            var endtime = Convert.ToDateTime(form.UndertimeModel.UndertimeDate.Value.Date.ToString("MM/dd/yyyy") + " " + form.EndTimeString);
                            //endtime = endtime.AddDays((endtime <= startime ? 1 : 0));

                            form.UndertimeModel.DepartureTime = (!string.IsNullOrWhiteSpace(form.StartTimeString) ? startime : Constants.NullDate);
                            form.UndertimeModel.ArrivalTime = (!string.IsNullOrWhiteSpace(form.EndTimeString) ? endtime : Constants.NullDate);

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitUndertimeRequest
                            };

                            var data = new R.Models.Undertime();
                            PropertyCopier<UndertimeModel, R.Models.Undertime>.Copy(form.UndertimeModel, data);

                            var param = new R.Requests.SubmitUndertimeRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitUndertimeRequest, R.Responses.BaseResponse<R.Models.Undertime>>(builder.ToString(), param);

                            if (response.Model.UndertimeId > 0)
                            {
                                /*
                                if (!string.IsNullOrEmpty(form.FileData.FileName))
                                    await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.UndertimeRequest, response.Model.UndertimeId);
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
                                                FileTags = "UNDERTIME",
                                                FileType = x.FileType,
                                                MimeType = x.MimeType,
                                                RawFileSize = x.RawFileSize,
                                                ModuleFormId = ModuleForms.UndertimeRequest,
                                                TransactionId = response.Model.UndertimeId,
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
                    }
                }
            }

            return form;
        }

        public async Task<UndertimeApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new UndertimeApprovalHolder();
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetUndertimeRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.UndertimeRequestDetailResponse>(builder.ToString());
                if (response.IsSuccess)
                {
                    var enumUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{EnumValues.UndertimeType}/{response.Undertime.UndertimeTypeId}"
                    };

                    var enumVal = await genericRepository_.GetAsync<R.Models.Enums>(enumUrl.ToString());

                    retValue.UndertimeModel = new UndertimeModel();

                    PropertyCopier<R.Models.Undertime, UndertimeModel>.Copy(response.Undertime, retValue.UndertimeModel);
                    PropertyCopier<R.Models.Undertime, UndertimeApprovalHolder>.Copy(response.Undertime, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = Convert.ToDateTime(response.Undertime.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.UndertimeDate = Convert.ToDateTime(response.Undertime.UndertimeDate).ToString(FormHelper.DateFormat);
                    retValue.UndertimeHours = string.Format("{0:0.00} hrs", response.Undertime.UTHrs);
                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;
                    retValue.TimeRange = string.Empty.ConcatDateTime((DateTime)response.Undertime.UndertimeDate,
                        (DateTime)response.Undertime.DepartureTime,
                        (DateTime)response.Undertime.ArrivalTime);

                    retValue.Reason = enumVal.DisplayText;
                    retValue.Remarks = response.Undertime.Reason;

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }

                    if (retValue.UndertimeModel.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
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

        public async Task<UndertimeApprovalHolder> WorkflowTransaction(UndertimeApprovalHolder form)
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

                        if (form.UndertimeModel.StatusId == RequestStatusValue.Approved && form.SelectedWorkflowAction.ActionTriggeredId == ActionTypeId.Cancel)
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
                                Path = $"{ApiConstants.UndertimeApi}/update-wfsource"
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

        public async Task<UndertimeHolder> WorkflowTransactionRequest(UndertimeHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.UndertimeModel.UndertimeId, TransactionType.Undertime);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(form.UndertimeModel.UndertimeId,
                                             TransactionType.Undertime,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.UndertimeApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.UndertimeModel.UndertimeId);

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