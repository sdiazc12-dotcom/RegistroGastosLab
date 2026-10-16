using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace MisGastosLab
{
    public class GastoRepositorio
    {
        private const string CadenaConexion =
            @"Server=(localdb)\MSSQLLocalDB;" +
            @"Database=GastosDB;" +
            @"Trusted_Connection=True;" +
            @"TrustServerCertificate=True;";

        public void Insertar(Gasto g)
        {
            string sql = "INSERT INTO Gastos (Descripcion, Monto, Categoria) " +
                         "VALUES (@descripcion, @monto, @categoria)";

            using (SqlConnection conexion = new SqlConnection(CadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@descripcion", g.Descripcion);
                comando.Parameters.AddWithValue("@monto", g.Monto);
                comando.Parameters.AddWithValue("@categoria", g.Categoria);

                conexion.Open();
                int filas = comando.ExecuteNonQuery();
                Console.WriteLine(filas == 1 ? "Gasto agregado." : "No Se Inserto");
            }
        }

        public List<Gasto> ListarTodos()
        {
            List<Gasto> lista = new List<Gasto>();
            string sql = "SELECT Id, Descripcion, Monto, Categoria " +
                         "FROM Gastos ORDER BY Id";
            using (SqlConnection conexion = new SqlConnection(CadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new Gasto
                        {
                            Id = lector.GetInt32(0),
                            Descripcion = lector.GetString(1),
                            Monto = lector.GetDecimal(2),
                            Categoria = lector.GetString(3)
                        });
                    }
                }
            }
            return lista;
        }
    }
}