# Furconfig spec
Furconfig files are used to store information about a package. Furconfig files are stored in json data and the file would be called `furconfig.json`

A basic furconifg.json would be
```json
{
    "name": "ExampleProject",
    "version": "1.0.0",
    "authors": ["Me", "Myself", "I"],
    "Supported_Platforms": ["linux", "macos", "windows"],
    "description": "This is an example package for FUR",
    "long_description": "This is a long description for the example package. It can be multiple lines and should give more information about the package.",
    "license": "MIT",
    "license_url": "https://opensource.org/license/mit/",
    "keywords": ["example", "fur", "package"],
    "tags": ["example", "fur", "package"],
    "homepage": "https://git.carsoncoder.com/Finite/FUR-Example-Repo",
    "issue_tracker": "https://git.carsoncoder.com/Finite/FUR-Example-Repo/issues",
    "git": "https://git.carsoncoder.com/Finite/FUR-Example-Repo.git",
    "installer": "install.sh",
    "dependencies": [
        "make@latest",
        "gcc@1.0.0"
    ]
}
```

name is the name of the package (no spaces allowed) <br>
version is the package version (x.y.z format required) <br>
authors is a array containing the people that made the package <br>
homepage is the main page for the package <br>
issue_tracker is a link where issues can be submitted <br>
git is a link the the git repo that is used by fur to download the package <br>
installer is a command that gets ran when program is installed (ran as sudo) <br>
dependencies is a list containing the name and version of a package (name@version) <br>

All fields are required