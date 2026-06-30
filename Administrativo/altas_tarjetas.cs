using System;
using MySql.Data.MySqlClient; 

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=root;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Funciones a completar:


         static void MenuEmitirTarjeta()
        {
             Console.Clear();
             Console.WriteLine("--- EMITIR NUEVA TARJETA ---");

            Console.Write("Documento: ");
            string documento = Console.ReadLine();

            Console.Write("Tipo de documento (DNI/PASAPORTE): ");
            string tipoDoc = Console.ReadLine().ToUpper();

            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Apellido: ");
            string apellido = Console.ReadLine();

            Console.Write("Fecha de nacimiento (AAAA-MM-DD): ");
            string fechaNacimiento = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Número de tarjeta: ");
            string numeroTarjeta = Console.ReadLine();

            Console.WriteLine("\nSeleccione el Banco Emisor:");
            Console.WriteLine("1. Banco Nación");
            Console.WriteLine("2. Banco Provincia");
            Console.WriteLine("3. Banco Galicia");
            Console.WriteLine("4. Banco Santander");
            Console.WriteLine("5. Banco BBVA");
            Console.WriteLine("6. Banco Macro");
            Console.Write("Opción: ");

            string banco = "";

            switch (Console.ReadLine())
            {
                case "1":
                    banco = "Banco Nación";
                 break;
                case "2":
                     banco = "Banco Provincia";
                break;
                case "3":
                    banco = "Banco Galicia";
                break;
                case "4":
                     banco = "Banco Santander";
                break;
                case "5":
                     banco = "Banco BBVA";
                break;
                case "6":
                     banco = "Banco Macro";
                 break;
                 default:
                     Console.WriteLine("Opción inválida.");
                     Console.ReadKey();
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                 conn.Open();

                MySqlTransaction trans = conn.BeginTransaction();

                 try
                {
                     // INSERT en usuarios
                     string sqlUsuario = @"INSERT INTO usuarios
                     (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password)
                     VALUES
                     (@documento, @tipoDoc, @nombre, @apellido, @fechaNacimiento, @email, NULL, NULL)";

                     MySqlCommand cmdUsuario = new MySqlCommand(sqlUsuario, conn, trans);

                    cmdUsuario.Parameters.AddWithValue("@documento", documento);
                    cmdUsuario.Parameters.AddWithValue("@tipoDoc", tipoDoc);
                    cmdUsuario.Parameters.AddWithValue("@nombre", nombre);
                    cmdUsuario.Parameters.AddWithValue("@apellido", apellido);
                    cmdUsuario.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                    cmdUsuario.Parameters.AddWithValue("@email", email);

                    cmdUsuario.ExecuteNonQuery();

                    // INSERT en tarjetas
                    string sqlTarjeta = @"INSERT INTO tarjetas
                    (numero_tarjeta, banco_emisor, estado, saldo, dni_titular)
                    VALUES
                    (@numeroTarjeta, @banco, 'Activa', 0, @documento)";

                    MySqlCommand cmdTarjeta = new MySqlCommand(sqlTarjeta, conn, trans);

                    cmdTarjeta.Parameters.AddWithValue("@numeroTarjeta", numeroTarjeta);
                    cmdTarjeta.Parameters.AddWithValue("@banco", banco);
                    cmdTarjeta.Parameters.AddWithValue("@documento", documento);

                    cmdTarjeta.ExecuteNonQuery();

                    // Obtener el número de cuenta generado automáticamente
                    string sqlCuenta = @"SELECT num_cuenta
                             FROM tarjetas
                             WHERE numero_tarjeta = @numeroTarjeta";

                            MySqlCommand cmdCuenta = new MySqlCommand(sqlCuenta, conn, trans);
                            cmdCuenta.Parameters.AddWithValue("@numeroTarjeta", numeroTarjeta);

                            int numCuenta = Convert.ToInt32(cmdCuenta.ExecuteScalar());

                            // Crear la primera liquidación en cero
                            string sqlLiquidacion = @"
                            INSERT INTO liquidaciones
                                (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo)
                                    VALUES
                                    (@cuenta, @periodo, @vencimiento, 0, 0)";

                                MySqlCommand cmdLiquidacion = new MySqlCommand(sqlLiquidacion, conn, trans);

                                cmdLiquidacion.Parameters.AddWithValue("@cuenta", numCuenta);
                                cmdLiquidacion.Parameters.AddWithValue("@periodo", DateTime.Now.ToString("yyyy-MM"));
                                cmdLiquidacion.Parameters.AddWithValue("@vencimiento", DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));

                         cmdLiquidacion.ExecuteNonQuery();

                         trans.Commit();

                         Console.ForegroundColor = ConsoleColor.Green;
                         Console.WriteLine("\nTarjeta emitida correctamente.");
                         Console.ResetColor();
                }
                 catch (Exception ex)
                {
                    trans.Rollback();

                     Console.ForegroundColor = ConsoleColor.Red;
                     Console.WriteLine("\nError al emitir la tarjeta.");
                     Console.WriteLine(ex.Message);
                     Console.ResetColor();
                }
            }

             Console.WriteLine("\nPresione una tecla para volver al menú...");
             Console.ReadKey();
}

        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            // === A realizar ===
            // Aquí deben implementar un SELECT sobre la tabla 'tarjetas'
            // para recorrer las filas e imprimirlas en la consola.
            
            ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            // === A realizar ===
            // Aquí deben realizar un SELECT con un JOIN entre 'tarjetas' y 'usuarios' 
            // filtrando por el numCuenta para traer todos los campos (Nombre, Apellido, Email, Saldo, etc.)
            
            MostrarDetalleCompleto(numCuenta);

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n⚠️ ADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");
            
            if (Console.ReadLine().ToUpper() == "S")
            {
                // === A realizar ===
                // Aquí deben ejecutar un DELETE sobre la tabla 'tarjetas' donde num_cuenta = numCuenta.
                // Como definimos ON DELETE CASCADE en la base de datos, las liquidaciones se borrarán solas.
                // Opcional: Evaluar si también eliminan al usuario de la tabla 'usuarios' o si lo mantienen.
                
                bool exito = DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }


        // =========================================================================
        // MÉTODOS BASE QUE DEBEN COMPLETAR CON LA LÓGICA 
        // =========================================================================

        static void ObtenerYMostrarTarjetas()
        {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                     conn.Open();

                        string sql = @"SELECT num_cuenta,
                                     numero_tarjeta,
                                     banco_emisor,
                                     dni_titular
                                     FROM tarjetas";

                     MySqlCommand cmd = new MySqlCommand(sql, conn);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                         Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}",
                                 reader["num_cuenta"],
                                 reader["numero_tarjeta"],
                                 reader["banco_emisor"],
                                 reader["dni_titular"]);
                    }

                     reader.Close();
                 }
        }
                static void MostrarDetalleCompleto(int cuenta)
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                            string sql = @"
                             SELECT u.nombre,
                             u.apellido,
                             u.documento,
                             u.email,
                             t.numero_tarjeta,
                             t.banco_emisor,
                             t.estado,
                             t.saldo
                            FROM usuarios u
                            INNER JOIN tarjetas t
                            ON u.documento = t.dni_titular
                            WHERE t.num_cuenta = @cuenta";

                            MySqlCommand cmd = new MySqlCommand(sql, conn);

                            cmd.Parameters.AddWithValue("@cuenta", cuenta);

                             MySqlDataReader reader = cmd.ExecuteReader();

                             if (reader.Read())
                                {
                                    Console.WriteLine("\n===== DATOS DEL CLIENTE =====");

                                    Console.WriteLine("Nombre: " + reader["nombre"]);
                                    Console.WriteLine("Apellido: " + reader["apellido"]);
                                    Console.WriteLine("Documento: " + reader["documento"]);
                                    Console.WriteLine("Email: " + reader["email"]);

                                    Console.WriteLine();

                                    Console.WriteLine("===== DATOS DE LA TARJETA =====");

                                    Console.WriteLine("Número: " + reader["numero_tarjeta"]);
                                    Console.WriteLine("Banco: " + reader["banco_emisor"]);
                                    Console.WriteLine("Estado: " + reader["estado"]);
                                    Console.WriteLine("Saldo: $" + reader["saldo"]);
                                }
                            else
                                 {
                                     Console.WriteLine("No existe una tarjeta con ese número de cuenta.");
                                 }

                        reader.Close();
                    }
                 }

            static bool DarDeBajaTarjeta(int cuenta)
            {
                 using (MySqlConnection conn = new MySqlConnection(connectionString))
                 {
                    conn.Open();

                        // Buscar el DNI del titular
                        string sqlBuscar = "SELECT dni_titular FROM tarjetas WHERE num_cuenta = @cuenta";

                        MySqlCommand cmdBuscar = new MySqlCommand(sqlBuscar, conn);
                        cmdBuscar.Parameters.AddWithValue("@cuenta", cuenta);

                        object resultado = cmdBuscar.ExecuteScalar();

                        if (resultado == null)
                        {
                            return false; // No existe esa cuenta
                        }

                        string dni = resultado.ToString();

                         // Eliminar el usuario
                         string sqlEliminar = "DELETE FROM usuarios WHERE documento = @dni";

                        MySqlCommand cmdEliminar = new MySqlCommand(sqlEliminar, conn);
                        cmdEliminar.Parameters.AddWithValue("@dni", dni);

                        int filas = cmdEliminar.ExecuteNonQuery();

                        return filas > 0;
                 }
            }  



        static void MenuEmitirLiquidacion()
        {
                 Console.Clear();
                 Console.WriteLine("--- EMITIR NUEVA LIQUIDACIÓN ---");

                Console.Write("Número de Cuenta: ");
                int numCuenta = Convert.ToInt32(Console.ReadLine());

                Console.Write("Período (AAAA-MM): ");
                string periodo = Console.ReadLine();

             Console.Write("Fecha de Vencimiento (AAAA-MM-DD): ");
             string fechaVencimiento = Console.ReadLine();

            Console.Write("Total a Pagar: ");
            decimal total = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Pago Mínimo: ");
            decimal pagoMinimo = Convert.ToDecimal(Console.ReadLine());

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

             try
             {  
                 // Verificar que exista la cuenta
                 string sqlExiste = "SELECT COUNT(*) FROM tarjetas WHERE num_cuenta = @cuenta";

                 MySqlCommand cmdExiste = new MySqlCommand(sqlExiste, conn);
                 cmdExiste.Parameters.AddWithValue("@cuenta", numCuenta);

                int existe = Convert.ToInt32(cmdExiste.ExecuteScalar());

                if (existe == 0)
                {
                     Console.ForegroundColor = ConsoleColor.Red;
                     Console.WriteLine("\nNo existe una tarjeta con ese número de cuenta.");
                     Console.ResetColor();
                     Console.ReadKey();
                    return;
                }

                string sql = @"INSERT INTO liquidaciones
                                (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo)
                                 VALUES
                                (@cuenta, @periodo, @vencimiento, @total, @pagoMinimo)";

                 MySqlCommand cmd = new MySqlCommand(sql, conn);

                 cmd.Parameters.AddWithValue("@cuenta", numCuenta);
                 cmd.Parameters.AddWithValue("@periodo", periodo);
                 cmd.Parameters.AddWithValue("@vencimiento", fechaVencimiento);
                 cmd.Parameters.AddWithValue("@total", total);
                 cmd.Parameters.AddWithValue("@pagoMinimo", pagoMinimo);

                int filas = cmd.ExecuteNonQuery();

                if (filas > 0)
                {
                     Console.ForegroundColor = ConsoleColor.Green;
                     Console.WriteLine("\nLiquidación emitida correctamente.");
                }
                else
                 {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nNo se pudo emitir la liquidación.");
                 }
             }
             catch (Exception ex)
             {
                 Console.ForegroundColor = ConsoleColor.Red;
                 Console.WriteLine("\nError: " + ex.Message);
             }

                Console.ResetColor();
            }

         Console.WriteLine("\nPresione una tecla para volver al menú...");
         Console.ReadKey();
        }

    }
}