using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using purrnet.Pages.Shared;

namespace purrnet.Pages.Documentation
{
    public class IndexModel : PageModel
    {
        public class DocEntry
        {
            public string Title { get; set; }
            public string Category { get; set; }
            public string Content { get; set; }
        }

        public List<DocEntry> DocumentationPages { get; set; } = new();
        public string SelectedContent { get; set; }
        public string SelectedTitle { get; set; }

        public void OnGet()
        {
            var page = Request.Query["page"].ToString();
            var docs = new List<DocEntry>();
            var methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(m => m.GetCustomAttribute<DocumentationPageAttribute>() != null);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<DocumentationPageAttribute>();
                var content = method.Invoke(null, null) as string;
                docs.Add(new DocEntry
                {
                    Title = attr.Title,
                    Category = attr.Category,
                    Content = content
                });
            }
            DocumentationPages = docs.OrderBy(d => d.Category).ThenBy(d => d.Title).ToList();
            var selected = DocumentationPages
                .FirstOrDefault(d => string.Equals(d.Title?.Trim(), page?.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? DocumentationPages.FirstOrDefault();
            SelectedContent = selected?.Content;
            SelectedTitle = selected?.Title;
        }
    }
}
