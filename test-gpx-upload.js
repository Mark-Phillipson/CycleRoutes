// Simple test script to upload the sample GPX file
async function testGpxUpload() {
    try {
        console.log('Starting GPX upload test...');
        
        // Fetch the sample GPX file
        const response = await fetch('/sample-london-loop.gpx');
        if (!response.ok) {
            throw new Error(`Failed to fetch GPX file: ${response.status}`);
        }
        
        const gpxContent = await response.text();
        console.log('GPX content loaded:', gpxContent.substring(0, 200) + '...');
        
        // Create a File object from the GPX content
        const file = new File([gpxContent], 'sample-london-loop.gpx', { type: 'application/gpx+xml' });
        
        // Find the file input element
        const fileInput = document.querySelector('input[type="file"][accept=".gpx"]');
        if (!fileInput) {
            throw new Error('File input not found');
        }
        
        // Create a FileList containing our file
        const dt = new DataTransfer();
        dt.items.add(file);
        fileInput.files = dt.files;
        
        // Trigger the change event
        const changeEvent = new Event('change', { bubbles: true });
        fileInput.dispatchEvent(changeEvent);
        
        console.log('GPX file uploaded successfully');
        
    } catch (error) {
        console.error('Error uploading GPX file:', error);
    }
}

// Auto-run the test after page load
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        setTimeout(testGpxUpload, 2000); // Wait 2 seconds for page to fully load
    });
} else {
    setTimeout(testGpxUpload, 2000);
}
