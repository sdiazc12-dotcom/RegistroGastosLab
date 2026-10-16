using System;
using System.IO;
using System.Collections.Generic;

namespace MisGastosLab
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ruta = "gastos.csv";
            string opcion;
            List<Gasto> inventario = Cargar(ruta);
            GastoRepositorio repo = new GastoRepositorio();
            int siguienteId = 1;

            foreach (Gasto g in inventario)
            {
                if (g.Id >= siguienteId)
                {
                    siguienteId = g.Id + 1;
                }
            }

            do
            {
                opcion = LeerOpcionMenu();

                switch (opcion)
                {
                    case "1":
                        Agregar(inventario, ref siguienteId);
                        break;
                    case "2":
                        Listar(inventario);
                        break;
                    case "3":
                        BuscarGasto(inventario);
                        break;
                    case "4":
                        ListarEnSqlServer(repo);
                        break;
                    case "5":
                        AgregarEnSqlServer(repo);
                        break;
                    case "6":
                        Guardar(inventario, ruta);
                        Console.WriteLine("Saliendo del sistema...");
                        break;
                    default:
                        Console.WriteLine("Opción no válida, intenta de nuevo.");
                        break;
                }
            }
            while (opcion != "6");
        }

        public static string LeerOpcionMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== MENU GASTOS ===");
            Console.WriteLine("1. Agregar gasto");
            Console.WriteLine("2. Listar gastos");
            Console.WriteLine("3. Buscar gasto");
            Console.WriteLine("4. Listar gastos de SQL Server");
            Console.WriteLine("5. Agregar gasto en SQL Server");
            Console.WriteLine("6. Salir");
            Console.Write("Elige una opcion: ");
            return Console.ReadLine();
        }

        private static void Agregar(List<Gasto> inv, ref int id)
        {
            Console.WriteLine();
            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine().Trim();

            Console.Write("Monto: ");
            bool montoOk = decimal.TryParse(Console.ReadLine(), out decimal monto);

            Console.Write("Categoría: ");
            string categoria = Console.ReadLine().Trim();

            if (!montoOk || monto <= 0 || string.IsNullOrWhiteSpace(descripcion) || string.IsNullOrWhiteSpace(categoria))
            {
                Console.WriteLine("Datos Invalidos");
                return;
            }
            else
            {
                inv.Add(new Gasto
                {
                    Id = id,
                    Descripcion = descripcion,
                    Monto = monto,
                    Categoria = categoria
                });
                id++;
                Console.WriteLine("Gasto agregado.");
            }
        }

        private static void Listar(List<Gasto> inv)
        {
            if (inv.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("No hay gastos registrados.");
                return;
            }

            decimal totalGeneral = 0;
            Console.WriteLine($"{"Id",-3} {"Descripción",-20} {"Monto",10}  {"Categoría",-15}");
            foreach (Gasto g in inv)
            {
                Console.WriteLine(g);
                totalGeneral += g.Monto;
            }

            Console.WriteLine($"TOTAL GASTADO: Q {totalGeneral:N2}");
        }

        private static void BuscarGasto(List<Gasto> inv)
        {
            Console.Write("Categoría a buscar: ");
            string t = Console.ReadLine().Trim().ToUpper();
            bool encontrado = false;

            foreach (Gasto g in inv)
            {
                if (g.Categoria.ToUpper().Contains(t) || g.Descripcion.ToUpper().Contains(t))
                {
                    Console.WriteLine(g);
                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("Sin coincidencias");
            }
        }

        private static void Guardar(List<Gasto> inv, string ruta)
        {
            var lineas = new List<string>();

            foreach (Gasto g in inv)
            {
                lineas.Add($"{g.Id};{g.Descripcion};{g.Monto};{g.Categoria}");
            }

            File.WriteAllLines(ruta, lineas);
            Console.WriteLine($"Gastos guardados en {ruta} ({inv.Count} registros)");
        }

        private static List<Gasto> Cargar(string ruta)
        {
            var inv = new List<Gasto>();
            if (!File.Exists(ruta)) return inv;

            foreach (string linea in File.ReadLines(ruta))
            {
                string[] campos = linea.Split(';');

                if (campos.Length != 4) continue;

                inv.Add(new Gasto
                {
                    Id = int.Parse(campos[0]),
                    Descripcion = campos[1],
                    Monto = decimal.Parse(campos[2]),
                    Categoria = campos[3]
                });
            }

            return inv;
        }

        private static void AgregarEnSqlServer(GastoRepositorio repo)
        {
            Console.WriteLine();
            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine().Trim();

            Console.Write("Monto: ");
            bool montoOk = decimal.TryParse(Console.ReadLine(), out decimal monto);

            Console.Write("Categoría: ");
            string categoria = Console.ReadLine().Trim();

            if (!montoOk || monto <= 0 || string.IsNullOrWhiteSpace(descripcion) || string.IsNullOrWhiteSpace(categoria))
            {
                Console.WriteLine("Datos Invalidos");
                return;
            }

            repo.Insertar(new Gasto { Descripcion = descripcion, Monto = monto, Categoria = categoria });
        }

        private static void ListarEnSqlServer(GastoRepositorio repo)
        {
            List<Gasto> lista = repo.ListarTodos();
            if (lista.Count == 0)
            {
                Console.WriteLine("No Hay Gastos en SQL Server");
                return;
            }

            decimal totalGeneral = 0;
            foreach (Gasto g in lista)
            {
                Console.WriteLine(g);
                totalGeneral += g.Monto;
            }
            Console.WriteLine($"TOTAL GASTADO EN SQL: Q {totalGeneral:N2}");
        }
    }
}