using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class RnauraUserModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int SuperviserId { get; set; }
        public string SuperviserName { get; set; }
        public string Position { get; set; }
        public DateTime? DateOfHire { get; set; }
        public DateTime? TerminationDate { get; set; }
        public Boolean UserStatus { get; set; }
        public int UserTypeId { get; set; }
        public string UserType { get; set; }
        public string ProfilePicture { get; set; }
    }
}
