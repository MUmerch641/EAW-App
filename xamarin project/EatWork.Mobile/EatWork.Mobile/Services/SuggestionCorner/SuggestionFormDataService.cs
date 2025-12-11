using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.SuggestionCorner;
using EatWork.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.SuggestionCorner
{
    public class SuggestionFormDataService : ISuggestionFormDataService
    {
        private readonly IDialogService dialogService_;
        private readonly ICommonDataService commonService_;
        private readonly IGenericRepository genericRepository_;

        public SuggestionFormDataService()
        {
            dialogService_ = AppContainer.Resolve<IDialogService>();
            commonService_ = AppContainer.Resolve<ICommonDataService>();
            genericRepository_ = AppContainer.Resolve<IGenericRepository>();
        }

        public async Task<ObservableCollection<ComboBoxObject>> GetCategories()
        {
            var list = new ObservableCollection<ComboBoxObject>();

            try
            {
                var url = await commonService_.RetrieveClientUrl();
                await commonService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.SuggestionCategory}/list"
                };

                var response = await genericRepository_.GetAsync<List<R.Models.SuggestionCategory>>(builder.ToString());

                if (response.Count > 0)
                {
                    foreach (var item in response)
                    {
                        list.Add(new ComboBoxObject()
                        {
                            Id = item.SuggestionCategoryId,
                            Value = item.Name,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }

            return list;
        }

        public async Task<R.Models.SuggestionCategory> SaveCategory(string category)
        {
            var retVal = new R.Models.SuggestionCategory();

            try
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.Save))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);

                        var url = await commonService_.RetrieveClientUrl();
                        await commonService_.HasInternetConnection(url);

                        var builder = new UriBuilder(url)
                        {
                            Path = ApiConstants.SuggestionCategory
                        };

                        var param = new R.Requests.SubmitSuggestionCategoryRequest() { CategoryName = category };

                        var response = await genericRepository_.PostAsync<R.Requests.SubmitSuggestionCategoryRequest, R.Responses.BaseResponse<R.Models.SuggestionCategory>>(builder.ToString(), param);

                        retVal = response.Model;
                    }
                }
            }
            catch (HttpRequestExceptionEx ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }

            return retVal;
        }

        public async Task<FormHolder> SaveRecord(FormHolder holder)
        {
            try
            {
                if (holder.Isvalid())
                {
                    if (await dialogService_.ConfirmDialogAsync(Messages.Save))
                    {
                        using (UserDialogs.Instance.Loading())
                        {
                            await Task.Delay(500);

                            var url = await commonService_.RetrieveClientUrl();
                            await commonService_.HasInternetConnection(url);
                            var user = PreferenceHelper.UserInfo();

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.Suggestion
                            };

                            var param = new R.Requests.SubmitSuggestionRequest()
                            {
                                Data = new R.Models.SuggestionDto()
                                {
                                    Detail = holder.Suggestions.Value,
                                    SuggestionCategoryId = holder.SelectedCategory.Id,
                                    ProfileId = user.ProfileId,
                                    SourceId = (short)SourceEnum.Mobile,
                                }
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitSuggestionRequest, R.Responses.BaseResponse<R.Models.Suggestion>>(builder.ToString(), param);

                            if (response != null && response.Model.SuggestionId > 0)
                            {
                                holder.Success = true;
                                FormSession.IsSubmitted = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }

            return holder;
        }
    }
}