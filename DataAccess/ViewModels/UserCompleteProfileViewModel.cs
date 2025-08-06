using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.ViewModels
{
    public class UserCompleteProfileViewModel: UserModel
    {
        public UserPackageViewModel Package { get; set; }
        public List<SocialMediaModel> SocialMedia { get; set; } = new List<SocialMediaModel>();
        public List<string> Gallery { get; set; }   
        public List<UserEducationModel> Educations { get; set; } = new List<UserEducationModel>();
        public List<UserProfessionalModel> Experiencee { get; set; } = new List<UserProfessionalModel>();
        public List<UserCertificationModel> TrainingCertification { get; set; } = new List<UserCertificationModel>();
        public List<ClientTestimonialModel> ClientTestimonials { get; set; } = new List<ClientTestimonialModel>();
        public List<TeamViewModel> Teams { get; set; } = new List<TeamViewModel>();
        public List<YouTubeModel> YoutubeVideos { get; set; } = new List<YouTubeModel>();
        public UpiDetailsModel UpiDetails { get; set; }
        public List<BlogModel> Blogs { get; set; } = new List<BlogModel>();
        public ScheduleOpenWeekDayModel MeetingWeedDays { get; set; }
        public ShowHideSectionsModel ShowHideSections { get; set; } = new ShowHideSectionsModel();
    }

    public class ShowHideSectionsModel
    {
        public bool PersonalInfo { get; set; }
        public bool UploadSection { get; set; }
        public bool Contact { get; set; }
        public bool SocialMedia { get; set; }
        public bool About { get; set; }
        public bool Gallery { get; set; }
        public bool Education { get; set; }
        public bool Experiencee { get; set; }
        public bool TrainingCertification { get; set; }
        public bool ClientTestimonials { get; set; }
        public bool Teams { get; set; }
        public bool YoutubeVideos { get; set; }
        public bool PaymentQR { get; set; }
        public bool Blogs { get; set; }
        public bool Adhaar { get; set; }
        public bool MeetingRequest { get; set; }
        public bool ProfileTemplates { get; set; }
        public bool ProfileAnalytics { get; set; }
        public bool Leads { get; set; }
    }
}

