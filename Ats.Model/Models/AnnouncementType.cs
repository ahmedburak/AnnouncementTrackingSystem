using System.Collections.Generic;

#nullable disable

namespace Ats.Model.Models
{
    public partial class AnnouncementType
    {
        public AnnouncementType()
        {
            AnnouncementDefinitions = new HashSet<AnnouncementDefinition>();
            Announcements = new HashSet<Announcement>();
        }

        public byte PkId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AnnouncementDefinition> AnnouncementDefinitions { get; set; }
        public virtual ICollection<Announcement> Announcements { get; set; }
    }
}
