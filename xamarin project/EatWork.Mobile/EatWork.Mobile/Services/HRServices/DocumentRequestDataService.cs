using Acr.UserDialogs;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
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
    public class DocumentRequestDataService : IDocumentRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;

        public DocumentRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            IDialogService dialogService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            dialogService_ = dialogService;
        }

        public async Task<DocumentRequestHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new DocumentRequestHolder()
            {
                DocumentsList = new ObservableCollection<Models.DataObjects.SelectableListModel>(),
                ReasonList = new ObservableCollection<Models.DataObjects.SelectableListModel>(),
                DocumentType = new Models.DataObjects.SelectableListModel(),
                Reason = new Models.DataObjects.SelectableListModel()
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            try
            {
                var documentListUrl = new UriBuilder(url)
                {
                    Path = ApiConstants.GetDocumentTypes
                };

                var documentType = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.DocumentType>>(documentListUrl.ToString());

                if (documentType.ListData.Count > 0)
                {
                    foreach (var item in documentType.ListData.Where(p => p.StatusId == 1 && p.SourceTypeId != 3))
                        retValue.DocumentsList.Add(new Models.DataObjects.SelectableListModel() { Id = item.DocumentTypeId, DisplayText = item.DocumentName });
                }

                var reasonListUrl = new UriBuilder(url)
                {
                    Path = ApiConstants.GetReasonPurposeList
                };

                var reasonList = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.ReasonPurpose>>(reasonListUrl.ToString());

                if (reasonList.ListData.Count > 0)
                {
                    foreach (var item in reasonList.ListData)
                        retValue.ReasonList.Add(new Models.DataObjects.SelectableListModel() { Id = item.ReasonPurposeId, DisplayText = item.Text });
                }

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetDocumentRequestDetail}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.DocumentRequestResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        retValue.DocumentRequestModel = new DocumentRequestModel();

                        PropertyCopier<R.Models.DocumentRequest, DocumentRequestModel>.Copy(response.DocumentRequest, retValue.DocumentRequestModel);
                        PropertyCopier<R.Models.DocumentRequest, DocumentRequestHolder>.Copy(response.DocumentRequest, retValue);

                        var document = retValue.DocumentsList.Where(p => p.Id == retValue.DocumentRequestModel.DocumentId).FirstOrDefault();

                        if (document.Id > 0)
                            retValue.DocumentType = document;
                    }
                    else
                        throw new Exception(response.ErrorMessage);

                    retValue.IsEnabled = (retValue.DocumentRequestModel.StatusId == RequestStatusValue.Submitted);
                    retValue.ShowCancelButton = (retValue.DocumentRequestModel.StatusId == RequestStatusValue.ForApproval);
                }
                else
                {
                    retValue.DocumentRequestModel = new DocumentRequestModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        DateRequested = DateTime.Now.Date,
                        DateStart = selectedDate ?? DateTime.Now.Date,
                        DateEnd = selectedDate ?? DateTime.Now.Date,
                    };

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

        public async Task<DocumentRequestHolder> SubmitRequest(DocumentRequestHolder form)
        {
            form.ErrorDetails = false;
            form.ErrorDocumentType = false;
            form.ErrorReason = false;
            var errors = new List<int>();

            if (string.IsNullOrWhiteSpace(form.DocumentType.DisplayText))
            {
                form.ErrorDocumentType = true;
                errors.Add(1);
            }

            if (string.IsNullOrWhiteSpace(form.DocumentRequestModel.Reason))
            {
                form.ErrorReason = true;
                errors.Add(1);
            }

            if (string.IsNullOrWhiteSpace(form.DocumentRequestModel.Details))
            {
                form.ErrorDetails = true;
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        try
                        {
                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitDocumentRequest
                            };

                            form.DocumentRequestModel.DocumentId = form.DocumentType.Id;

                            var data = new R.Models.DocumentRequest();
                            PropertyCopier<DocumentRequestModel, R.Models.DocumentRequest>.Copy(form.DocumentRequestModel, data);

                            var param = new R.Requests.SubmitDocumentRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitDocumentRequest, R.Responses.BaseResponse<R.Models.DocumentRequest>>(builder.ToString(), param);

                            if (response.Model.DocumentRequestId > 0)
                            {
                                /*
                                if (!string.IsNullOrEmpty(form.FileData.FileName))
                                    await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.DocumentRequest, response.Model.DocumentRequestId);
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
                                                FileTags = "DOCUMENT",
                                                FileType = x.FileType,
                                                MimeType = x.MimeType,
                                                RawFileSize = x.RawFileSize,
                                                ModuleFormId = ModuleForms.DocumentRequest,
                                                TransactionId = response.Model.DocumentRequestId,
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

        public async Task<DocumentRequestHolder> WorkflowTransactionRequest(DocumentRequestHolder form)
        {
            var retValue = form;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.ActionTypeId, form.Msg);

                if (!confirm.Continue)
                    return retValue;

                await Task.Delay(500);
                using (UserDialogs.Instance.Loading())
                {
                    var WFDetail = await workflowDataService_.GetWorkflowDetails(form.DocumentRequestModel.DocumentRequestId, TransactionType.Document);

                    var response = await workflowDataService_.CancelWorkFlowByRecordId(form.DocumentRequestModel.DocumentRequestId,
                                         TransactionType.Document,
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

                        await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.DocumentRequestModel.DocumentRequestId);

                        FormSession.IsSubmitted = response.IsSuccess;
                        form.Success = response.IsSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }

            return retValue;
        }

        public async Task<DocumentRequestApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new DocumentRequestApprovalHolder();

            return retValue;
        }

        public async Task<DocumentRequestApprovalHolder> WorkflowTransaction(DocumentRequestApprovalHolder form)
        {
            var retValue = form;

            return retValue;
        }
    }
}