using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using EatWork.Mobile.Views.PerformanceEvaluation;
using EatWork.Mobile.Views.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.PerformanceEvaluation
{
    public class QuestionnaireViewModel : BaseViewModel
    {
        public ICommand TakePhotoCommand { get; set; }
        public ICommand AttachFileCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand MoveToNextCommand { get; set; }
        public ICommand CompetencyBackCommand { get; set; }
        public ICommand CompetencyNextCommand { get; set; }
        public ICommand RatingValueChangedCommand { get; set; }
        public ICommand UnfocusedOutputCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }
        public ICommand BackItemCommand { get; set; }

        private PEFormHolder holder_;

        public PEFormHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private bool forSubmission_;

        public bool ForSubmission
        {
            get { return forSubmission_; }
            set { forSubmission_ = value; RaisePropertyChanged(() => ForSubmission); }
        }

        private bool leftArrowEnabled_;

        public bool LeftArrowEnabled
        {
            get { return leftArrowEnabled_; }
            set { leftArrowEnabled_ = value; RaisePropertyChanged(() => LeftArrowEnabled); }
        }

        private bool rightArrowEnabled_;

        public bool RightArrowEnabled
        {
            get { return rightArrowEnabled_; }
            set { rightArrowEnabled_ = value; RaisePropertyChanged(() => RightArrowEnabled); }
        }

        private readonly ICommonDataService commonService_;
        private readonly IDialogService dialogService_;
        private readonly IPEFormDataService service_;

        public QuestionnaireViewModel()
        {
            commonService_ = AppContainer.Resolve<ICommonDataService>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
            service_ = AppContainer.Resolve<IPEFormDataService>();
        }

        public void Init(INavigation nav, PEFormHolder holder)
        {
            NavigationBack = nav;
            Holder = holder;
            LeftArrowEnabled = false;
            RightArrowEnabled = (holder.CombinedCriteriaList.Count > 1);

            TakePhotoCommand = new Command(async () => await ExecuteTakePhotoCommand());
            AttachFileCommand = new Command(async () => await ExecuteAttachFileCommand());
            MoveToNextCommand = new Command(async () => await ExecuteMoveToNextCommand());
            /*OpenFileCommand = new Command<PEUploadedFilesDto>(ExecuteOpenFileCommand);*/
            FileChooseOptionCommand = new Command<PEUploadedFilesDto>(ExecuteFileChooseOptionCommand);
            CompetencyBackCommand = new Command(ExecuteCompetencyBackCommand);
            CompetencyNextCommand = new Command(ExecuteCompetencyNextCommand);
            RatingValueChangedCommand = new Command<Syncfusion.SfRating.XForms.ValueEventArgs>(ExecuteRatingValueChangedCommand);
            UnfocusedOutputCommand = new Command<string>(ExecuteUnfocusedOutputCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);
            BackItemCommand = new Command(ExecuteBackItemCommand);

            ForSubmission = holder.ForSubmission;
        }

        private async Task ExecuteTakePhotoCommand()
        {
            try
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    var file = await commonService_.TakePhotoAsync("PE");
                    /*var file = await commonService_.AttachPhoto2(response, "PE");*/

                    if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                    {
                        var dto = new PEUploadedFilesDto()
                        {
                            FileData = file.FileData,
                            FileDataArray = file.FileDataArray,
                            FileName = file.FileName,
                            FileResult = file.FileResult,
                            FileSize = file.FileSize,
                            FileType = file.FileType,
                            MimeType = file.MimeType,
                            RawFileSize = file.RawFileSize,
                            HeaderId = 0,
                        };

                        Holder.UploadedFilesDisplay = new System.Collections.ObjectModel.ObservableCollection<PEUploadedFilesDto>
                        {
                            dto
                        };

                        /*Holder.UploadedFile = dto;*/

                        Holder = await service_.ManageUploadFile(file, Holder);
                    }

                    Holder = Holder;
                }
            }
            catch (Exception ex)
            {
                await dialogService_.AlertAsync(ex.Message);
            }
        }

        private async Task ExecuteAttachFileCommand()
        {
            try
            {
                var file = new Models.DataObjects.FileUploadResponse();

                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    file = await commonService_.FileUploadAsync();
                }

                if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    //check if exists
                    /*var exst = Holder.UploadedFilesDisplay.Where(x => x.FileName == file.FileName).FirstOrDefault();*/

                    var dto = new PEUploadedFilesDto()
                    {
                        FileData = file.FileData,
                        FileDataArray = file.FileDataArray,
                        FileName = file.FileName,
                        FileResult = file.FileResult,
                        FileSize = file.FileSize,
                        FileType = file.FileType,
                        MimeType = file.MimeType,
                        RawFileSize = file.RawFileSize,
                        HeaderId = 0,
                    };

                    Holder.UploadedFilesDisplay = new System.Collections.ObjectModel.ObservableCollection<PEUploadedFilesDto>
                    {
                        dto
                    };
                    /*
                    if (exst != null)
                        await dialogService_.AlertAsync("File already exists.");
                    else
                    {
                        Holder.UploadedFilesDisplay = new System.Collections.ObjectModel.ObservableCollection<PEUploadedFilesDto>();
                        Holder.UploadedFilesDisplay.Add(dto);
                    }
                    */

                    Holder = await service_.ManageUploadFile(file, Holder);
                }
            }
            catch (Exception ex)
            {
                await dialogService_.AlertAsync(ex.Message);
            }
        }

        private async Task ExecuteMoveToNextCommand()
        {
            if (!IsBusy)
            {
                if (Holder.CurrentPage == Holder.CombinedCriteriaList.Count)
                {
                    if (Holder.CurrentPage > 1)
                    {
                        if (Holder.IsValidQuestionAnswer())
                            Holder = await service_.CollectQuestionAnswer(Holder);
                    }

                    if (await dialogService_.ConfirmDialogAsync("Move to Narrative Section?"))
                    {
                        using (Dialogs.Loading())
                        {
                            await Task.Delay(500);

                            if (Holder.ForSubmission)
                                Holder = await service_.SavePAQuestionnaire(Holder);

                            await NavigationService.PushPageAsync(new NarrativeSectionPage(Holder));
                        }
                    }
                }
                else
                    ExecuteCompetencyNextCommand();
            }
        }

        private async void ExecuteCompetencyBackCommand()
        {
            if (Holder.CurrentPage > 1)
                Holder.CurrentPage = Holder.CurrentPage - 1;

            Holder = await service_.CollectQuestionAnswer(Holder);

            OnChangeCompetency();
        }

        private async void ExecuteCompetencyNextCommand()
        {
            if (Holder.IsValidQuestionAnswer())
            {
                Holder.CurrentPage = Holder.CurrentPage + 1;

                Holder = await service_.CollectQuestionAnswer(Holder);

                OnChangeCompetency();
            }

            if (!Holder.IsValidStarRating)
                await dialogService_.AlertAsync("Please select rating");
        }

        private async void OnChangeCompetency()
        {
            RightArrowEnabled = (Holder.CurrentPage < Holder.CombinedCriteriaList.Count);
            LeftArrowEnabled = (Holder.CurrentPage > 1);

            /*var page = (Holder.CurrentPage == 1 ? 0 : Holder.Page);*/
            var page = (Holder.CurrentPage - 1);

            var item = Holder.CombinedCriteriaList.Skip(page * Holder.PageSize).Take(Holder.PageSize).FirstOrDefault();
            if (item != null)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    Holder = await service_.OnCompetencyChanged(Holder, item);
                }
            }
        }

        private void ExecuteRatingValueChangedCommand(Syncfusion.SfRating.XForms.ValueEventArgs e)
        {
            if (e != null)
            {
                var _methodMaximumScorePerItemWeight = Holder.MethodMaximumScorePerItemWeight.ToString();
                var _weightRating = Holder.ActualWeightRating.ToString();

                var starRatingHint = Holder.CriteriaRatingValues.FirstOrDefault(x => x.Id == e.Value).Value;
                var ws = float.Parse(starRatingHint) * float.Parse(_weightRating) / 100;
                var weightPercentage = float.Parse(_weightRating) / 100;

                if (Holder.MethodMaximumScorePerItem)
                    ws = (float.Parse(starRatingHint) / float.Parse(_methodMaximumScorePerItemWeight) * weightPercentage) * 100;

                Holder.WeightedScore = ws.ToString("n2");
                Holder.StarRatingHint = starRatingHint;
            }
        }

        private void ExecuteUnfocusedOutputCommand(string val)
        {
            if (!string.IsNullOrWhiteSpace(val))
            {
                var floatVal = float.Parse(val);
                var item = Holder.KpiCriteriaLookup.FirstOrDefault(x => floatVal >= x.Min && floatVal <= x.Max);

                if (item != null)
                {
                    Holder.KPIRating.Value = item.Score.ToString("n2");

                    var ws = float.Parse(Holder.KPIRating.Value) * float.Parse(Holder.ActualWeightRating.ToString()) / 100;

                    Holder.WeightedScore = ws.ToString("n2");
                }
            }
        }

        private async void ExecuteFileChooseOptionCommand(PEUploadedFilesDto file)
        {
            if (file != null)
            {
                Holder.SelectedFile = file;

                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new FileOptionModal());
                }
            }
        }

        private async void ExecuteChosenOption(long option)
        {
            var file = Holder.SelectedFile;

            //view file
            if (option == 1)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    if (file.FileDataArray != null)
                    {
                        var base64 = $"data:{file.MimeType};base64,{Convert.ToBase64String(file.FileDataArray)}";
                        await commonService_.PreviewFileBase64(base64, file.FileType, file.FileName);
                    }
                    else
                    {
                        var urlFile = await service_.ViewAttachment(file.FileUpload, file.FileName, file.FileType);
                    }
                }
            }
            //delete file
            else
            {
                Holder = await service_.ManageUploadFile(file, Holder, true);
            }
        }

        private void ExecutePageAppearingCommand()
        {
            MessagingCenter.Subscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue", (s, param) =>
            {
                if (param > 0)
                {
                    ExecuteChosenOption(param);
                }
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
        }

        private async void ExecuteBackItemCommand()
        {
            if (!IsBusy)
            {
                if (Holder.CurrentPage == 1)
                {
                    if (await dialogService_.ConfirmDialogAsync("Move to Individual Objectives?"))
                        base.BackItemPage();
                }
                else
                    ExecuteCompetencyBackCommand();
            }
        }
    }
}