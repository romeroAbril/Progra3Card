<?php

$server = "localhost";
$usuario = "root";
$password = "root";
$baseDatos = "mi_banco_db";

$conn = new mysqli($server, $usuario, $password, $baseDatos);

if ($conn->connect_error) {
    die("Error de conexión: " . $conn->connect_error);
}

?>