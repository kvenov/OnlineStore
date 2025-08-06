document.addEventListener("DOMContentLoaded", function () {
    const progressBars = document.querySelectorAll('.progress-fill');

    progressBars.forEach(bar => {
        const start = new Date(bar.dataset.start);
        const end = new Date(bar.dataset.end);
        const completed = bar.dataset.completed === 'true';
        const now = new Date();

        let percentage = 0;

        if (completed) {
            percentage = 100;
        } else if (now >= end) {
            percentage = 95;
        } else if (now <= start) {
            percentage = 5;
        } else {
            const totalTime = end - start;
            const elapsed = now - start;
            percentage = Math.min(100, Math.max(0, (elapsed / totalTime) * 100));
        }

        bar.style.width = percentage + '%';
    });
});