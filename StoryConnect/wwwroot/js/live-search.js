//document.addEventListener("DOMContentLoaded", function () {
//    // Sidebar toggle

document.addEventListener("DOMContentLoaded", function () {

    document.getElementById('toggleSidebar').addEventListener('click', function () {
        document.getElementById('sidebar').classList.toggle('collapsed');
    });
    let searchBox = document.getElementById("searchBox");
    let searchResults = document.getElementById("searchResults");

    function adjustDropdownWidth() {
        if (searchBox) {
            searchResults.style.width = `${searchBox.offsetWidth}px`; // Ajusta el ancho al del input
        }
    }
    adjustDropdownWidth();
    window.addEventListener("resize", adjustDropdownWidth);

    searchBox.addEventListener("input", function () {
        let query = searchBox.value.trim();

        if (query.length < 2) {
            searchResults.style.display = "none";
            return;
        }

        fetch(`/Libros/BuscarLibros?query=${encodeURIComponent(query)}`)
            .then(response => response.json())
            .then(data => {
                searchResults.innerHTML = ""; // Limpia resultados anteriores

                if (data.results.length === 0) {
                    searchResults.innerHTML = "<div class='dropdown-item'>No se encontraron resultados</div>";
                } else {
                    data.results.forEach(libro => {
                        let item = document.createElement("a");
                        item.href = `/Libros/Detalles/${libro.id}`;
                        item.classList.add("dropdown-item");
                        item.innerHTML = `<strong>${libro.titulo}</strong> - ${libro.autor}`;
                        searchResults.appendChild(item);
                    });
                }

                searchResults.style.display = "block";
            })
            .catch(error => console.error("Error en la búsqueda:", error));
    });

    // Ocultar los resultados al hacer clic fuera
    document.addEventListener("click", function (event) {
        if (!searchBox.contains(event.target) && !searchResults.contains(event.target)) {
            searchResults.style.display = "none";
        }
    });
});
