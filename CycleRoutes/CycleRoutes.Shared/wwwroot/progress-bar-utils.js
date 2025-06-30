// Progress bar click handling functions
window.calculateClickPercentage = (event, progressBarClass) => {
    try {
        // Find the progress bar element
        const progressBar = document.querySelector(`.${progressBarClass}`);
        if (!progressBar) {
            console.warn(`Progress bar with class '${progressBarClass}' not found`);
            return -1;
        }

        // Get the bounding rectangle of the progress bar
        const rect = progressBar.getBoundingClientRect();
        
        // Calculate the click position relative to the progress bar
        const clickX = event.clientX - rect.left;
        const progressBarWidth = rect.width;
        
        // Calculate the percentage (0 to 1)
        let percentage = clickX / progressBarWidth;
        
        // Ensure the percentage is within bounds
        percentage = Math.max(0, Math.min(1, percentage));
        
        return percentage;
    } catch (error) {
        console.error('Error calculating click percentage:', error);
        return -1;
    }
};
