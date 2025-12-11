using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Models.FormHolder.TravelRequest;
using EatWork.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class TravelRequestService : ITravelRequestDataService
    {
        private readonly IDialogService dialogService_;
        private readonly ICommonDataService commonService_;
        private readonly IGenericRepository genericRepository_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;

        public TravelRequestService()
        {
            dialogService_ = AppContainer.Resolve<IDialogService>();
            commonService_ = AppContainer.Resolve<ICommonDataService>();
            genericRepository_ = AppContainer.Resolve<IGenericRepository>();
            workflowDataService_ = AppContainer.Resolve<IWorkflowDataService>();
            employeeProfileDataService_ = AppContainer.Resolve<IEmployeeProfileDataService>();
        }

        public async Task<TravelRequestHolder> InitForm(long id, DateTime? selectedDate)
        {
            var holder = new TravelRequestHolder();
            try
            {
                var url = await commonService_.RetrieveClientUrl();
                await commonService_.HasInternetConnection(url);
                var user = PreferenceHelper.UserInfo();

                #region enums

                var enumUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{EnumValues.TypeOfBusinessTrip}"
                };

                var enums = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                if (enums.Count > 0)
                {
                    holder.TripTypes = new ObservableCollection<ComboBoxObject>(
                        enums.Select(item => new ComboBoxObject
                        {
                            Id = Convert.ToInt64(item.Value),
                            Value = item.DisplayText,
                        })
                    );
                }

                #endregion enums

                #region origins and destinations and others

                var INIT_BUILDER = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.TravelRequest}/init-form"
                };

                var INIT_RESPONSE = await genericRepository_.GetAsync<R.Responses.InitTravelRequestFormResponse>(INIT_BUILDER.ToString());

                if (INIT_RESPONSE.IsSuccess)
                {
                    if (INIT_RESPONSE.Origins.Count > 0)
                    {
                        holder.Origins = new ObservableCollection<ComboBoxObject>(
                                INIT_RESPONSE.Origins.Select(x => new ComboBoxObject()
                                {
                                    Value = x,
                                    Id = 0
                                })
                            );
                    }

                    if (INIT_RESPONSE.Destinations.Count > 0)
                    {
                        holder.Destinations = new ObservableCollection<ComboBoxObject>(
                                INIT_RESPONSE.Destinations.Select(x => new ComboBoxObject()
                                {
                                    Value = x,
                                    Id = 0
                                })
                            );
                    }

                    holder.ModuleFormId = INIT_RESPONSE.FARModuleFormId;
                }

                #endregion origins and destinations and others

                if (id > 0)
                {
                    var RECORD_BUILDER = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.TravelRequest}/{id}"
                    };

                    var RECORD_RESPONSE = await genericRepository_.GetAsync<R.Responses.TravelRequestResponse>(RECORD_BUILDER.ToString());

                    if (RECORD_RESPONSE.Model != null || RECORD_RESPONSE != null)
                    {
                        holder.DateRequested = RECORD_RESPONSE.Model.RequestDate;
                        holder.Model = RECORD_RESPONSE.Model;
                        holder.Details.Value = RECORD_RESPONSE.Model.Details;
                        holder.SpecialRequestNote = RECORD_RESPONSE.Model.Reason;
                        holder.FirstOrigin.Value = RECORD_RESPONSE.Model.FirstOrigin;
                        holder.FirstDestination.Value = RECORD_RESPONSE.Model.FirstDestination;
                        holder.FirstDepartureDate = RECORD_RESPONSE.Model.FirstDepartureDate.GetValueOrDefault().Date;
                        holder.FirstDepartureTime = RECORD_RESPONSE.Model.FirstDepartureTime.GetValueOrDefault().TimeOfDay;
                        holder.SecondOrigin.Value = RECORD_RESPONSE.Model.SecondOrigin;
                        holder.SecondDestination.Value = RECORD_RESPONSE.Model.SecondDestination;
                        holder.SecondDepartureDate = RECORD_RESPONSE.Model.SecondDepartureDate.GetValueOrDefault().Date;
                        holder.SecondDepartureTime = RECORD_RESPONSE.Model.SecondDepartureTime.GetValueOrDefault().TimeOfDay;
                    }

                    holder.IsEnabled = (holder.Model.StatusId == RequestStatusValue.Draft);
                    holder.ShowCancelButton = (holder.Model.StatusId == RequestStatusValue.ForApproval);

                    holder.IsEditable = Constants.EditableStatusLookup.Split(',')
                                                  .Where(x => string.Compare(holder.Model.StatusId.ToString(), x, true) == 0)
                                                  .Count() > 0;
                }
                else
                {
                    holder.ProfileId = user.ProfileId;
                    holder.IsEnabled = true;
                    holder.IsEditable = true;
                    holder.DateRequested = selectedDate ?? DateTime.Now.Date;
                }

                holder.RequestedBy = $"{user.FirstName} {(!string.IsNullOrWhiteSpace(user.MiddleName) ? user.MiddleName[0] + "." : "")} {user.LastName}";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return holder;
        }

        public async Task<TravelRequestHolder> SubmitRecord(TravelRequestHolder holder)
        {
            try
            {
                if (holder.ExecuteSubmit())
                {
                    if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                    {
                        using (UserDialogs.Instance.Loading())
                        {
                            await Task.Delay(500);

                            var url = await commonService_.RetrieveClientUrl();
                            await commonService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.TravelRequest
                            };

                            var param = new R.Requests.SubmitTravelRequest()
                            {
                                Data = holder.Model
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitTravelRequest, R.Responses.BaseResponse<R.Models.TravelRequest>>(builder.ToString(), param);

                            if (response.Model.TravelRequestId > 0)
                            {
                                #region save file attachment

                                if (holder.UploadedFilesDisplay.Count > 0)
                                {
                                    var files = new List<FileAttachmentParamsDto>(
                                            holder.UploadedFilesDisplay.Select(x => new FileAttachmentParamsDto()
                                            {
                                                FileDataArray = x.FileDataArray,
                                                FileName = x.FileName,
                                                FileSize = x.FileSize,
                                                FileTags = "TRAVEL REQUEST",
                                                FileType = x.FileType,
                                                MimeType = x.MimeType,
                                                RawFileSize = x.RawFileSize,
                                                ModuleFormId = holder.ModuleFormId, /*ModuleForms.TravelRequest,*/ /*NEED TO VERIFY ON OTHER DEV*/
                                                TransactionId = response.Model.TravelRequestId,
                                            })
                                        );

                                    await commonService_.SaveFileAttachmentsAsync(files);
                                    /*
                                    foreach (var item in holder.UploadedFilesDisplay)
                                    {
                                        await commonService_.SaveSingleFileAttachmentAsync(item.FileData, ModuleForms.TravelRequest, response.Model.TravelRequestId);
                                    }
                                    */
                                }

                                #endregion save file attachment

                                /*
                                if (!string.IsNullOrEmpty(holder.FileData.FileName))
                                    await commonService_.SaveSingleFileAttachmentAsync(holder.FileData, ModuleForms.TravelRequest, response.Model.TravelRequestId);
                                */
                                holder.Success = true;
                                FormSession.IsSubmitted = true;
                            }
                        }
                    }
                }
                else
                {
                    holder.Msg = "Please check for required fields.";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return holder;
        }

        public async Task<TravelRequestHolder> RequestCancelRequest(TravelRequestHolder holder)
        {
            holder.Success = false;
            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(holder.ActionTypeId, holder.Msg);
                if (confirm.Continue)
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(holder.Model.TravelRequestId, TransactionType.Travel);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(holder.Model.TravelRequestId,
                                             TransactionType.Travel,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.TravelRequest}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), holder.Model.TravelRequestId);

                            FormSession.IsSubmitted = response.IsSuccess;
                            holder.Success = response.IsSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return holder;
        }

        public async Task<TravelRequestApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            try
            {
                var holder = new TravelRequestApprovalHolder();
                var url = await commonService_.RetrieveClientUrl();
                await commonService_.HasInternetConnection(url);
                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.TravelRequest}/{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var RECORD_RESPONSE = await genericRepository_.GetAsync<R.Responses.TravelRequestResponse>(builder.ToString());

                if (RECORD_RESPONSE != null && RECORD_RESPONSE.Model != null)
                {
                    var INIT_BUILDER = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.TravelRequest}/init-form"
                    };

                    var INIT_RESPONSE = await genericRepository_.GetAsync<R.Responses.InitTravelRequestFormResponse>(INIT_BUILDER.ToString());

                    if (INIT_RESPONSE != null && INIT_RESPONSE.IsSuccess)
                    {
                        holder.ModuleFormId = INIT_RESPONSE.FARModuleFormId;
                    }

                    var enumUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{EnumValues.TypeOfBusinessTrip}"
                    };

                    var enums = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                    if (enums.Any())
                    {
                        var tripEnum = enums.FirstOrDefault(x => x.Value.Equals(RECORD_RESPONSE.Model.TypeOfBusinessTrip.ToString()));
                        holder.TripType = tripEnum?.DisplayText;
                    }

                    holder.Model = RECORD_RESPONSE.Model;

                    holder.DateFiled = RECORD_RESPONSE.Model.RequestDate.ToString(FormHelper.DateFormat);
                    holder.DepartureDate = RECORD_RESPONSE.Model.FirstDepartureDate.GetValueOrDefault().ToString(FormHelper.DateFormat);
                    holder.DepartureTime = RECORD_RESPONSE.Model.FirstDepartureTime.GetValueOrDefault().ToString(Constants.TimeFormatHHMMTT);
                    holder.Origin = RECORD_RESPONSE.Model.FirstOrigin;
                    holder.Destination = RECORD_RESPONSE.Model.FirstDestination;
                    holder.SpecialRequestNote = RECORD_RESPONSE.Model.Reason;

                    holder.EmployeeName = RECORD_RESPONSE.EmployeeInfo.FullNameMiddleInitialOnly;
                    holder.EmployeeNo = RECORD_RESPONSE.EmployeeInfo.EmployeeNo;
                    holder.EmployeePosition = RECORD_RESPONSE.EmployeeInfo.Position;
                    holder.EmployeeDepartment = RECORD_RESPONSE.EmployeeInfo.Department;

                    holder.TransactionId = transactionId;
                    holder.TransactionTypeId = transactionTypeId;
                    holder.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        holder.WorkflowActions.Add(model);
                    }

                    if (RECORD_RESPONSE.Model.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
                    {
                        holder.WorkflowActions.Add(new WorkflowAction()
                        {
                            ActionMessage = Messages.Cancel,
                            ActionTriggeredId = ActionTypeId.Cancel,
                            ActionType = ActionType.Cancel,
                            CurrentStageId = WFDetail.WorkflowDetail.CurrentStageId,
                            TransactionId = transactionId,
                            TransactionTypeId = transactionTypeId,
                        });
                    }

                    holder.ImageSource = await employeeProfileDataService_.GetProfileImage(RECORD_RESPONSE.EmployeeInfo.ProfileId);
                }

                return holder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
        }

        public async Task<TravelRequestApprovalHolder> WorkflowTransaction(TravelRequestApprovalHolder form)
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

                        var url = await commonService_.RetrieveClientUrl();
                        await commonService_.HasInternetConnection(url);

                        var response = new R.Responses.WFTransactionResponse();

                        if (form.Model.StatusId == RequestStatusValue.Approved && form.SelectedWorkflowAction.ActionTriggeredId == ActionTypeId.Cancel)
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
                                Path = $"{ApiConstants.TravelRequest}/update-wfsource"
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