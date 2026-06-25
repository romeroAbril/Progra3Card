<?php
session_start();

if (!isset($_SESSION["usuario"])) {
    header("Location: ingreso.html");
    exit();
}
?>

<h1>Bienvenido <?php echo $_SESSION["nombre"]; ?></h1>
<a href="logout_usuario.php">Cerrar sesión</a>