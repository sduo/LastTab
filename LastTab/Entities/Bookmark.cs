using System;

namespace LastTab.Entities
{
    public sealed class Bookmark
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public long Timestamp { get; set; }

        public string User { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is not Bookmark) { return false; }
            return string.Equals(Id, (obj as Bookmark).Id, StringComparison.OrdinalIgnoreCase);
        }
    }
}
