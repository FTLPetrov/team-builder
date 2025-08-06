/**
 * Escapes HTML special characters to prevent XSS attacks
 * @param {string} text - The text to escape
 * @returns {string} - The escaped text
 */
export const escapeHtml = (text) => {
  if (typeof text !== 'string') {
    return text;
  }
  
  const htmlEntities = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#39;'
  };
  
  return text.replace(/[&<>"']/g, (char) => htmlEntities[char]);
};

/**
 * Safely renders text that might contain HTML characters
 * @param {string} text - The text to render safely
 * @returns {string} - Safe text for rendering
 */
export const safeText = (text) => {
  return escapeHtml(text || '');
};
