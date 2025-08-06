using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.AdminApiDto
{
    public class UserIdPaginationDto : PaginationModel
    {
        public int UserId { get; set; }
    }

    public class ReferredUserPaginationDto: PaginationModel
    {
        public string ReferalCode { get; set; }
    }

    public class PackagePaginationDto : PaginationModel
    {
        public int PackageId { get; set; }
    }
}
    