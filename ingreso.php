<?php
session_start();
include("conexion.php");

if (isset($_POST["ingresar"])) {

    $tipo_doc = $_POST["tipo_doc"];
    $documento = $_POST["documento"];
    $usuario = $_POST["usuario"];
    $password = $_POST["password"];

    $sql = "SELECT * FROM usuarios 
    WHERE documento = '$documento' 
    AND usuario = '$usuario' 
    AND password = '$password'
    AND tipo_doc = '$tipo_doc'";

    $resultado = $conn->query($sql);

    if ($resultado->num_rows > 0) {

        $fila = $resultado->fetch_assoc();

        $_SESSION["usuario"] = $fila["usuario"];
        $_SESSION["nombre"] = $fila["nombre"];
        $_SESSION["documento"] = $fila["documento"];

        header("Location: panel_usuario.php");
        exit();

    } else {
        echo "Usuario o contraseña incorrectos";
    }
}
?>