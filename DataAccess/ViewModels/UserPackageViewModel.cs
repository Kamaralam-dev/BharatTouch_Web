using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class UserPackageViewModel
    {
        public int LoggedUserId { get; set; }
        public int UserId { get; set; }
        public string EmailId { get; set; }
        public string UrlCode { get; set; }
        public string DisplayName { get; set; }
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
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int UserCount { get; set; }
    }
}
