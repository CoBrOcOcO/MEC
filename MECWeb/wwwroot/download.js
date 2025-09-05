// wwwroot/js/download.js - JavaScript für File-Download

window.downloadFile = (filename, base64Content) => {
    try {
        console.log(`📥 Download gestartet: ${filename}`);

        // Base64 zu Blob konvertieren
        const binaryString = atob(base64Content);
        const bytes = new Uint8Array(binaryString.length);

        for (let i = 0; i < binaryString.length; i++) {
            bytes[i] = binaryString.charCodeAt(i);
        }

        const blob = new Blob([bytes]);

        // Download-Link erstellen
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;

        // Link automatisch klicken
        document.body.appendChild(link);
        link.click();

        // Cleanup
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);

        console.log(`✅ Download erfolgreich: ${filename}`);

    } catch (error) {
        console.error(`❌ Download-Fehler: ${error.message}`);
        alert(`Download fehlgeschlagen: ${error.message}`);
    }
};