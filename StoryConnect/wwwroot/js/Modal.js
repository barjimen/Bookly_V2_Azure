$(document).ready(function () {
    var idLibro = $(this).data("idlibro");

    console.log(idLibro);

    // Mover libro entre listas
    $(".mover-libro").click(function () {
        var idlibro = $(this).data("idlibro");
        var destino = $(this).data("destino");
        var origen = $(this).data("origen");

        $.ajax({
            url: '/Libros/MoverLibrosEntreListas',
            type: 'POST',
            data: { idlibro: idlibro, destino: destino, origen: origen },
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: '¡Libro movido!',
                    text: 'El libro ha sido trasladado exitosamente.',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(() => location.reload());
            },
            error: function (xhr, status, error) {
                Swal.fire({
                    icon: 'success',
                    title: '¡Libro movido!',
                    text: 'El libro ha sido trasladado exitosamente.',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(() => location.reload());
            }
        });
    });

    // Abrir modal de edición de reseña
    $(".editar-resena").click(function () {
        var id = $(this).data("idresena");
        var usuario = $(this).data("usuario");
        var calificacion = $(this).data("calificacion");
        var texto = $(this).data("texto");

        $("#idResena").val(id);
        $("#usuario").val(usuario);
        $("#calificacion").val(calificacion);
        $("#texto").val(texto);

        var modal = new bootstrap.Modal(document.getElementById('modalEditar'));
        modal.show();
    });

    // Actualizar reseña (script simplificado)
    $("#actualizarReseña").click(function () {
        // Obtener los valores
        const idReseña = $("#idResena").val();
        const calificacion = $("#calificacion").val();
        const texto = $("#texto").val();

        // Validación básica de campos
        if (!texto.trim()) {
            Swal.fire({
                icon: 'error',
                title: 'Campo vacío',
                text: 'El texto de la reseña no puede estar vacío.',
                confirmButtonColor: '#d33'
            });
            return;
        }

        if (calificacion < 1 || calificacion > 5) {
            Swal.fire({
                icon: 'error',
                title: 'Calificación inválida',
                text: 'La calificación debe estar entre 1 y 5.',
                confirmButtonColor: '#d33'
            });
            return;
        }

        // Todo está correcto, enviar el formulario
        $("#formReseña").submit();
    });

    
});
