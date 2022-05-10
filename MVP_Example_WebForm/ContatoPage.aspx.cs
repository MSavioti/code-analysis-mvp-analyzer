using System;
using ExemploMVP_WebForm.Presenters;
using ExemploMVP_WebForm.Views.Contato;
using ExemploMVP_WebForm.Models;

namespace ExemploMVP_WebForm
{
    public partial class ContatoPage : System.Web.UI.Page, IContatoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void BuscarNome(object sender, EventArgs e)
        {
            //var presenter = new ContatoPresenter(this);
            var presenter = new ContatoPresenter(this, new ContatoModel());
            presenter.Buscar(CaixaTexto.Text);
            NomeBuscado.Text = NomeMostrado;
        }

        public string NomeMostrado { get; set; }
    }
}