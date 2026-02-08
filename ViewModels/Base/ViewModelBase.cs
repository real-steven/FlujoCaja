using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlujoCajaWpf.ViewModels.Base
{
    /// <summary>
    /// Clase base para todos los ViewModels
    /// Implementa INotifyPropertyChanged para data binding
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifica a la UI que una propiedad ha cambiado
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Actualiza el valor de una propiedad y notifica el cambio
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
