using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TesteBinding.Tela1.Modelos;

namespace TesteBinding.Tela1
{
    public interface IView
    {
        IEnumerable<Grupo> Grupos { set; }
        IEnumerable<Usuario> Usuarios { set; }
        Grupo GrupoSelecionado { get; }
    }

    public partial class View : Form, IView
    {
        private Presenter presenter;

        public View()
        {
            InitializeComponent();
            dataGrid.AutoGenerateColumns = false;
            presenter = new Presenter(this);
            presenter.Carrega();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            presenter.Pesquisa();
        }

        #region IView Members

        public IEnumerable<Grupo> Grupos
        {
            set { comboBox.DataSource = value; }
        }

        public IEnumerable<Usuario> Usuarios
        {
            set { dataGrid.DataSource = value; }
        }

        public Grupo GrupoSelecionado
        {
            get { return (Grupo)comboBox.SelectedItem; }
        }

        #endregion
    }
}
