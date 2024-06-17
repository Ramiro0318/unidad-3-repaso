using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using unidad_3_repaso.models;

namespace unidad_3_repaso.viewmodels
{
    public class RecetarioViewModel : INotifyPropertyChanged
    {

        public Receta Receta { get; set; } = new();
        public ObservableCollection<Receta> OCRecetas { get; set; } = new();
        public string Error { get; set; } = "";

        public ICommand AgregarCommand { get; set; }
        public ICommand IrAgregarCommand { get; set; }
        public ICommand IrEditarCommad { get; set; }
        public ICommand EditarCommad { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }

        public RecetarioViewModel()
        {
            AgregarCommand = new Command(Agregar);
            IrAgregarCommand = new Command(IrAgregar);
            IrEditarCommad = new Command<Receta>(IrEditar);
            EditarCommad = new Command(Editar);
            EliminarCommand = new Command(Eliminar);
            CancelarCommand = new Command(Cancelar);
            Cargar();
        }

        private void Cancelar()
        {
            Receta = new();
            Actualizar(nameof(Receta));
            Error = "";
            Actualizar(nameof(Error));
            OCRecetas.Clear();
            Cargar();
            Shell.Current.GoToAsync("//MainView");
        }

        private void Editar()
        {
            Error = "";
            if (string.IsNullOrWhiteSpace(Receta.Nombre))
            {
                Error = "Indique el nombre de la receta";
            }
            if (string.IsNullOrWhiteSpace(Receta.Nombre))
            {
                Error = "Coloque una descripción a la receta";
            }
            if (Receta.Tiempo <= 0)
            {
                Error = "Indique correctamente el tiempo de preparación";
            }
            if (Error == "")
            {
                OCRecetas.Add(Receta);
                Guardar();
                Actualizar(nameof(Receta));
            }
            Actualizar(nameof(Error));
        }

        private void IrEditar(Receta? receta)
        {
            receta = Receta;
            if (receta.Nombre != null)
            {
                var recetaclon = new Receta
                {
                    Nombre = receta.Nombre,
                    Descripcion = receta.Descripcion,
                    Tiempo = receta.Tiempo,
                };
                Receta = recetaclon;
                Actualizar(nameof(Receta));
                Shell.Current.GoToAsync("//EditarView");
            }

        }

        private void Eliminar()
        {
            OCRecetas.Remove(Receta);
            Actualizar(nameof(Receta));
            Receta = new();
            Guardar();
        }

        private void IrAgregar()
        {
            Receta = new();
            Actualizar(nameof(Receta));
            Shell.Current.GoToAsync("//AgregarView");
        }

        private void Agregar()
        {
            if (string.IsNullOrWhiteSpace(Receta.Nombre))
            {
                Error = "Indique el nombre de la receta";
            }
            if (string.IsNullOrWhiteSpace(Receta.Nombre))
            {
                Error += "Coloque una descripción a la receta";
            }
            if (Receta.Tiempo <= 0)
            {
                Error += "Indique correctamente el tiempo de preparación";
            }
            if (Error == "")
            {
                OCRecetas.Add(Receta);
                Guardar();
                Cancelar();
                Actualizar(nameof(Receta));
            }
            Actualizar(nameof(Error));
        }
        private void Actualizar(string? name)
        {
            PropertyChanged?.Invoke(this, new(name));
        }

        string r = "/receta.json";
        private void Guardar()
        {
            var ruta = FileSystem.AppDataDirectory;
            File.WriteAllText(ruta + r, JsonSerializer.Serialize(OCRecetas));
        }

        private void Cargar()
        {
            var ruta = FileSystem.AppDataDirectory;
            if (File.Exists(ruta + r))
            {
                var datos = JsonSerializer.Deserialize<ObservableCollection<Receta>>(File.ReadAllText(ruta + r));
                if (datos != null)
                {
                    foreach (var d in datos)
                    {
                        OCRecetas.Add(d);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
