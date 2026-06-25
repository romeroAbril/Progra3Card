<?php
$server = "localhost";
$usuario = "root"; 
$password = "root";
$baseDatos = "mi_banco_db";


// Crea conexión (objeto)
$conexión = new mysqli($server, $usuario, $password);

// Verifica establecimientro de la conexión:
if ($conexión->connect_error) {
  die("Fallo al conectar: " . $conexión->connect_error);
}
else {
  echo "Conexión exitosa a MySQL <br>";
}
$conexión->close();
?>