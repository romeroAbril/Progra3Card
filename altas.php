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
        die("❌ Las contraseñas no coinciden");
    }

    $verificar = $conn->query("
        SELECT * FROM usuarios 
        WHERE documento = '$documento' 
        OR email = '$email' 
        OR usuario = '$usuario'
    ");

    if (!$verificar) {
        die("❌ Error SELECT: " . $conn->error);
    }

    if ($verificar->num_rows > 0) {
        die("❌ Usuario ya existe");
    }

    $sql = "INSERT INTO usuarios 
    (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password)
    VALUES 
    ('$documento', '$tipo_doc', '$nombre', '$apellido', '$fecha', '$email', '$usuario', '$passwordA')";

    if ($conn->query($sql) === TRUE) {
        echo "✅ Usuario registrado correctamente";
    } else {
        echo "❌ Error INSERT: " . $conn->error;
    }

} else {
    echo "❌ No llegó POST";
}

?>