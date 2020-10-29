using System;

#nullable disable

namespace Ats.Data.Models
{
    public partial class Announcement
    {
        public int PkId { get; set; }
        public byte TypeId { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public DateTime Date { get; set; }

        public virtual AnnouncementType Type { get; set; }
    }
}
