// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Theme management
document.addEventListener('DOMContentLoaded', function() {
    const themeToggle = document.getElementById('themeToggle');
    const themeIcon = document.getElementById('themeIcon');
    const htmlElement = document.documentElement;
    
    // Check for saved theme preference or default to light mode
    const savedTheme = localStorage.getItem('theme') || 'light';
    
    // Apply the saved theme
    function applyTheme(theme) {
        if (theme === 'dark') {
            htmlElement.setAttribute('data-theme', 'dark');
            themeIcon.className = 'bi bi-sun';
        } else {
            htmlElement.removeAttribute('data-theme');
            themeIcon.className = 'bi bi-moon';
        }
    }
    
    // Initialize theme
    applyTheme(savedTheme);
    
    // Toggle theme
    themeToggle.addEventListener('click', function() {
        const currentTheme = htmlElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        
        applyTheme(newTheme);
        localStorage.setItem('theme', newTheme);
    });
    
    // Listen for system theme changes
    if (window.matchMedia) {
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function(e) {
            // Only apply system theme if user hasn't set a preference
            if (!localStorage.getItem('theme')) {
                applyTheme(e.matches ? 'dark' : 'light');
            }
        });
    }

    // Cache management functionality
    function clearPackageCache() {
        if (confirm('Are you sure you want to clear the package cache?')) {
            fetch('/api/v1/packages/cache', { 
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(response => {
                if (response.ok) {
                    alert('Cache cleared successfully');
                    location.reload();
                } else {
                    alert('Failed to clear cache');
                }
            })
            .catch(error => {
                console.error('Error clearing cache:', error);
                alert('Error clearing cache');
            });
        }
    }

    // Expose cache clear function globally
    window.clearPackageCache = clearPackageCache;
});

// Existing site.js code (if any) would go here
