using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class LeadCommunicationViewModel
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public int CommunicationTypeId { get; set; }
        public string CommunicationTypeName { get; set; }
        public string CommunicatedByName { get; set; }
        public string FollowUpTypeName { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int CommunicatedBy { get; set; }
        public DateTime? CommunicatedOn { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public int FollowUpCommunicationTypeId { get; set; }
        public int CreatedBy { get; set; }
    }
}
