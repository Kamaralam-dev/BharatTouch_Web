using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.AdminApiDto
{
    public class AboutInfoDto
    {
        public int UserId { get; set; }
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
    }
}
