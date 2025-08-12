using System;

namespace purrnet.Documentation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DocumentationPageAttribute : Attribute
    {
        public string Title { get; }
        public string Category { get; }
        public DocumentationPageAttribute(string title, string category = null)
        {
            Title = title;
            Category = category;
        }
    }
}
