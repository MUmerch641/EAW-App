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
    console.log('DOM fully loaded, waiting for Blazor...');
    
    const loadingContainer = document.querySelector('.loading-container');
    
    // Poll for Blazor availability
    let attempts = 0;
    const maxAttempts = 50; // 5 seconds (100ms interval)
    
    const checkInterval = setInterval(() => {
        attempts++;
        
        if (typeof Blazor !== 'undefined') {
            clearInterval(checkInterval);
            console.log('Blazor script loaded!');
            
            // Blazor is loaded. It should auto-start.
            // We set a safety timeout to check if it actually started
            setTimeout(() => {
                // Check if Blazor has started (runtime is initialized)
                const isStarted = window.Blazor && window.Blazor._internal && window.Blazor._internal.runtime;
                
                if (!isStarted) {
                    console.log('Blazor loaded but not started. Attempting manual start...');
                    try {
                        Blazor.start().then(() => {
                            console.log('Blazor started manually.');
                        }).catch(e => {
                            console.error('Manual start failed:', e);
                        });
                    } catch (e) {
                        console.error('Exception during manual start:', e);
                    }
                } else {
                    console.log('Blazor appears to have auto-started successfully.');
                }
            }, 1000);
            
        } else if (attempts >= maxAttempts) {
            clearInterval(checkInterval);
            console.error('Blazor failed to load after 5 seconds.');
            
            if (loadingContainer) {
                loadingContainer.innerHTML = `
                    <div class="error-message">
                        <h3>Application Error</h3>
                        <p>Blazor framework failed to load.</p>
                        <p>The application environment may be unstable.</p>
                        <button onclick="location.reload()">Retry</button>
                    </div>
                `;
            }
        }
    }, 100);
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
