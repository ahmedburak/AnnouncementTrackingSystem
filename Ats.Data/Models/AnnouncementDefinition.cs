using System;
using System.Collections.Generic;

#nullable disable

namespace Ats.Data.Models
{
    public partial class AnnouncementDefinition
    {
        public int PkId { get; set; }
        public string Url { get; set; }
        public byte TypeId { get; set; }
        public string RowCssSelector { get; set; }
        public string ClickCssSelector { get; set; }
        public string Description { get; set; }

        public virtual AnnouncementType Type { get; set; }
    }
}
