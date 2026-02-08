using FlujoCajaWpf.Views.Controls;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views
{
    public partial class GestionWindow : Window
    {
        public GestionWindow()
        {
            InitializeComponent();
            
            // Cargar pestaña de Dueños por defecto
            TabDuenos_Click(btnTabDuenos, new RoutedEventArgs());
        }

        private void TabDuenos_Click(object sender, RoutedEventArgs e)
        {
            ActivarTab(btnTabDuenos);
            ContentArea.Content = new GestionDuenosControl();
        }

        private void TabCasas_Click(object sender, RoutedEventArgs e)
        {
            ActivarTab(btnTabCasas);
            ContentArea.Content = new GestionCasasControl();
        }

        private void TabCategoriasPropiedades_Click(object sender, RoutedEventArgs e)
        {
            ActivarTab(btnTabCategoriasPropiedades);
            ContentArea.Content = new GestionCategoriasControl();
        }

        private void TabCategoriasMovimientos_Click(object sender, RoutedEventArgs e)
        {
            ActivarTab(btnTabCategoriasMovimientos);
            ContentArea.Content = new GestionCategoriasMovimientosControl();
        }

        private void ActivarTab(Button botonActivo)
        {
            // Desactivar todos los tabs
            btnTabDuenos.Tag = "Inactive";
            btnTabCasas.Tag = "Inactive";
            btnTabCategoriasPropiedades.Tag = "Inactive";
            btnTabCategoriasMovimientos.Tag = "Inactive";
            
            // Activar el tab seleccionado
            botonActivo.Tag = "Active";
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
