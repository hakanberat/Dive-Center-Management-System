// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {

    const sidebar =
        document.getElementById("appSidebar");

    const menuButton =
        document.getElementById("mobileMenuButton");

    const overlay =
        document.getElementById("sidebarOverlay");

    if (!sidebar || !menuButton || !overlay) {
        return;
    }

    function openSidebar() {

        sidebar.classList.add("is-open");

        overlay.classList.add("is-visible");

        document.body.classList.add("sidebar-open");

        menuButton.setAttribute(
            "aria-expanded",
            "true"
        );
    }

    function closeSidebar() {

        sidebar.classList.remove("is-open");

        overlay.classList.remove("is-visible");

        document.body.classList.remove("sidebar-open");

        menuButton.setAttribute(
            "aria-expanded",
            "false"
        );
    }

    menuButton.addEventListener(
        "click",
        function () {

            if (sidebar.classList.contains("is-open")) {
                closeSidebar();
            }
            else {
                openSidebar();
            }

        }
    );

    overlay.addEventListener(
        "click",
        closeSidebar
    );

    document.addEventListener(
        "keydown",
        function (event) {

            if (event.key === "Escape") {
                closeSidebar();
            }

        }
    );

    sidebar
        .querySelectorAll(".sidebar-link")
        .forEach(function (link) {

            link.addEventListener(
                "click",
                function () {

                    if (window.innerWidth <= 991.98) {
                        closeSidebar();
                    }

                }
            );

        });

    window.addEventListener(
        "resize",
        function () {

            if (window.innerWidth > 991.98) {
                closeSidebar();
            }

        }
    );

});