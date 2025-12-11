using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.Questionnaire;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class SurveyViewModel : BaseViewModel
{
    private readonly ISurveyDataService _surveyService;
    private readonly NavigationManager _navigationManager;
    
    private ObservableCollection<PulseSurveyList> _surveys;

    public SurveyViewModel(ISurveyDataService surveyService, NavigationManager navigationManager)
    {
        _surveyService = surveyService;
        _navigationManager = navigationManager;
        _surveys = new ObservableCollection<PulseSurveyList>();
        
        RefreshCommand = new AsyncRelayCommand(LoadSurveysAsync);
        OpenSurveyCommand = new RelayCommand<PulseSurveyList>(OpenSurvey);
    }

    public ObservableCollection<PulseSurveyList> Surveys
    {
        get => _surveys;
        private set => SetProperty(ref _surveys, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand OpenSurveyCommand { get; }

    public override async Task InitializeAsync()
    {
        await LoadSurveysAsync();
    }

    private async Task LoadSurveysAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            try
            {
                var list = await _surveyService.RetrieveSurveysAsync();
                Surveys = new ObservableCollection<PulseSurveyList>(list);
                ClearError();
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error loading surveys");
            }
        }, "Loading surveys...");
    }

    private void OpenSurvey(PulseSurveyList survey)
    {
        if (survey == null) return;
        _navigationManager.NavigateTo($"/survey/{survey.FormHeaderId}/{survey.AnswerId}");
    }
}

public class SurveyFormViewModel : BaseViewModel
{
    private readonly ISurveyDataService _surveyService;
    private readonly NavigationManager _navigationManager;
    
    private SurveyHolder _surveyHolder;
    private long _formHeaderId;
    private long _answerId;

    public SurveyFormViewModel(ISurveyDataService surveyService, NavigationManager navigationManager)
    {
        _surveyService = surveyService;
        _navigationManager = navigationManager;
        _surveyHolder = new SurveyHolder();
        
        SubmitCommand = new AsyncRelayCommand(SubmitAsync);
        GoBackCommand = new RelayCommand(GoBack);
    }

    public SurveyHolder SurveyHolder
    {
        get => _surveyHolder;
        private set => SetProperty(ref _surveyHolder, value);
    }

    public ICommand SubmitCommand { get; }
    public ICommand GoBackCommand { get; }

    public async Task InitializeFormAsync(long formHeaderId, long answerId)
    {
        _formHeaderId = formHeaderId;
        _answerId = answerId;
        
        await ExecuteBusyAsync(async () =>
        {
            try
            {
                var controls = await _surveyService.RetrievePulseSurveyAsync(new ObservableCollection<BaseQControlDto>(), formHeaderId, answerId);
                
                SurveyHolder = new SurveyHolder
                {
                    ControlList = controls,
                    FormHeaderId = formHeaderId,
                    // FormSurveyHistoryId will be set from the first control if available
                    FormSurveyHistoryId = controls.FirstOrDefault()?.FormSurveyHistoryId ?? 0
                };
                
                ClearError();
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error loading survey form");
            }
        }, "Loading survey...");
    }

    private async Task SubmitAsync()
    {
        if (IsBusy) return;

        // Validation
        if (!ValidateForm())
        {
            ErrorMessage = "Please answer all required questions.";
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            try
            {
                // Collect answers
                SurveyHolder.Answers.Clear();
                foreach (var control in SurveyHolder.ControlList)
                {
                    // Map UI values back to AnswerDto
                    var answer = new AnswerDto
                    {
                        FormQuestionId = control.BaseQuestion.FormQuestionId,
                        Value = control.Value
                    };
                    SurveyHolder.Answers.Add(answer);
                }

                var result = await _surveyService.SubmitAnswerAsync(SurveyHolder);
                
                if (result.IsSuccess)
                {
                    _navigationManager.NavigateTo("/survey/success");
                }
                else
                {
                    ErrorMessage = "Failed to submit survey. Please try again.";
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error submitting survey");
            }
        }, "Submitting...");
    }

    private bool ValidateForm()
    {
        foreach (var control in SurveyHolder.ControlList)
        {
            if (control.BaseQuestion.IsRequired && string.IsNullOrWhiteSpace(control.Value))
            {
                return false;
            }
        }
        return true;
    }

    private void GoBack()
    {
        _navigationManager.NavigateTo("/surveys");
    }
}
