using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Profile;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.MyProfile;
using EatWork.Mobile.Views.Shared;
using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class EmployeeProfileDataService : IEmployeeProfileDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;

        public EmployeeProfileDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
        }

        public async Task<ObservableCollection<MenuItemModel>> InitEmployeeCategory()
        {
            var retValue = new ObservableCollection<MenuItemModel>();

            await Task.Run(() =>
            {
                retValue = new ObservableCollection<MenuItemModel>()
                {
                    new MenuItemModel { Icon="Personalinformation.svg", Title="Personal Details", TargetType = typeof(ProfileCategoryPage), GroupId = 1 },
                    new MenuItemModel { Icon="EmployeeProfile.svg", Title="Employee Profile", TargetType = typeof(ProfileCategoryPage), GroupId = 2 },
                    //new MenuItemModel { Icon="EmploymentInformation.svg", Title="Employment Information", TargetType = typeof(EmploymentInformationPage), GroupId = 3 },
                    new MenuItemModel { Icon="EmploymentInformation.svg", Title="Employment Information", TargetType = typeof(ProfileCategoryPage), GroupId = 3 },
                    new MenuItemModel { Icon="Gift.svg", Title="Compensation & Benefits",TargetType = typeof(ComingSoonPage), GroupId = 4 },
                    new MenuItemModel { Icon="PersonnelDevelopment.svg", Title="Personnel Development", TargetType = typeof(ComingSoonPage), GroupId = 5 },
                    new MenuItemModel { Icon="Awards.svg", Title="Awards & Citation", TargetType = typeof(ComingSoonPage), GroupId = 6 },
                    new MenuItemModel { Icon="Cancel.svg", Title="Violation & Disciplinary Action", TargetType = typeof(ComingSoonPage), GroupId = 7 },
                    new MenuItemModel { Icon="Resume.svg", Title="Employee Documents", TargetType = typeof(ComingSoonPage), GroupId = 8 },
                    new MenuItemModel { Icon="EmployeeGroup.svg", Title="Affiliations", TargetType = typeof(ComingSoonPage), GroupId = 9 },
                    new MenuItemModel { Icon="Certificate.svg", Title="Certifications", TargetType = typeof(ComingSoonPage), GroupId = 10 },

                    //new MenuItemModel { Icon="fas-user", Title="Personal Details", TargetType = typeof(PersonalDetailsPage), GroupId = 1 },
                    //new MenuItemModel { Icon="fas-male", Title="Employee Profile", TargetType = typeof(ProfileCategoryPage), GroupId = 2 },
                    //new MenuItemModel { Icon="fas-info", Title="Employment Information", TargetType = typeof(ComingSoonPage), GroupId = 3 },
                    //new MenuItemModel { Icon="fas-gift", Title="Compensation & Benefits",TargetType = typeof(ComingSoonPage), GroupId = 4 },
                    //new MenuItemModel { Icon="fas-tasks", Title="Personnel Development", TargetType = typeof(ComingSoonPage), GroupId = 5 },
                    //new MenuItemModel { Icon="fas-bookmark", Title="Awards & Citation", TargetType = typeof(ComingSoonPage), GroupId = 6 },
                    //new MenuItemModel { Icon="fas-exclamation", Title="Violation & Disciplinary Action", TargetType = typeof(ComingSoonPage), GroupId = 7 },
                    //new MenuItemModel { Icon="fas-briefcase", Title="Employee Documents", TargetType = typeof(ComingSoonPage), GroupId = 8 },
                    //new MenuItemModel { Icon="fab-connectdevelop", Title="Affiliations", TargetType = typeof(ComingSoonPage), GroupId = 9 },
                    //new MenuItemModel { Icon="fas-certificate", Title="Certifications", TargetType = typeof(ComingSoonPage), GroupId = 10 },
                    //new MenuItemModel { Icon="fas-info", Title="Custom Fields", TargetType = typeof(ComingSoonPage) },
                    //new MenuItemModel { Icon="fas-ellipsis-h", Title="Others", TargetType = typeof(ComingSoonPage) },
                };
            });

            return retValue;
        }

        public async Task<ObservableCollection<MenuItemModel>> InitEmployeeSubCategory(int groupId = 0)
        {
            var retValue = new ObservableCollection<MenuItemModel>();

            var list = new ObservableCollection<MenuItemModel>()
            {
                new MenuItemModel { Icon="fas-user", Title="Personal Details", TargetType = typeof(PersonalDetailsPage),GroupId = 1, IsSeparatorVisible = true},
                new MenuItemModel { Icon="fas-phone", Title="Contact Details", TargetType = typeof(ContactInformationPage),GroupId = 1, IsSeparatorVisible = true },

                new MenuItemModel { Icon="fas-home", Title="Family Background", TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = true},
                new MenuItemModel { Icon="fas-graduation-cap", Title="Educational Background", TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = true },
                new MenuItemModel { Icon="fas-phone", Title="Emergency Contact", TargetType = typeof(EmergencyContactPage),GroupId = 2, IsSeparatorVisible = true },
                new MenuItemModel { Icon="fas-users", Title="Character Reference",TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = true },
                new MenuItemModel { Icon="fas-info-circle", Title="Tax & Govt-related Info", TargetType = typeof(TaxGovernmentRelatedInfoPage),GroupId = 2, IsSeparatorVisible = true },
                new MenuItemModel { Icon="fas-history", Title="Past Employment", TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = false },

                new MenuItemModel { Icon="fas-user", Title="Current Job Information", TargetType = typeof(CurrentJobInformationPage),GroupId = 3, IsSeparatorVisible = true},
                new MenuItemModel { Icon="fas-phone", Title="Area of Assignment", TargetType = typeof(AreaOfAssignmentPage),GroupId = 3, IsSeparatorVisible = true },
                new MenuItemModel { Icon="fas-user", Title="Relevant Employment Dates", TargetType = typeof(RelevantEmploymentDatesPage),GroupId = 3, IsSeparatorVisible = true},
                new MenuItemModel { Icon="fas-phone", Title="Rehire/Renewal Dates", TargetType = typeof(ComingSoonPage),GroupId = 3, IsSeparatorVisible = true },

                new MenuItemModel { Icon="fas-user", Title="Salary Information", TargetType = typeof(ComingSoonPage),GroupId = 4, IsSeparatorVisible = true},
                new MenuItemModel { Icon="fas-phone", Title="Other Earnings & Benefits", TargetType = typeof(ComingSoonPage),GroupId = 4, IsSeparatorVisible = true },
                new MenuItemModel { Icon="fas-user", Title="Employee Beneficiaries", TargetType = typeof(ComingSoonPage),GroupId = 4, IsSeparatorVisible = true},
                new MenuItemModel { Icon="fas-phone", Title="Bank Account", TargetType = typeof(ComingSoonPage),GroupId = 4, IsSeparatorVisible = true },

                new MenuItemModel { Icon="fas-user", Title="Skills", TargetType = typeof(ComingSoonPage),GroupId = 5, IsSeparatorVisible = true},
                new MenuItemModel { Icon="fas-phone", Title="Training Attended", TargetType = typeof(ComingSoonPage),GroupId = 5, IsSeparatorVisible = true },
                new MenuItemModel { Icon="fas-user", Title="Past/Pre-employment Training", TargetType = typeof(ComingSoonPage),GroupId = 5, IsSeparatorVisible = true},
                new MenuItemModel { Icon="fas-phone", Title="Presentations & Seminars Conducted", TargetType = typeof(ComingSoonPage),GroupId = 5, IsSeparatorVisible = true },

                //new MenuItemModel { Icon="FamilyBackground.svg", Title="Family Background", TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = true},
                //new MenuItemModel { Icon="EducationalBackground.svg", Title="Educational Background", TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = true },
                //new MenuItemModel { Icon="Contactinformation.svg", Title="Emergency Contact", TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = true },
                //new MenuItemModel { Icon="CharacterReference.svg", Title="Character Reference",TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = true },
                //new MenuItemModel { Icon="IdentificationCard.svg", Title="Tax & Govt-related Info", TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = true },
                //new MenuItemModel { Icon="Resume.svg", Title="Past Employment", TargetType = typeof(ComingSoonPage),GroupId = 2, IsSeparatorVisible = false },
            };

            await Task.Run(() =>
            {
                retValue = new ObservableCollection<MenuItemModel>(list.Where(p => p.GroupId == groupId));
            });

            return retValue;
        }

        #region personal details

        public async Task<ProfileHolder> InitPersonalDetails(long profileId)
        {
            var retValue = new ProfileHolder();
            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            var recordId = (profileId > 0 ? profileId : userInfo.ProfileId);

            try
            {
                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEmployeeById}{recordId}"
                };

                var personal = new UriBuilder(url)
                {
                    Path = string.Format(ApiConstants.GetEmployeePersonalDetails, recordId)
                };

                var response = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.EmployeeInformation>>(builder.ToString());
                var personalDetails = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.Profile>>(personal.ToString());

                if (response.Model.ProfileId > 0)
                {
                    PropertyCopier<R.Models.Profile, ProfileHolder>.Copy(personalDetails.Model, retValue);
                    PropertyCopier<R.Models.EmployeeInformation, ProfileHolder>.Copy(response.Model, retValue);

                    if (!string.IsNullOrWhiteSpace(retValue.CivilStatus))
                    {
                        var enumrequest = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.CivilStatus}/{retValue.CivilStatus}"
                        };

                        var civilStatus = await genericRepository_.GetAsync<R.Models.Enums>(enumrequest.ToString());

                        retValue.CivilStatusString = civilStatus.DisplayText;
                    }

                    if (retValue.TaxExemptionStatusId > 0)
                    {
                        var taxStatusRequest = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.TaxExemptionStatus}/{retValue.TaxExemptionStatusId}"
                        };
                        var taxStatus = await genericRepository_.GetAsync<R.Models.Enums>(taxStatusRequest.ToString());
                        retValue.TaxExemptionStatus = taxStatus.DisplayText;
                    }

                    if (retValue.ApplicableTaxId > 0)
                    {
                        var request = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.ApplicableTax}/{retValue.ApplicableTaxId}"
                        };
                        var applicableTax = await genericRepository_.GetAsync<R.Models.Enums>(request.ToString());
                        retValue.ApplicableTax = applicableTax.DisplayText;
                    }

                    if (retValue.ApplicableTaxId > 0)
                    {
                        var request = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.ApplicableTax}/{retValue.ApplicableTaxId}"
                        };
                        var applicableTax = await genericRepository_.GetAsync<R.Models.Enums>(request.ToString());
                        retValue.ApplicableTax = applicableTax.DisplayText;
                    }

                    if (!string.IsNullOrWhiteSpace(personalDetails.Model.BloodType))
                    {
                        var request = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.BloodType}/{personalDetails.Model.BloodType}"
                        };
                        var blood = await genericRepository_.GetAsync<R.Models.Enums>(request.ToString());
                        retValue.BloodType = blood.DisplayText;
                    }

                    if (!string.IsNullOrWhiteSpace(personalDetails.Model.HairColor))
                    {
                        var request = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.HairColor}/{personalDetails.Model.HairColor}"
                        };
                        var hair = await genericRepository_.GetAsync<R.Models.Enums>(request.ToString());
                        retValue.HairColor = hair.DisplayText;
                    }

                    if (!string.IsNullOrWhiteSpace(personalDetails.Model.EyeColor))
                    {
                        var request = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.EyeColor}/{personalDetails.Model.EyeColor}"
                        };
                        var eye = await genericRepository_.GetAsync<R.Models.Enums>(request.ToString());
                        retValue.EyeColor = eye.DisplayText;
                    }

                    retValue.DateOfMarriageString = retValue.DateOfMarriage.GetValueOrDefault().DateToString();
                    retValue.BirthdateString = retValue.Birthdate.GetValueOrDefault().DateToString();
                    retValue.MinimumWageEarnerString = (retValue.MinimumWageEarner == 1 ? "Yes" : "No");
                    retValue.SoloParanentString = (retValue.SoloParent == 1 ? "Yes" : "No");
                    retValue.Age = (response.Model.Age > 0 ? response.Model.Age.ToString() : "");
                    retValue.FullProvincialAddress = response.Model.FullProvincialAddress;
                    retValue.ReligionString = response.Model.Religion;
                    retValue.NationalityString = response.Model.Nationality;
                    retValue.DualNationalityString = response.Model.DualNationality;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        #endregion personal details

        #region family background

        public async Task<SfListView> InitListViewFamilyBackground(SfListView listview, bool isAsceding = true)
        {
            var retValue = listview;

            await Task.Run(() =>
            {
                if (retValue.DataSource.GroupDescriptors.Count == 0)
                {
                    retValue.DataSource.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "CategoryString",
                        KeySelector = (object obj1) =>
                        {
                            return (obj1 as FamilyBackgroundListHolder).CategoryString;
                        },
                    });

                    retValue.DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "Category",
                        Direction = isAsceding ? ListSortDirection.Ascending : ListSortDirection.Descending
                    });
                }
            });

            return retValue;
        }

        public async Task<ObservableCollection<FamilyBackgroundListHolder>> RetrieveFamilyBackgroundList(int listcount, int count, ObservableCollection<FamilyBackgroundListHolder> list, long profileId)
        {
            var retValue = list;

            var temp = await RetrieveFamilyBackgroundList();
            count = (temp.Count <= count ? temp.Count : count);

            if (temp.Count > 0)
            {
                try
                {
                    for (int i = listcount; i < listcount + count; i++)
                    {
                        if (temp.ElementAtOrDefault(i) != null)
                        {
                            var model = new FamilyBackgroundListHolder()
                            {
                                Category = temp[i].Category,
                                CategoryString = temp[i].CategoryString,
                                Relationship = temp[i].Relationship,
                                Name = temp[i].Name,
                                Birtdate = temp[i].Birtdate,
                                Occupation = temp[i].Occupation,
                                OccAddress = temp[i].OccAddress,
                                Address = temp[i].Address,
                                ContactNo = temp[i].ContactNo,
                                DependentString = temp[i].DependentString,
                                IncapacitatedString = temp[i].IncapacitatedString,
                            };

                            retValue.Add(model);
                        }
                        else
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return retValue;
        }

        private async Task<ObservableCollection<FamilyBackgroundListHolder>> RetrieveFamilyBackgroundList()
        {
            var retValue = new ObservableCollection<FamilyBackgroundListHolder>()
            {
                new FamilyBackgroundListHolder(){CategoryString = "Parents", Category = 1, Relationship = "Father", Name = "Roberto Dela Cruz", Birtdate = Convert.ToDateTime("12/31/1940"), Occupation = "Electrician",OccAddress = "Quezon City", Address = "Quezon City",ContactNo = "09123456789",DependentString = "No", IncapacitatedString = "No" },
                new FamilyBackgroundListHolder(){CategoryString = "Parents", Category = 1, Relationship = "Mother", Name = "Isabel Dela Cruz", Birtdate = Convert.ToDateTime("12/30/1941"), Occupation = "Yaya/Cook",OccAddress = "Quezon City", Address = "Quezon City",ContactNo = "09123456789",DependentString = "No", IncapacitatedString = "No" },
                new FamilyBackgroundListHolder(){CategoryString = "Spouse", Category = 2, Relationship = "Wife", Name = "Jillian Dela Cruz", Birtdate = Convert.ToDateTime("12/30/1979"), Occupation = "Businesswoman",OccAddress = "Makati City", Address = "Quezon City",ContactNo = "09123456789",DependentString = "No", IncapacitatedString = "No" },
                new FamilyBackgroundListHolder(){CategoryString = "Children", Category = 3, Relationship = "Daughter", Name = "Isabel Dela Cruz", Birtdate = Convert.ToDateTime("12/31/1999"), Occupation = "Student",OccAddress = "Manily City", Address = "Quezon City",ContactNo = "09123456789",DependentString = "Yes", IncapacitatedString = "No" },
                new FamilyBackgroundListHolder(){CategoryString = "Children", Category = 3, Relationship = "Son", Name = "Johnny Dela Cruz", Birtdate = Convert.ToDateTime("12/31/2004"), Occupation = "Student",OccAddress = "Manily City", Address = "Quezon City",ContactNo = "09123456789",DependentString = "Yes", IncapacitatedString = "No" },
                new FamilyBackgroundListHolder(){CategoryString = "Siblings", Category = 4, Relationship = "Brother", Name = "Alex Dela Cruz", Birtdate = Convert.ToDateTime("12/31/1978"), Occupation = "Student",OccAddress = "Manily City", Address = "Quezon City",ContactNo = "09123456789",DependentString = "No", IncapacitatedString = "No" },
                new FamilyBackgroundListHolder(){CategoryString = "Siblings", Category = 4, Relationship = "Sister", Name = "Cathy Dela Cruz", Birtdate = Convert.ToDateTime("12/31/1980"), Occupation = "Student",OccAddress = "Manily City", Address = "Quezon City",ContactNo = "09123456789",DependentString = "No", IncapacitatedString = "No" },
            };

            return retValue;
        }

        #endregion family background

        #region educational background

        public async Task<SfListView> InitListViewEducationalBackground(SfListView listview, bool isAsceding = false)
        {
            var retValue = listview;

            await Task.Run(() =>
            {
                if (retValue.DataSource.GroupDescriptors.Count == 0)
                {
                    retValue.DataSource.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "EducationalLevelString",
                        KeySelector = (object obj1) =>
                        {
                            return (obj1 as EducationalBackgroundListHolder).EducationalLevelString;
                        },
                    });

                    retValue.DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "EducationalLevel",
                        Direction = isAsceding ? ListSortDirection.Ascending : ListSortDirection.Descending
                    });
                }
            });

            return retValue;
        }

        public async Task<ObservableCollection<EducationalBackgroundListHolder>> RetrieveEducationalBackgroundList(int listcount, int count, ObservableCollection<EducationalBackgroundListHolder> list, long profileId)
        {
            var retValue = list;

            var temp = await RetrieveEducationalBackgroundList();
            count = (temp.Count <= count ? temp.Count : count);

            if (temp.Count > 0)
            {
                try
                {
                    for (int i = listcount; i < listcount + count; i++)
                    {
                        if (temp.ElementAtOrDefault(i) != null)
                        {
                            var model = new EducationalBackgroundListHolder()
                            {
                                //Category = temp[i].Category,
                            };

                            retValue.Add(model);
                        }
                        else
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return retValue;
        }

        private async Task<ObservableCollection<EducationalBackgroundListHolder>> RetrieveEducationalBackgroundList()
        {
            var retValue = new ObservableCollection<EducationalBackgroundListHolder>()
            {
            };

            return retValue;
        }

        #endregion educational background

        #region employment information

        public async Task<EmploymentInformationHolder> InitEmploymentInformation(long profileId)
        {
            var retValue = new EmploymentInformationHolder();
            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            var recordId = (profileId > 0 ? profileId : userInfo.ProfileId);

            try
            {
                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEmployeeById}{recordId}"
                };

                var response = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.EmployeeInformation>>(builder.ToString());

                if (response.Model.ProfileId > 0)
                {
                    var model = response.Model;
                    //PropertyCopier<R.Models.EmployeeInformation, EmploymentInformationHolder>.Copy(model, retValue);
                    retValue = new EmploymentInformationHolder()
                    {
                        CJI_EmployeeNo = model.EmployeeNo,
                        CJI_AccessId = model.AccessId,
                        EmployeeType = model.EmploymentTypeCode,
                        EmploymentStatus = model.EmploymentStatus,
                        ManpowerClassification = model.ManpowerClassification,
                        UnionMemberInt = (Convert.ToBoolean(model.UnionMember) ? 1 : 0),
                        Position = model.Position,
                        JobRank = model.JobRankCode,
                        JobGrade = model.JobGradeCode,
                        JobLevel = model.JobLevelCode,
                        ManHourClassification = model.ManHourClassificationCode,

                        Company = model.Company,
                        Branch = model.Branch,
                        Department = model.Department,
                        Office = model.Office,
                        Unit = model.UnitCode,
                        Division = model.DivisionCode,
                        Group = model.GroupsCode,
                        District = model.DistrictCode,
                        Location = model.Location,
                        Project = model.ProjectName,
                        CostCenter = model.CostCenterCode,
                        Line = model.LineIdCode,
                        Team = model.TeamCode,
                        ChargeCode = model.ChargeCode,

                        HireDate = model.HireDate.GetValueOrDefault().DateToString(),
                        ReglarizationDate = model.ReglarizationDate.GetValueOrDefault().DateToString(),
                        EndOfContractDate = model.EndOfContractDate.GetValueOrDefault().DateToString(),
                        DueTo = model.DueTo,
                        Reason = model.DueToReason,
                        RED_Others = model.RED_Others,
                        SeparationDate = model.SeparationDate.GetValueOrDefault().DateToString(),
                        ClearanceDate = model.ClearanceDate.GetValueOrDefault().DateToString(),
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        #endregion employment information

        public async Task<Xamarin.Forms.ImageSource> GetProfileImage(long profileId = 0)
        {
            Xamarin.Forms.ImageSource retVal = null;

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                var userInfo = PreferenceHelper.UserInfo();

                var id = (profileId == 0 ? userInfo.ProfileId : profileId);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.ProfileImage}/{id}"
                };

                var response = await genericRepository_.GetAsync<string>(builder.ToString());

                if (response.Contains("base64"))
                {
                    var attachment = response.Split(';')[1];
                    var sourceString = attachment.Replace("base64,", "");

                    retVal = Xamarin.Forms.ImageSource.FromStream(
                        () => new MemoryStream(Convert.FromBase64String(sourceString)));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
            }

            return retVal;
        }
    }
}