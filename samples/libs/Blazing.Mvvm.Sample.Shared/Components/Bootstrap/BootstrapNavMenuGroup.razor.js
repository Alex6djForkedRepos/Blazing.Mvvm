// Check if this group contains any links with the 'active' class
export function hasActiveLink(element) {
    if (!element) return false;
    
    // Find all anchor tags within this element
    const links = element.querySelectorAll('a');
    
    // Check if any link has the 'active' class
    for (const link of links) {
        if (link.classList.contains('active')) {
            return true;
        }
    }
    
    return false;
}
