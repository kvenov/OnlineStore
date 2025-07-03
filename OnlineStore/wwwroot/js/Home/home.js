const promoSwiper = new Swiper(".promoSwiper", {
    loop: true,
    spaceBetween: 30,
    autoplay: {
        delay: 4000,
        disableOnInteraction: false,
    },
    navigation: {
        nextEl: ".swiper-button-next.promo-btn",
        prevEl: ".swiper-button-prev.promo-btn",
    },
    breakpoints: {
        320: {
            slidesPerView: 1.1,
        },
        768: {
            slidesPerView: 2,
        },
        992: {
            slidesPerView: 3,
        }
    },
});


AOS.init();

const swiper = new Swiper(".heroSwiper", {
    autoplay: {
        delay: 5000,
        disableOnInteraction: false
    },
    loop: true,
    effect: "fade",
    speed: 1000
});