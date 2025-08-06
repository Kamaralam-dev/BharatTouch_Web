using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class UserPackageHistoryViewModel
    {
        public int ChangeHistoryId { get; set; }
        public int NewPackageId { get; set; }
        public string PackageName { get; set; }
        public string PackageDescription { get; set; }
        public bool Deleted { get; set; }
        public decimal PackageCost { get; set; }
        public int OldPackageId { get; set; }
        public string OldPackageName { get; set; }
        public string OldPackageDescription { get; set; }
        public decimal OldPackageCost { get; set; }
        public bool OldPackageDeleted { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string EmailId { get; set; }
        public string Phone { get; set; }
        public string NumberCode { get; set; }
        public int ChangedBy { get; set; }
        public string ChangedByFullName { get; set; }
        public string ChangedByEmailId { get; set; }
        public string ChangedByPhone { get; set; }
        public string ChangedByNumberCode { get; set; }
        public bool IsAdminChangedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
