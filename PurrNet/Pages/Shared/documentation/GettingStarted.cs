using purrnet.Documentation;
 
namespace purrnet.Pages.Shared.documentation
{
    public static class GettingStarted
    {
        [DocumentationPage("Getting Started", "General")]
        public static string Started() =>
            """
# Getting Started

Welcome to the Purr package manager documentation! This guide will help you get started.

- **Install:** Download from the [Downloads page](/Download).
- **First Run:** Open your terminal and type `purr --help`.

Enjoy!
""";
        [DocumentationPage("Usage", "General")]
        public static string usage() =>
            """
With Purr, you can install packages, manage dependencies, and more. Here are some basic commands to get you started:
- **Install a package:** `purr install <package-name>`
- **Update a package:** `purr update <package-name>`
- **Remove a package:** `purr remove <package-name>`
- **List installed packages:** `purr list`
- **Search for packages:** `purr search <query>`
""";
    }
}
