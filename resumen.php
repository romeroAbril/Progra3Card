<?php
        session_start();
        include("conexion.php");

        if (!isset($_SESSION["usuario"])) {
            header("Location: ingreso.html");
            exit();
        }

        $documento = $_SESSION["documento"];

        // Última liquidación
        $sqlUltima = "
        SELECT t.numero_tarjeta,
                l.periodo,
                l.fecha_vencimiento,
                l.total_a_pagar,
                l.pago_minimo
                FROM tarjetas t
                INNER JOIN liquidaciones l ON t.num_cuenta = l.num_cuenta
                WHERE t.dni_titular = '$documento'
                ORDER BY l.periodo DESC
                LIMIT 1";

        $resultadoUltima = $conn->query($sqlUltima);
        $ultima = $resultadoUltima->fetch_assoc();

        // Historial
        $sqlHistorial = "
            SELECT periodo,
            fecha_vencimiento,
            total_a_pagar,
            pago_minimo
            FROM tarjetas t
            INNER JOIN liquidaciones l ON t.num_cuenta = l.num_cuenta
            WHERE t.dni_titular = '$documento'
            ORDER BY periodo DESC
            LIMIT 100 OFFSET 1";

        $historial = $conn->query($sqlHistorial);
?>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Resumen de Cuenta</title>
</head>
<body>

            <h1>Bienvenido <?php echo $_SESSION["nombre"]; ?></h1>

            <a href="logout_usuario.php">Cerrar sesión</a>

            <hr>

                <?php if ($ultima) { ?>

                    <h2>Última Liquidación</h2>

                    <p><strong>Número de tarjeta:</strong> <?php echo $ultima["numero_tarjeta"]; ?></p>

                    <p><strong>Período:</strong> <?php echo $ultima["periodo"]; ?></p>

                    <p><strong>Fecha de vencimiento:</strong> <?php echo $ultima["fecha_vencimiento"]; ?></p>

                    <p><strong>Total a pagar:</strong> $<?php echo $ultima["total_a_pagar"]; ?></p>

                    <p><strong>Pago mínimo:</strong> $<?php echo $ultima["pago_minimo"]; ?></p>

                 <?php } else { ?>

                 <p>No existen liquidaciones para esta tarjeta.</p>

                <?php } ?>

            <hr>

                <h2>Historial de Liquidaciones</h2>

                <table border="1" cellpadding="8">

                    <tr>
                        <th>Período</th>
                        <th>Vencimiento</th>
                        <th>Total</th>
                        <th>Pago mínimo</th>
                    </tr>

                    <?php while ($fila = $historial->fetch_assoc()) { ?>

                         <tr>
                            <td><?php echo $fila["periodo"]; ?></td>
                            <td><?php echo $fila["fecha_vencimiento"]; ?></td>
                            <td>$<?php echo $fila["total_a_pagar"]; ?></td>
                            <td>$<?php echo $fila["pago_minimo"]; ?></td>
                        </tr>

                    <?php } ?>

                </table>

    </body>
</html>