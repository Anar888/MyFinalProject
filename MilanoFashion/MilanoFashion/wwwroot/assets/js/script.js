

$(document).ready(function () {
    $(document).on("click", ".open-product-modal", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url)
            .then(response => response.text())
            .then(data => {

                $("#quick_view .prmodal").html(data);
                $("#quick_view").modal(true);
            });


    });

    $(document).on("click", ".add-to-cart", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url)
            .then(function (response) {

                return response.text();
            }).then(data =>
                $(".basket-model").html(data)

            );
    });

    $(document).on("click", ".remove-to-cart", function (e) {
        e.preventDefault();
        let url = $(this).attr("href");

        fetch(url)
            .then(function (response) {

                return response.text();
            }).then(data => {

                $(".basket-model").html(data);




            });
    });
    $(document).on("click", ".add-to-wishlist", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url)
            .then(function (response) {

                return response.text();
            }).then(data =>
                $(".wish").html(data)

            );
    });
    $(document).on("click", ".remove-to-wishlist", function (e) {
        e.preventDefault();
        let url = $(this).attr("href");

        fetch(url)
            .then(function (response) {

                return response.text();
            }).then(data => {

                $(".wish").html(data);




            });
    });
   


})

