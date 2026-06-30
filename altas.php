<?php

include("conexion.php");

if ($_SERVER["REQUEST_METHOD"] == "POST") {

    $documento = $_POST["documento"];
    $tipo_doc = $_POST["tipo_doc"];
    $nombre = $_POST["nombre"];
    $apellido = $_POST["apellido"];
    $fecha = $_POST["fecha_nacimiento"];
    $email = $_POST["email"];
    $usuario = $_POST["usuario"];
    $passwordA = $_POST["passwordA"];
    $passwordB = $_POST["passwordB"];

    if ($passwordA != $passwordB) {
        die("❌ Las contraseñas no coinciden.");
    }

    // Buscar el cliente creado desde C#
    $sql = "SELECT * FROM usuarios
            WHERE documento='$documento'
            AND tipo_doc='$tipo_doc'
            AND nombre='$nombre'
            AND apellido='$apellido'
            AND fecha_nacimiento='$fecha'
            AND email='$email'";

    $resultado = $conn->query($sql);

    if ($resultado->num_rows == 0) {
        die("❌ Los datos ingresados no corresponden a un cliente registrado.");
    }

    $datos = $resultado->fetch_assoc();

    // Verificar si ya activó la cuenta
    if ($datos["usuario"] != NULL) {
        die("❌ Esta cuenta ya fue activada.");
    }

    // Verificar que el usuario elegido no exista
    $existe = $conn->query("SELECT * FROM usuarios WHERE usuario='$usuario'");

    if ($existe->num_rows > 0) {
        die("❌ Ese nombre de usuario ya está en uso.");
    }

    // Activar la cuenta
    $update = "UPDATE usuarios
               SET usuario='$usuario',
                   password='$passwordA'
               WHERE documento='$documento'";


    if ($conn->query($update) === TRUE) {
        echo "✅ Cuenta activada correctamente. Serás redirigido...";

        header("refresh:2;url=ingreso.html");
        exit();
    } else {
                echo "❌ Error: " . $conn->error;
            }

} else {
    echo "❌ No llegó información por POST.";
}

$conn->close();

?>