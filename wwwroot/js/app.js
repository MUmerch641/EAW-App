// JavaScript interop functions for MAUI Hybrid app

window.blazorHelpers = {
    scrollToTop: function () {
        window.scrollTo(0, 0);
    },
    
    scrollToElement: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollIntoView({ behavior: 'smooth' });
        }
    },
    
    setFocus: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.focus();
        }
    },
    
    downloadFile: function (filename, contentType, content) {
        const blob = new Blob([content], { type: contentType });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    },
    
    showAlert: function (message) {
        alert(message);
    },
    
    confirmDialog: function (message) {
        return confirm(message);
    },
    
    hideLoadingScreen: function () {
        const loadingContainer = document.querySelector('.loading-container');
        if (loadingContainer) {
            loadingContainer.style.display = 'none';
        }
    }
};

// Initialize Blazor with error handling
window.addEventListener('DOMContentLoaded', () => {
    console.log('DOM fully loaded, Blazor should auto-start...');
    
    // Check if required elements exist
    const appElement = document.getElementById('app');
    const loadingContainer = document.querySelector('.loading-container');
    
    console.log('App element:', appElement);
    console.log('Loading container:', loadingContainer);
    
    // Check if Blazor is available
    if (typeof Blazor === 'undefined') {
        console.error('Blazor is not defined - blazor.webview.js may not have loaded');
        if (loadingContainer) {
            loadingContainer.innerHTML = `
                <div class="error-message">
                    <h3>Application Error</h3>
                    <p>Blazor framework not found.</p>
                    <p>The application files may not be properly installed.</p>
                    <p>Please try reinstalling the application.</p>
                    <button onclick="location.reload()">Retry</button>
                </div>
            `;
        }
        return;
    }
    
    console.log('Blazor is available, waiting for auto-start...');
    
    // Fallback: try to start Blazor manually after a delay if it hasn't started
    setTimeout(() => {
        if (!window.Blazor._internal || !window.Blazor._internal.runtime) {
            console.log('Blazor auto-start may have failed, trying manual start...');
            try {
                Blazor.start().then(() => {
                    console.log('Blazor started successfully via fallback');
                    // Hide loading screen
                    if (loadingContainer) {
                        loadingContainer.style.display = 'none';
                    }
                }).catch((error) => {
                    console.error('Fallback Blazor start failed:', error);
                    showError(error);
                });
            } catch (error) {
                console.error('Exception during fallback Blazor startup:', error);
                showError(error);
            }
        } else {
            console.log('Blazor appears to be running from auto-start');
            // Hide loading screen
            if (loadingContainer) {
                loadingContainer.style.display = 'none';
            }
        }
    }, 3000); // Wait 3 seconds for auto-start
});

function showError(error) {
    const loadingContainer = document.querySelector('.loading-container');
    if (loadingContainer) {
        loadingContainer.innerHTML = `
            <div class="error-message">
                <h3>Application Error</h3>
                <p>Failed to initialize the application.</p>
                <p class="error-details">Error: ${error.message || error}</p>
                <p class="error-details">Type: ${error.name || 'Unknown'}</p>
                <p>Please check your device and try again.</p>
                <button onclick="location.reload()">Retry</button>
            </div>
        `;
    }
}
