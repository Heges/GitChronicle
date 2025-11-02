namespace Changeloger.Models
{
    public class ChangelogItem
    {
        public long ChangelogItemId { get; set; }
        public string ChangelogItemDate { get; set; } = "";
        public string ChangelogItemTypeCommit { get; set; } = "";
        public string ChangelogItemTitle { get; set; } = "";
        public string ChangelogItemDescription { get; set; } = "";
        public string ChangelogItemAuthor { get; set; } = "";
        public string ChangelogItemHashCommit { get; set; } = "";

        public bool SubjectContains(string containMessage)
        {
            return ChangelogItemTitle.ToLower().Contains(containMessage.ToLower());
        }

        public bool BodyContains(string containMessage)
        {
            return ChangelogItemDescription.ToLower().Contains(containMessage.ToLower());
        }
    }
}
