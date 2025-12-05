/**
 * Application configuration
 */
export const config = {
    // API base URL - change based on environment
    apiBaseUrl: typeof window !== 'undefined' && window.location.hostname === 'localhost' 
        ? 'http://localhost:5000/api'
        : 'http://localhost:5000/api',
    
    // Alternative: read from environment or meta tag
    // apiBaseUrl: document.querySelector('meta[name="api-url"]')?.content || 'http://localhost:5000/api'
};
