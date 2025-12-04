/**
 * Application configuration
 */
export const config = {
    // API base URL - change based on environment
    apiBaseUrl: window.location.hostname === 'localhost' 
        ? 'http://localhost:5000/api'
        : 'https://your-production-domain.com/api',
    
    // Alternative: read from environment or meta tag
    // apiBaseUrl: document.querySelector('meta[name="api-url"]')?.content || 'http://localhost:5000/api'
};
