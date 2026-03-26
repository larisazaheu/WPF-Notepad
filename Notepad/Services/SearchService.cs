using Notepad.Models;

namespace Notepad.Services
{
    public class SearchService
    {
        public List<(MyTabModel tab, int index)> FindAll(string searchText, IEnumerable<MyTabModel> targets)
        {
            List<(MyTabModel, int)> results = new List<(MyTabModel, int)>();

            foreach (MyTabModel tab in targets)
            {
                if (tab.Content == null) continue;

                int searchStartIndex = 0;

                while (true)
                {
                    int foundIndex = tab.Content.IndexOf(
                        searchText,
                        searchStartIndex,
                        StringComparison.OrdinalIgnoreCase);

                    if (foundIndex < 0) break;

                    results.Add((tab, foundIndex));
                    searchStartIndex = foundIndex + 1;
                }
            }

            return results;
        }

        public bool ReplaceFirst(MyTabModel tab, string searchText, string replacementText)
        {
            if (tab.Content == null) return false;

            int foundIndex = tab.Content.IndexOf(
                searchText,
                StringComparison.OrdinalIgnoreCase);

            if (foundIndex < 0) return false;

            tab.Content = tab.Content
                .Remove(foundIndex, searchText.Length)
                .Insert(foundIndex, replacementText);

            return true;
        }

        public int ReplaceAll(string searchText, string replacementText, IEnumerable<MyTabModel> targets)
        {
            int totalReplacements = 0;

            foreach (MyTabModel tab in targets)
            {
                if (tab.Content == null) continue;

                int replacementsInTab = (tab.Content.Length -
                    tab.Content.Replace(
                        searchText, "",
                        StringComparison.OrdinalIgnoreCase).Length)
                    / searchText.Length;

                totalReplacements += replacementsInTab;

                tab.Content = tab.Content.Replace(
                    searchText,
                    replacementText,
                    StringComparison.OrdinalIgnoreCase);
            }

            return totalReplacements;
        }
    }
}