using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesteBinding.Tela1.Modelos;

namespace TesteBinding.Tela1
{
    public class Presenter
    {
        private readonly IView view;

        public Presenter(IView view)
        {
            this.view = view;
        }

        public void Carrega()
        {
            view.Grupos = BuscaGrupos();
            view.Usuarios = new List<Usuario>();
        }

        public void Pesquisa()
        {
            if (view.GrupoSelecionado != null)
            {
                var usuarios = BuscaUsuariosDoGrupo(view.GrupoSelecionado);
                view.Usuarios = usuarios;
            }
        }

        private IEnumerable<Grupo> BuscaGrupos()
        {
            var grupos = new List<Grupo>();
            for (int i = 1; i <= 10; i++)
            {
                grupos.Add(new Grupo { Id = i, Nome = "Grupo " + i });
            }

            return grupos;
        }

        private IEnumerable<Usuario> BuscaUsuariosDoGrupo(Grupo grupo)
        {
            var usuarios = new List<Usuario>();
            for (int i = 1; i <= 5; i++)
            {
                var usuario = new Usuario
                {
                    Nome = String.Format("Usuario {0} do grupo {1}", i, grupo.Nome),
                    Grupo = grupo,
                    Ativo = i%2 == 0,
                    CriadoEm = DateTime.Now.AddDays(-i)
                };
                usuarios.Add(usuario);
            }

            return usuarios;
        }
    }
}
