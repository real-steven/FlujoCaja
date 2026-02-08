using FlujoCajaWpf.ViewModels;
using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Windows;

namespace FlujoCajaWpf.Views
{
    public partial class InactivasWindow : Window
    {
        public InactivasWindow()
        {
            InitializeComponent();
            DataContext = new InactivasViewModel(this);
        }

        private void VolveryMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
