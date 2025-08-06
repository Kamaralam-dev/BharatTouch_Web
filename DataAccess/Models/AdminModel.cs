using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class AdminModel
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string UserType { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public bool IsActive { get; set; }
        public string UrlCode { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ActivatedOn { get; set; }
        public DateTime? LastLogin { get; set; }

        //profile
        public int ProfileId { get; set; }
        public string ProfileImage { get; set; }
        public string CoverImage { get; set; }
        public string BestFitCoverImage { get; set; }
        public string Company { get; set; }
        public int CompanyTypeParentId { get; set; }
        public int CompanyTypeId { get; set; }
        public string CompanyType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CurrentDesignation { get; set; }
        public string Tagline { get; set; }
        public string Website { get; set; }
        public string PersonalEmail { get; set; }
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
        public int WhatsAppCountryId { get; set; }
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
        public decimal KnowledgePercent1 { get; set; }
        public decimal KnowledgePercent2 { get; set; }
        public decimal KnowledgePercent3 { get; set; }
        public decimal KnowledgePercent4 { get; set; }
        public decimal KnowledgePercent5 { get; set; }
        public decimal KnowledgePercent6 { get; set; }
        [Display(Name = "Show About")]
        public bool ShowAbout { get; set; }
        [Display(Name = "Show Experience")]
        public bool ShowExperience { get; set; }
        [Display(Name = "Show Education")]
        public bool ShowEducation { get; set; }
        [Display(Name = "Show Social")]
        public bool ShowSocial { get; set; }
        [Display(Name = "Show Skill")]
        public bool ShowSkill { get; set; }
        public string MenuLink { get; set; }
        public bool HideGallery { get; set; }
        public bool HideTeam { get; set; }
        public bool HideTestimonial { get; set; }
        public string Displayname { get; set; }
        public bool HideBlog { get; set; }
        public bool HideYouTube { get; set; }
        public bool IsAdmin { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int PageViewed { get; set; }
        public string GoogleId { get; set; }
        public string MicrosoftId { get; set; }
        public string AuthToken { get; set; }
        public string CompanyDisplayName { get; set; }
        public bool IsCompanyAdmin { get; set; }
    }
}
