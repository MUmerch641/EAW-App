using EatWork.Mobile.Models.DataObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using R = EAW.API.DataContracts.Models;

namespace EatWork.Mobile.Contracts
{
    public interface ISuggestionListDataService
    {
        long TotalListItem { get; set; }

        Task<ObservableCollection<R.SuggestionListDto>> GetListAsync(ObservableCollection<R.SuggestionListDto> list, ListParam args);
    }
}
