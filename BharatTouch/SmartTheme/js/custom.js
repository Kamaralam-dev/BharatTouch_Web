$(document).ready(function(){

  $("button.navbar-toggle").click(function(){
    $("html").toggleClass("nav-open");
    $("body").toggleClass("overflow-hidden");
  });


    $('.logo-slider').slick({
        dots: false,
        infinite: true,
      arrows: false,
      speed: 300,
      autoplay: true,
      autoplaySpeed: 1500,
      slidesToShow: 5,
      slidesToScroll: 1,
      responsive: [
        {
          breakpoint: 1024,
          settings: {
            slidesToShow: 3,
            slidesToScroll: 3,
            infinite: true,
            dots: false
          }
        },
        {
          breakpoint: 600,
          settings: {
            slidesToShow: 2,
            slidesToScroll: 2
          }
        },
        {
          breakpoint: 480,
          settings: {
            slidesToShow: 1,
            slidesToScroll: 1
          }
        }
      ]
    });



    jQuery('.testimonial-slider').slick({ 
      slidesToShow: 1.9,
      slidesToScroll: 1,
      infinite: false,
      dots: true,
      arrows: false,
      responsive: [
        {
          breakpoint: 1024,
          settings: {
            slidesToShow: 1.5
          }
        },
        {
          breakpoint: 768,
          settings: {
            slidesToShow: 1.5
          }
        },
        {
          breakpoint: 480,
          settings: {
            slidesToShow: 1.1
          }
        },
      ]
    }); 

    

   //TABS
  
      $(".referral-tabs .tab").click(function () {
        var selectedTab = $(this).data("tab");

        $(".referral-tabs .tab").removeClass("active");
        $(this).addClass("active");

        $(".tab-content").removeClass("active");
        $("#tab-" + selectedTab).addClass("active");
      });


    // Accordion toggle
      $('.accordion-header').click(function () {
        var $accordion = $(this).next('.accordion-body');
        var isOpen = $(this).hasClass('active');

        $('.accordion-header').removeClass('active');
        $('.accordion-body').slideUp();

        if (!isOpen) {
          $(this).addClass('active');
          $accordion.slideDown();
        }
      });

      // Tab switching
      $(".faq-tabs .tab").click(function () {
        var selected = $(this).data("tab");

        $(".faq-tabs .tab").removeClass("active");
        $(this).addClass("active");

        $(".accordion-group").hide();
        $("#tab-" + selected).show();

        // Reset accordion to show first one
        $("#tab-" + selected + " .accordion-body").hide();
        $("#tab-" + selected + " .accordion-header").removeClass("active").first().addClass("active");
        $("#tab-" + selected + " .accordion-body").first().show();

        // Hide extra accordions and show the button again
        $(".extra-accordion").hide();
        $("#toggle-all").show();
      });

      // View All button - RUN ONCE ONLY, outside tab click

      let showingAll = false;
      $("#toggle-all").click(function () {
        if (!showingAll) {
          // Show all extra accordions
          $(".extra-accordion").slideDown();
            $(this).html('View Less <img src="SmartTheme/images/double-arrow.png" alt="Arrow">');
          showingAll = true;
        } else {
          // Hide them again
          $(".extra-accordion").slideUp();
            $(this).html('View All <img src="SmartTheme/images/double-arrow.png" alt="Arrow">');
          showingAll = false;
        }
      });

      showingAll = false;
    $("#toggle-all").html('View All <img src="SmartTheme/images/double-arrow.png" alt="Arrow">').show();


      $('a[href^="#"]').on('click', function (e) {
        e.preventDefault();
      
        var target = $($(this).attr('href'));
      
        if (target.length) {
          $('html, body').animate({
            scrollTop: target.offset().top - 80 // adjust offset as needed
          }, 600);
        }
          $("html").removeClass("nav-open");
          $("body").removeClass("overflow-hidden");
      });
      

      $(window).on('scroll', function () {
        if ($(this).scrollTop() > 100) { // Adjust the scroll offset as needed
          $('header').addClass('sticky-header');
        } else {
          $('header').removeClass('sticky-header');
        }
      });
      

  
  })


  const steps = Array.from(document.querySelectorAll(".form-step"));
  const indicators = Array.from(document.querySelectorAll(".step-point"));
  let currentStep = 0;

  function showStep(index) {
    // Update form step visibility
    steps.forEach((step, i) => {
      step.classList.toggle("active-step", i === index);
    });

    // Update step-point classes
    indicators.forEach((el, i) => {
      el.classList.remove("active", "completed");
      if (i < index) {
        el.classList.add("completed");
      } else if (i === index) {
        el.classList.add("active");
      }
    });

    currentStep = index;
  }

  //document.querySelectorAll(".next").forEach(btn => {
  //  btn.addEventListener("click", e => {
  //    e.preventDefault();
  //    if (currentStep < steps.length - 1) {
  //      showStep(currentStep + 1);
  //    }
  //  });
  //});

  document.querySelectorAll(".back").forEach(btn => {
    btn.addEventListener("click", e => {
      e.preventDefault();
      if (currentStep > 0) {
        showStep(currentStep - 1);
      }
    });
  });

  // Initial step
  showStep(0);