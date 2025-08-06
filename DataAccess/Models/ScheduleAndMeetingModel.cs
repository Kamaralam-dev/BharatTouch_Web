using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{

    public class ScheduleOpenDayModel
    {
        public int DayId { get; set; }
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string multipleOpenDays { get; set; }
    }

    public class ScheduleOpenWeekDayModel
    {
        public int WeekDaysId { get; set; }
        public int UserId { get; set; }
        public Boolean Sun { get; set; }
        public Boolean Mon { get; set; }
        public Boolean Tue { get; set; }
        public Boolean Wed { get; set; }
        public Boolean Thu { get; set; }
        public Boolean Fri { get; set; }
        public Boolean Sat { get; set; }
    }
}
