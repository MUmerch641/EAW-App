using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiHybridApp.Models;

namespace MauiHybridApp.Services.Data
{
    public interface IDocumentDataService
    {
        Task<List<DocumentRequestListModel>> GetDocumentRequestsAsync();
        Task<DocumentRequestModel> GetDocumentRequestByIdAsync(long id);
        Task<bool> SubmitDocumentRequestAsync(DocumentRequestModel request);
        Task<List<DocumentTypeModel>> GetDocumentTypesAsync();
        Task<List<ReasonModel>> GetReasonsAsync();
    }
}
