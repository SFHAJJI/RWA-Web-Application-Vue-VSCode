// Modern Interactions JavaScript
document.addEventListener('DOMContentLoaded', function() {
    console.log('Modern interactions loaded');

    // Initialize page transition overlay
    createPageTransitionOverlay();

    // Add ripple effect to buttons
    addRippleEffect();

    // Add smooth page transitions
    addPageTransitions();

    // Initialize fade-in animations for cards
    initializeFadeInAnimations();
});

function createPageTransitionOverlay() {
    if (!document.getElementById('pageTransition')) {
        const overlay = document.createElement('div');
        overlay.id = 'pageTransition';
        overlay.className = 'page-transition';
        overlay.innerHTML = '<div class="spinner"></div>';
        document.body.appendChild(overlay);
    }
}

function addRippleEffect() {
    const buttons = document.querySelectorAll('.module-card, .btn, button');
    
    buttons.forEach(button => {
        button.addEventListener('click', function(e) {
            // Skip ripple for disabled buttons
            if (this.classList.contains('helios-card')) return;

            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.cssText = `
                position: absolute;
                width: ${size}px;
                height: ${size}px;
                left: ${x}px;
                top: ${y}px;
                background: rgba(255, 255, 255, 0.3);
                border-radius: 50%;
                transform: scale(0);
                animation: ripple 0.6s ease-out;
                pointer-events: none;
                z-index: 1;
            `;
            
            this.style.position = 'relative';
            this.style.overflow = 'hidden';
            this.appendChild(ripple);
            
            setTimeout(() => {
                if (ripple.parentNode) {
                    ripple.parentNode.removeChild(ripple);
                }
            }, 600);
        });
    });
    
    // Add ripple animation
    if (!document.getElementById('rippleAnimation')) {
        const style = document.createElement('style');
        style.id = 'rippleAnimation';
        style.textContent = `
            @keyframes ripple {
                to {
                    transform: scale(2);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
}

function addPageTransitions() {
    const links = document.querySelectorAll('.module-card[onclick]');
    
    links.forEach(link => {
        const originalOnClick = link.getAttribute('onclick');
        
        link.removeAttribute('onclick');
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Skip transition for disabled cards
            if (this.classList.contains('helios-card')) return;
            
            // Show transition overlay
            const overlay = document.getElementById('pageTransition');
            if (overlay) {
                overlay.classList.add('active');
            }
            
            // Execute original navigation after short delay
            setTimeout(() => {
                if (originalOnClick) {
                    eval(originalOnClick);
                }
            }, 300);
        });
    });
}

function initializeFadeInAnimations() {
    // Add fade-in class to elements that should animate
    const cards = document.querySelectorAll('.module-card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        setTimeout(() => {
            card.style.opacity = '1';
            card.classList.add('fade-in');
        }, 100 * (index + 1));
    });
}

// Enhanced hover effects for better user experience
function enhanceHoverEffects() {
    const cards = document.querySelectorAll('.module-card');
    
    cards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            if (!this.classList.contains('helios-card')) {
                this.style.transform = 'translateY(-5px) scale(1.02)';
            }
        });
        
        card.addEventListener('mouseleave', function() {
            if (!this.classList.contains('helios-card')) {
                this.style.transform = '';
            }
        });
    });
}

// Initialize enhanced effects
document.addEventListener('DOMContentLoaded', enhanceHoverEffects);