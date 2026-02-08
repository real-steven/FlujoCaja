using FlujoCajaWpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views
{
    public partial class ReactivacionDialog : Window
    {
        private List<Propiedad> _todasPropiedades;
        public Propiedad? PropiedadSeleccionada { get; private set; }
        public string Motivo { get; private set; } = string.Empty;

        public ReactivacionDialog(List<Propiedad> propiedadesInactivas)
        {
            InitializeComponent();
            _todasPropiedades = propiedadesInactivas;
            PropiedadesListBox.ItemsSource = _todasPropiedades;
        }

        private void BusquedaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filtro = BusquedaTextBox.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(filtro))
            {
                PropiedadesListBox.ItemsSource = _todasPropiedades;
            }
            else
            {
                var filtradas = _todasPropiedades.Where(p =>
                    p.Nombre.ToLower().Contains(filtro) ||
                    (p.CategoriaNombre?.ToLower().Contains(filtro) ?? false) ||
                    p.DuenoNombre.ToLower().Contains(filtro)
                ).ToList();
                
                PropiedadesListBox.ItemsSource = filtradas;
            }
        }

        private void PropiedadesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfirmarButton.IsEnabled = PropiedadesListBox.SelectedItem != null;
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ConfirmarButton_Click(object sender, RoutedEventArgs e)
        {
            PropiedadSeleccionada = PropiedadesListBox.SelectedItem as Propiedad;
            Motivo = MotivoTextBox.Text ?? string.Empty;
            this.DialogResult = true;
            this.Close();
        }
    }
}
