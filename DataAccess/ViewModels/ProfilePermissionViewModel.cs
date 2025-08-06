using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class ProfilePermissionViewModel
    {
        public ProfileDetailModel ProfileDetails { get; set; }
        public SummaryCountsViewModel SummaryCounts { get; set; }
        public UpiDetailViewModel UpiDetails { get; set; }
        public List<SocialMediaViewModel> SocialMedia { get; set; }
        public OpenWeekDayViewModel OpenWeekDays { get; set; }
        public OauthTokenViewModel OauthTokens { get; set; }
        public PackageDetailsViewModel PackageDetails { get; set; }
    }

    public class ProfileDetailModel
    {
        public int UserID { get; set; }
        public int CompanyId { get; set; }
        public string EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CurrentDesignation { get; set; }
        public string ProfileImage { get; set; }
        public string CoverImage { get; set; }
        public string BestFitCoverImage { get; set; }
        public string Tagline { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string Zip { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string NumberCode { get; set; }
        public string UpdatedOn { get; set; }
        public string Skype { get; set; }
        public int? WhatsAppCountryId { get; set; }
        public string WhatsappNumberCode { get; set; }
        public string Whatsapp { get; set; }
        public string PortfolioLink { get; set; }
        public string ResumeLink { get; set; }
        public string ServicesLink { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string AboutDescription { get; set; }
        public string SkillName1 { get; set; }
        public string SkillName2 { get; set; }
        public string SkillName3 { get; set; }
        public string SkillName4 { get; set; }
        public string SkillName5 { get; set; }
        public string SkillName6 { get; set; }
        public string KnowledgePercent1 { get; set; }
        public string KnowledgePercent2 { get; set; }
        public string KnowledgePercent3 { get; set; }
        public string KnowledgePercent4 { get; set; }
        public string KnowledgePercent5 { get; set; }
        public string KnowledgePercent6 { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public int CompanyTypeParentId { get; set; }
        public int CompanyTypeId { get; set; }
        public string CompanyType { get; set; }
        public string Website { get; set; }
        public DateTime? StartDate { get; set; }
        public string IsCurrent { get; set; }
        public string ProfAddress1 { get; set; }
        public string ProfAddress2 { get; set; }
        public string ProfCity { get; set; }
        public string ProfStateName { get; set; }
        public string ProfZip { get; set; }
        public string ProfCountry { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ShowAbout { get; set; }
        public bool ShowExperience { get; set; }
        public bool ShowEducation { get; set; }
        public bool ShowSocial { get; set; }
        public bool ShowSkill { get; set; }
        public string MenuLink { get; set; }
        public string UserType { get; set; }
        public bool HideGallery { get; set; }
        public bool HideTeam { get; set; }
        public bool HideBlog { get; set; }
        public bool HideYouTube { get; set; }
        public bool HideTestimonial { get; set; }
        public bool HideCertification { get; set; }
        public bool HideAdhaar { get; set; }
        public string GoogleId { get; set; }
        public string MicrosoftId { get; set; }
        public string AdhaarFrontImgPath { get; set; }
        public string AdhaarBackImgPath { get; set; }
        public string ReferalCode { get; set; }
        public string ReferredByCode { get; set; }
        public string AdhaarFrontThumbnailPath { get; set; }
        public string AdhaarBackThumbnailPath { get; set; }
        public string ProfileImageThumbnailPath { get; set; }
        public string CoverImageThumbnailPath { get; set; }
        public string SelectedTemplateName { get; set; }
        public int ProfileTemplateId { get; set; }
        public string WorkPhone { get; set; }
        public int WorkPhoneCountryId { get; set; }
        public string WorkPhoneCountryCode { get; set; }
        public string WorkPhoneNumberCode { get; set; }
        public string OtherPhone { get; set; }
        public int OtherPhoneCountryId { get; set; }
        public string OtherPhoneCountryCode { get; set; }
        public string OtherPhoneNumberCode { get; set; }
        public string OfficeAddress1 { get; set; }
        public string OfficeAddress2 { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeStatename { get; set; }
        public string OfficeZip { get; set; }
        public string OfficeCountry { get; set; }
        public int CheckUserPaymentStatus { get; set; }
        public int NfcCardColorId { get; set; }
    }  

    public class SummaryCountsViewModel
    {
        public int TotalEducationCount { get; set; }
        public int TotalProfessionalCount { get; set; }
        public int TotalCertificationCount { get; set; }
        public int TotalTeamCount { get; set; }
        public int TotalYouTubeVideo { get; set; }
        public int TotalBlogCount { get; set; }
        public int TotalLeadCount { get; set; }
        public int TotalClientTestimonialCount { get; set; }
    }

    public class UpiDetailViewModel
    {
        public string UPIId { get; set; }
        public string PayeeName { get; set; }
        public string QrImage { get; set; }
        public string QrImageThumbnailPath { get; set; }
    }

    public class SocialMediaViewModel
    {
        public int SocialMediaId { get; set; }
        public string SocialMedia { get; set; }
        public string Url { get; set; }
    }

    public class OpenWeekDayViewModel
    {
        public int WeekDaysId { get; set; } 
        public bool Sun { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
    }

    public class OauthTokenViewModel
    {
        public int TokenId { get; set; }
        public string GoogleAccessToken { get; set; }
        public string GoogleRefreshToken { get; set; }
        public string MicrosoftAccessToken { get; set; }
        public string MicrosoftRefreshToken { get; set; }
    }

    public class PackageDetailsViewModel
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public string PackageDescription { get; set; }
        public decimal PackageCost { get; set; }
        public int AllowedImages { get; set; }
        public int AllowedVideos { get; set; }
        public bool AllowContactUsSection { get; set; }
        public bool AllowCalendarSection { get; set; }
        public bool AllowBlogSection { get; set; }
        public bool AllowTeamSection { get; set; }
        public bool AllowUploadDetailSection { get; set; }
        public bool AllowTestimonialSection { get; set; }
        public bool AllowTrainingCertificateSection { get; set; }
        public bool AllowProfileViewAnalytics { get; set; }
        public bool AllowSocialMedia { get; set; }
        public bool AllowAdhaarCard { get; set; }
        public bool AllowedEducationSection { get; set; }
        public bool AllowedExperienceSection { get; set; }
        public bool AllowedPaymentSection { get; set; }
        public bool AllowedProfileTemplateSection { get; set; }
    }

}
