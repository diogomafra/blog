using System;

namespace TesteBinding.Tela1.Modelos
{
    public class Usuario
    {
        public string Nome { get; set; }
        public DateTime CriadoEm { get; set; }
        public Grupo Grupo { get; set; }
        public bool Ativo { get; set; }
    }
}
