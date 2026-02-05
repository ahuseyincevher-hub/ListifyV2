class ListifyApp {
    constructor() {
        this.currentListeId = null;
        this.init();
    }

    init() {
        this.initEventListeners();
    }

    showToast(message, type = 'info') {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.innerHTML = `
            <i class="fas fa-${this.getToastIcon(type)}"></i>
            <span>${message}</span>
        `;
        
        document.body.appendChild(toast);
        
        setTimeout(() => {
            toast.style.animation = 'slideOut 0.3s ease';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    getToastIcon(type) {
        const icons = {
            success: 'check-circle',
            error: 'exclamation-circle',
            warning: 'exclamation-triangle',
            info: 'info-circle'
        };
        return icons[type] || icons.info;
    }

    async updateProductStatus(productId, listeId) {
        try {
            const response = await fetch(`/Alisveris/DurumGuncelle?id=${productId}&listeId=${listeId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            });

            if (response.ok) {
                const data = await response.json();
                if (data.success) {
                    this.updateStats(data.istatistik);
                    return true;
                }
            }
            return false;
        } catch (error) {
            console.error('Durum güncellenirken hata:', error);
            return false;
        }
    }

    updateStats(stats) {
        const progressBar = document.querySelector('.progress-bar');
        const statsText = document.querySelector('.stats-text');
        
        if (progressBar) {
            progressBar.style.width = `${stats.tamamlanmaOrani}%`;
        }
        
        if (statsText) {
            statsText.textContent = `${stats.alinanUrun}/${stats.toplamUrun} ürün alındı`;
        }
    }

    switchListe(listeId) {
        window.location.href = `/Alisveris?listeId=${listeId}`;
    }

    showLoading(button) {
        const originalText = button.innerHTML;
        button.innerHTML = '<span class="loading"></span> Yükleniyor...';
        button.disabled = true;
        return () => {
            button.innerHTML = originalText;
            button.disabled = false;
        };
    }
}

const app = new ListifyApp();
window.app = app;

document.addEventListener('DOMContentLoaded', () => {
    const produktCards = document.querySelectorAll('.product-card');
    produktCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-4px)';
        });
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });
});

function confirmAction(message) {
    return confirm(message);
}

window.formatCurrency = function(amount) {
    return new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY'
    }).format(amount || 0);
};
