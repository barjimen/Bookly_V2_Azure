$(document).ready(function () {
    $(".mover-libro").click(function () {
        var idlibro = $(this).data("idlibro");
        var destino = $(this).data("destino");
        var origen = $(this).data("origen");

        $.ajax({
            url: '/Libros/MoverLibrosEntreListas',
            type: 'POST',
            data: { idlibro: idlibro, destino: destino, origen: origen },
            success: function (response) {
                alert("item movido exitosamente.");
                location.reload(); // Recargar la página para actualizar la lista
            },
            error: function (xhr, status, error) {
                alert("Error al mover el item: " + xhr.responseText);
            }
        });
    });
});