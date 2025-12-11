using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class DocumentRequestFormViewModel : BaseViewModel
    {
        private readonly IDocumentDataService _documentService;
        private readonly NavigationManager _navigationManager;

        public DocumentRequestFormViewModel(IDocumentDataService documentService, NavigationManager navigationManager)
        {
            _documentService = documentService;
            _navigationManager = navigationManager;
            
            SubmitCommand = new Command(async () => await SubmitAsync());
            CancelCommand = new Command(GoBack);
        }

        // Form Fields
        private DateTime _requestDate = DateTime.Today;
        public DateTime RequestDate
        {
            get => _requestDate;
            set => SetProperty(ref _requestDate, value);
        }

        private DocumentTypeModel _selectedDocumentType;
        public DocumentTypeModel SelectedDocumentType
        {
            get => _selectedDocumentType;
            set => SetProperty(ref _selectedDocumentType, value);
        }

        private ReasonModel _selectedReason;
        public ReasonModel SelectedReason
        {
            get => _selectedReason;
            set => SetProperty(ref _selectedReason, value);
        }

        private string _details;
        public string Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }

        private DateTime _dateStart = DateTime.Today;
        public DateTime DateStart
        {
            get => _dateStart;
            set => SetProperty(ref _dateStart, value);
        }

        private DateTime _dateEnd = DateTime.Today;
        public DateTime DateEnd
        {
            get => _dateEnd;
            set => SetProperty(ref _dateEnd, value);
        }

        // Dropdowns
        public ObservableCollection<DocumentTypeModel> DocumentTypes { get; private set; } = new();
        public ObservableCollection<ReasonModel> Reasons { get; private set; } = new();

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public override async Task InitializeAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var types = await _documentService.GetDocumentTypesAsync();
                DocumentTypes = new ObservableCollection<DocumentTypeModel>(types);

                var reasons = await _documentService.GetReasonsAsync();
                Reasons = new ObservableCollection<ReasonModel>(reasons);
            }, "Loading form...");
        }

        private async Task SubmitAsync()
        {
            if (SelectedDocumentType == null)
            {
                ErrorMessage = "Please select a document type.";
                return;
            }

            if (SelectedReason == null)
            {
                ErrorMessage = "Please select a reason.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Details))
            {
                ErrorMessage = "Details are required.";
                return;
            }

            await ExecuteBusyAsync(async () =>
            {
                var request = new DocumentRequestModel
                {
                    RequestDate = RequestDate,
                    DocumentId = SelectedDocumentType.DocumentTypeId,
                    Reason = SelectedReason.Text, // Or ID if API expects ID
                    Details = Details,
                    DateStart = DateStart,
                    DateEnd = DateEnd
                };

                var success = await _documentService.SubmitDocumentRequestAsync(request);

                if (success)
                {
                    SuccessMessage = "Request submitted successfully.";
                    await Task.Delay(1500);
                    GoBack();
                }
                else
                {
                    ErrorMessage = "Failed to submit request.";
                }
            }, "Submitting...");
        }

        private void GoBack()
        {
            _navigationManager.NavigateTo("/document");
        }
    }
}
