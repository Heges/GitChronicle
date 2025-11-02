using Changloger.Utils;

namespace Changeloger.Models
{
    public class Changelog
    {
        public string Platform { get; private set; }
        public string ProjectName { get; private set; }
        public List<ChangelogItem> Items { get; private set; }
        private readonly List<ChangelogItem> _cachedItems;

        public Changelog(string platform, string project, List<ChangelogItem> items)
        {
            Platform = platform;
            ProjectName = project;
            _cachedItems = items;
            Items = items;
        }

        public void RemoveEmptyAndMergeMessages()
        {
            Items = Items.Where(x =>
                !String.IsNullOrEmpty(x.ChangelogItemHashCommit)
                && !String.IsNullOrEmpty(x.ChangelogItemDate)
                && !String.IsNullOrEmpty(x.ChangelogItemDescription)
                && !String.IsNullOrEmpty(x.ChangelogItemTitle)
                && !String.IsNullOrEmpty(x.ChangelogItemTypeCommit)
                && !String.IsNullOrEmpty(x.ChangelogItemAuthor)
                && !x.SubjectContains("merge")
                && !x.BodyContains("merge")
                && !x.SubjectContains("revert"))
                    .Select(x => new ChangelogItem
                    {
                        ChangelogItemId = DateTime.UtcNow.Ticks,
                        ChangelogItemHashCommit = x.ChangelogItemHashCommit,
                        ChangelogItemDate = x.ChangelogItemDate.FormatingStrDate(),
                        ChangelogItemDescription = x.ChangelogItemDescription,
                        ChangelogItemTitle = x.ChangelogItemTitle,
                        ChangelogItemTypeCommit = x.ChangelogItemTypeCommit,
                        ChangelogItemAuthor = x.ChangelogItemAuthor
                    }).ToList();
        }
    }
}
