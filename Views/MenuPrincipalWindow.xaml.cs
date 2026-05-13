using System.Windows;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.ViewModels;
using FlujoCajaWpf.Services;

namespace FlujoCajaWpf.Views
{
    public partial class MenuPrincipalWindow : Window
    {
        private readonly Usuario _usuario;

        public MenuPrincipalWindow(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            DataContext = new MenuPrincipalViewModel(this, usuario);
            ActualizarIconoTema();
            Loaded += async (s, e) => await CargarAvatarMenuAsync();
        }

        private async Task CargarAvatarMenuAsync()
        {
            try
            {
                var perfil = await SupabaseAuthHelper.ObtenerPerfilCompletoAsync(_usuario.Id);
                if (perfil == null) return;

                // Mostrar iniciales
                var iniciales = "";
                if (!string.IsNullOrWhiteSpace(perfil.Nombre))   iniciales += perfil.Nombre[0];
                if (!string.IsNullOrWhiteSpace(perfil.Apellido)) iniciales += perfil.Apellido[0];
                if (iniciales.Length > 0)
                {
                    txtInicialesMenu.Text     = iniciales.ToUpper();
                    txtInicialesMenu.FontSize = 13;
                }

                // Cargar foto si existe
                if (!string.IsNullOrWhiteSpace(perfil.FotoPerfil))
                {
                    using var http  = new HttpClient();
                    var bytes = await http.GetByteArrayAsync(perfil.FotoPerfil);
                    var ms    = new MemoryStream(bytes);
                    var bmp   = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource  = ms;
                    bmp.CacheOption   = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    imgPerfilMenu.ImageSource  = bmp;
                    elipseImgMenu.Visibility   = System.Windows.Visibility.Visible;
                    txtInicialesMenu.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            catch { /* si falla, queda el icono por defecto */ }
        }

        private async void AbrirGestion_Click(object sender, RoutedEventArgs e)
        {
            var gestionWindow = new GestionWindow();
            gestionWindow.Owner = this;
            this.Hide(); // Ocultar MenuPrincipal
            gestionWindow.ShowDialog();
            this.Show(); // Mostrar MenuPrincipal al cerrar Gestión
            
            // Recargar casas después de cerrar la ventana de gestión
            if (DataContext is MenuPrincipalViewModel viewModel)
            {
                await viewModel.CargarCasasAsync();
            }
        }

        private void AbrirResumen_Click(object sender, RoutedEventArgs e)
        {
            var resumenWindow = new ResumenConsolidadoWindow();
            resumenWindow.Owner = this;
            resumenWindow.ShowDialog();
        }

        private void BtnOrdenar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MenuPrincipalViewModel viewModel)
            {
                viewModel.CambiarOrdenamiento();
            }
        }

        private void CambiarTema_Click(object sender, RoutedEventArgs e)
        {
            ThemeService.ModoOscuro = !ThemeService.ModoOscuro;
            ActualizarIconoTema();
        }

        private void AbrirAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MenuPrincipalViewModel vm)
            {
                var adminWindow = new AdminUsuariosWindow(vm.NombreUsuario.Replace("👤 ", ""))
                {
                    Owner = this
                };
                adminWindow.ShowDialog();
            }
        }

        private void ActualizarIconoTema()
        {
            // &#xE706; = sol (Segoe MDL2), &#xE708; = luna
            txtIconoTema.Text = ThemeService.ModoOscuro ? "\uE706" : "\uE708";
            btnTema.ToolTip = ThemeService.ModoOscuro ? "Cambiar a modo claro" : "Cambiar a modo oscuro";
        }

        private async void AbrirPerfil_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PerfilUsuarioDialog(_usuario) { Owner = this };
            dlg.ShowDialog();
            // Refrescar avatar por si cambió la foto
            await CargarAvatarMenuAsync();
        }
    }
}
