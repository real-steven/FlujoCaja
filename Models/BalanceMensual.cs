using System.Globalization;

namespace FlujoCajaWpf.Models
{
    public class BalanceMensual
    {
        public string Mes { get; set; } = string.Empty;
        public decimal IngresosValor { get; set; }
        public decimal EgresosValor { get; set; }
        public decimal BalanceMesValor { get; set; }
        public decimal BalanceAcumuladoValor { get; set; }
        
        // Propiedades formateadas para mostrar en el DataGrid
        public string Ingresos => IngresosValor.ToString("C", new CultureInfo("es-CR"));
        public string Egresos => EgresosValor.ToString("C", new CultureInfo("es-CR"));
        public string BalanceMes => BalanceMesValor.ToString("C", new CultureInfo("es-CR"));
        public string BalanceAcumulado => BalanceAcumuladoValor.ToString("C", new CultureInfo("es-CR"));
        
        // Estado visual
        public string Estado
        {
            get
            {
                if (BalanceAcumuladoValor > 500) return "🟢";
                if (BalanceAcumuladoValor > 0) return "🟡";
                return "🔴";
            }
        }
    }
}
