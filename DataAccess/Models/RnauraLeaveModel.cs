using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class RnauraLeaveModel
    {
        public int LeaveId { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int NoOfDays { get; set; }
        public string Reason { get; set; }
        public string ApprovalStatus { get; set; }
        public int ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string CommentByManager { get; set; }
        public string Document { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int SuperviserId { get; set; }
        public string Superviser { get; set; }
        public string ApprovedByName { get; set; }
    }

    public class RnauraLeaveTypesModel
    {
        public int LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
    }

    public class RnauraHolidayModel
    {
        public int HolidayId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
    }
}
