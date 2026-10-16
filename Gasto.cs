using System;

namespace MisGastosLab
{
    public class Gasto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Categoria { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Id,-3} {Descripcion,-20} Q{Monto,8:N2}  [{Categoria}]";
        }
    }
}