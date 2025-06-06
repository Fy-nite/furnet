from flask import Flask, jsonify, request
from datetime import datetime, timedelta
import json
import os
import re
from pathlib import Path

app = Flask(__name__)

# Directory to store project files
PROJECTS_DIR = Path("projects")
PROJECTS_DIR.mkdir(exist_ok=True)

def sanitize_package_name_for_directory(package_name):
    """Sanitize package name to be safe for use as directory name"""
    # Replace invalid characters with underscores
    sanitized = re.sub(r'[<>:"/\\|?*]', '_', package_name)
    # Replace multiple underscores with single underscore
    sanitized = re.sub(r'_+', '_', sanitized)
    # Remove leading/trailing underscores
    sanitized = sanitized.strip('_')
    return sanitized

def load_packages_from_files():
    """Load all packages from individual project directories"""
    packages = []
    
    if not PROJECTS_DIR.exists():
        return get_default_packages()
    
    for project_dir in PROJECTS_DIR.iterdir():
        if project_dir.is_dir():
            furconfig_path = project_dir / "furconfig.json"
            if furconfig_path.exists():
                try:
                    with open(furconfig_path, 'r', encoding='utf-8') as f:
                        package_data = json.load(f)
                        # Add metadata for website functionality
                        package_data.update({
                            "downloads": 0,
                            "uploaded": datetime.now() - timedelta(days=30),
                            "updated": datetime.now() - timedelta(days=2)
                        })
                        packages.append(package_data)
                        print(f"Loaded package: {package_data['name']}")
                except Exception as e:
                    print(f"Error loading {furconfig_path}: {e}")
    
    # If no packages found, use defaults
    if not packages:
        packages = get_default_packages()
        # Save default packages to files
        for package in packages:
            save_package_to_file(package)
    
    return packages

def save_package_to_file(package_data):
    """Save a package to its individual furconfig.json file"""
    package_name = package_data["name"]
    sanitized_name = sanitize_package_name_for_directory(package_name)
    project_dir = PROJECTS_DIR / sanitized_name
    project_dir.mkdir(exist_ok=True)
    
    furconfig_path = project_dir / "furconfig.json"
    
    # Create furconfig data (exclude website metadata)
    furconfig_data = {
        "name": package_data["name"],
        "version": package_data["version"],
        "authors": package_data["authors"],
        "Supported_Platforms": package_data.get("Supported_Platforms", []),
        "description": package_data.get("description", ""),
        "long_description": package_data.get("long_description", ""),
        "license": package_data.get("license", ""),
        "license_url": package_data.get("license_url", ""),
        "keywords": package_data.get("keywords", []),
        "tags": package_data.get("tags", []),
        "homepage": package_data["homepage"],
        "issue_tracker": package_data["issue_tracker"],
        "git": package_data["git"],
        "installer": package_data["installer"],
        "dependencies": package_data["dependencies"]
    }
    
    try:
        with open(furconfig_path, 'w', encoding='utf-8') as f:
            json.dump(furconfig_data, f, indent=2, ensure_ascii=False)
        print(f"Saved package to: {furconfig_path}")
        return True
    except Exception as e:
        print(f"Error saving package {package_name}: {e}")
        return False

def get_default_packages():
    """Return the original default packages"""
    return [
        {
            "name": "web-framework",
            "version": "2.1.0",
            "authors": ["devuser1", "contributor2"],
            "Supported_Platforms": ["linux", "macos", "windows"],
            "description": "Lightweight web framework",
            "long_description": "A comprehensive web framework designed for modern applications with built-in routing, middleware support, and templating.",
            "license": "MIT",
            "license_url": "https://opensource.org/license/mit/",
            "keywords": ["web", "framework", "http"],
            "tags": ["web", "framework"],
            "homepage": "https://example.com/web-framework",
            "issue_tracker": "https://github.com/devuser1/web-framework/issues",
            "git": "https://github.com/devuser1/web-framework.git",
            "installer": "install.sh",
            "dependencies": ["make@latest", "gcc@9.0.0"],
            "downloads": 12550,
            "uploaded": datetime.now() - timedelta(days=30),
            "updated": datetime.now() - timedelta(days=2)
        },
        {
            "name": "json-parser",
            "version": "1.5.3",
            "authors": ["jsondev"],
            "Supported_Platforms": ["linux", "macos"],
            "description": "Fast JSON parsing library",
            "long_description": "High-performance JSON parser with streaming support and minimal memory footprint. Ideal for processing large JSON files.",
            "license": "Apache-2.0",
            "license_url": "https://www.apache.org/licenses/LICENSE-2.0",
            "keywords": ["json", "parser", "streaming"],
            "tags": ["json", "parser"],
            "homepage": "https://example.com/json-parser",
            "issue_tracker": "https://github.com/jsondev/json-parser/issues",
            "git": "https://github.com/jsondev/json-parser.git",
            "installer": "setup.py",
            "dependencies": ["make@latest"],
            "downloads": 8920,
            "uploaded": datetime.now() - timedelta(days=45),
            "updated": datetime.now() - timedelta(days=5)
        },
        {
            "name": "crypto-utils",
            "version": "3.0.1",
            "authors": ["securedev", "cryptoexpert"],
            "Supported_Platforms": ["linux", "windows"],
            "description": "Cryptographic utilities",
            "long_description": "Secure cryptographic library providing encryption, hashing, and digital signature capabilities with modern algorithms.",
            "license": "BSD-3-Clause",
            "license_url": "https://opensource.org/licenses/BSD-3-Clause",
            "keywords": ["crypto", "encryption", "security"],
            "tags": ["crypto", "security"],
            "homepage": "https://example.com/crypto-utils",
            "issue_tracker": "https://gitlab.com/securedev/crypto-utils/issues",
            "git": "https://gitlab.com/securedev/crypto-utils.git",
            "installer": "configure.sh",
            "dependencies": ["openssl@1.1.1", "make@latest"],
            "downloads": 2340,
            "uploaded": datetime.now() - timedelta(days=60),
            "updated": datetime.now() - timedelta(days=1)
        },
        {
            "name": "database-connector",
            "version": "4.2.1",
            "authors": ["dbdev", "sqlexpert", "dataengineer"],
            "Supported_Platforms": ["linux", "macos", "windows"],
            "description": "Database connectivity library",
            "long_description": "Universal database connector supporting MySQL, PostgreSQL, SQLite, and more with connection pooling and transaction management.",
            "license": "GPL-3.0",
            "license_url": "https://www.gnu.org/licenses/gpl-3.0.html",
            "keywords": ["database", "sql", "connector"],
            "tags": ["database", "sql"],
            "homepage": "https://example.com/database-connector",
            "issue_tracker": "https://github.com/dbdev/database-connector/issues",
            "git": "https://github.com/dbdev/database-connector.git",
            "installer": "db_setup.sh",
            "dependencies": ["libpq@13.0", "sqlite@3.36"],
            "downloads": 1567,
            "uploaded": datetime.now() - timedelta(days=20),
            "updated": datetime.now() - timedelta(days=3)
        },
        {
            "name": "ui-toolkit",
            "version": "1.0.0",
            "authors": ["uimaster"],
            "Supported_Platforms": ["linux", "macos"],
            "description": "Modern UI toolkit",
            "long_description": "Cross-platform UI toolkit with modern widgets, theming support, and responsive design capabilities.",
            "license": "LGPL-2.1",
            "license_url": "https://www.gnu.org/licenses/lgpl-2.1.html",
            "keywords": ["ui", "toolkit", "widgets"],
            "tags": ["ui", "toolkit"],
            "homepage": "https://example.com/ui-toolkit",
            "issue_tracker": "https://github.com/uimaster/ui-toolkit/issues",
            "git": "https://github.com/uimaster/ui-toolkit.git",
            "installer": "ui_install.sh",
            "dependencies": ["gtk@3.0", "cairo@1.16"],
            "downloads": 445,
            "uploaded": datetime.now() - timedelta(days=10),
            "updated": datetime.now() - timedelta(days=7)
        }
    ]

# Load packages from files on startup
packages_data = load_packages_from_files()

def get_furconfig(package_data):
    """Convert package data to furconfig format"""
    return {
        "name": package_data["name"],
        "version": package_data["version"],
        "authors": package_data["authors"],
        "Supported_Platforms": package_data.get("Supported_Platforms", []),
        "description": package_data.get("description", ""),
        "long_description": package_data.get("long_description", ""),
        "license": package_data.get("license", ""),
        "license_url": package_data.get("license_url", ""),
        "keywords": package_data.get("keywords", []),
        "tags": package_data.get("tags", []),
        "homepage": package_data["homepage"],
        "issue_tracker": package_data["issue_tracker"],
        "git": package_data["git"],
        "installer": package_data["installer"],
        "dependencies": package_data["dependencies"]
    }

@app.route('/api/v1/packages/<package_name>')
@app.route('/api/v1/packages/<package_name>/<version>')
def get_package(package_name, version=None):
    """Get package information - returns furconfig.json"""
    package = next((p for p in packages_data if p["name"] == package_name), None)
    
    if not package:
        return jsonify({"error": f"Package '{package_name}' not found"}), 404
    
    if version and package["version"] != version:
        return jsonify({"error": f"Version '{version}' not found for package '{package_name}'"}), 404
    
    return jsonify(get_furconfig(package))

@app.route('/api/v1/packages')
def get_packages():
    """Get all packages with optional sorting and searching"""
    sort_by = request.args.get('sort', '')
    search_query = request.args.get('search', '')
    details = request.args.get('details', '').lower() == 'true'
    
    filtered_packages = packages_data.copy()
    
    # Apply search filter
    if search_query:
        search_lower = search_query.lower()
        filtered_packages = [
            p for p in filtered_packages 
            if search_lower in p["name"].lower() or 
               search_lower in " ".join(p["authors"]).lower() or
               search_lower in p["description"].lower() or
               any(search_lower in keyword.lower() for keyword in p["keywords"]) or
               any(search_lower in tag.lower() for tag in p["tags"])
        ]
    
    # Apply sorting
    if sort_by == "mostDownloads":
        filtered_packages.sort(key=lambda x: x["downloads"], reverse=True)
    elif sort_by == "leastDownloads":
        filtered_packages.sort(key=lambda x: x["downloads"])
    elif sort_by == "recentlyUpdated":
        filtered_packages.sort(key=lambda x: x["updated"], reverse=True)
    elif sort_by == "recentlyUploaded":
        filtered_packages.sort(key=lambda x: x["uploaded"], reverse=True)
    elif sort_by == "oldestUpdated":
        filtered_packages.sort(key=lambda x: x["updated"])
    elif sort_by == "oldestUploaded":
        filtered_packages.sort(key=lambda x: x["uploaded"])
    else:
        # Default: alphabetical by name
        filtered_packages.sort(key=lambda x: x["name"])
    
    # Return detailed response if requested
    if details:
        # Convert datetime objects to ISO strings for JSON serialization
        detailed_packages = []
        for p in filtered_packages:
            package_copy = p.copy()
            if 'uploaded' in package_copy and hasattr(package_copy['uploaded'], 'isoformat'):
                package_copy['uploaded'] = package_copy['uploaded'].isoformat()
            if 'updated' in package_copy and hasattr(package_copy['updated'], 'isoformat'):
                package_copy['updated'] = package_copy['updated'].isoformat()
            detailed_packages.append(get_furconfig(package_copy))
        
        return jsonify({
            "package_count": len(filtered_packages),
            "packages": [p["name"] for p in filtered_packages],
            "package_details": detailed_packages
        })
    
    return jsonify({
        "package_count": len(filtered_packages),
        "packages": [p["name"] for p in filtered_packages]
    })

@app.route('/api/v1/packages', methods=['POST'])
def upload_package():
    """Upload a new package"""
    try:
        furconfig = request.get_json()
        
        if not furconfig:
            return jsonify({"error": "No JSON data provided"}), 400
        
        # Validate required fields
        required_fields = ["name", "version", "authors", "homepage", "issue_tracker", "git", "installer", "dependencies"]
        missing_fields = [field for field in required_fields if field not in furconfig or not furconfig[field]]
        
        if missing_fields:
            return jsonify({"error": f"Missing required fields: {', '.join(missing_fields)}"}), 400
        
        # Validate version format (basic x.y.z check)
        version_parts = furconfig["version"].split(".")
        if len(version_parts) != 3 or not all(part.isdigit() for part in version_parts):
            return jsonify({"error": "Version must be in x.y.z format"}), 400
        
        # Check if package already exists
        existing_package = next((p for p in packages_data if p["name"] == furconfig["name"]), None)
        if existing_package:
            return jsonify({"error": f"Package '{furconfig['name']}' already exists"}), 409
        
        # Add package to mock data with defaults for optional fields
        new_package = {
            **furconfig,
            "Supported_Platforms": furconfig.get("Supported_Platforms", []),
            "description": furconfig.get("description", ""),
            "long_description": furconfig.get("long_description", ""),
            "license": furconfig.get("license", ""),
            "license_url": furconfig.get("license_url", ""),
            "keywords": furconfig.get("keywords", []),
            "tags": furconfig.get("tags", []),
            "downloads": 0,
            "uploaded": datetime.now(),
            "updated": datetime.now()
        }
        
        # Save to file
        if save_package_to_file(new_package):
            packages_data.append(new_package)
            
            # Print the complete package information
            print("\n=== New Package Uploaded ===")
            print(f"Name: {new_package['name']}")
            print(f"Version: {new_package['version']}")
            print(f"Authors: {', '.join(new_package['authors'])}")
            print(f"Supported Platforms: {', '.join(new_package['Supported_Platforms'])}")
            print(f"Description: {new_package['description']}")
            print(f"Dependencies: {', '.join(new_package['dependencies'])}")
            print(f"Homepage: {new_package['homepage']}")
            print(f"Git Repository: {new_package['git']}")
            sanitized_name = sanitize_package_name_for_directory(new_package['name'])
            print(f"Saved to: projects/{sanitized_name}/furconfig.json")
            print(f"Total packages now: {len(packages_data)}")
            print("=" * 30)
            print(f"✅ Package '{furconfig['name']}' v{furconfig['version']} uploaded successfully")
            
            return jsonify(get_furconfig(new_package)), 201
        else:
            return jsonify({"error": "Failed to save package to file"}), 500
        
    except Exception as e:
        return jsonify({"error": f"Server error: {str(e)}"}), 500

@app.route('/health')
def health_check():
    """Health check endpoint"""
    return jsonify({
        "status": "healthy",
        "timestamp": datetime.now().isoformat(),
        "total_packages": len(packages_data)
    })

@app.route('/')
def index():
    """API documentation endpoint"""
    return jsonify({
        "message": "FUR Package Registry API",
        "version": "1.0.0",
        "endpoints": {
            "GET /api/v1/packages": "List all packages",
            "GET /api/v1/packages/{name}": "Get package details",
            "GET /api/v1/packages/{name}/{version}": "Get specific package version",
            "POST /api/v1/packages": "Upload a new package",
            "GET /health": "Health check"
        }
    })

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5001)
