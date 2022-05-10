using ExemploMVP_WebForm.Models;
using ExemploMVP_WebForm.Views.Contato;

namespace ExemploMVP_WebForm.Presenters
{
    public class ContatoPresenter
    {
        private readonly IContatoView _view;

        // public ContatoPresenter()
        // {
        // }

        public ContatoPresenter(IContatoView view)
        {
            _view = view;
        }

        public ContatoPresenter(IContatoView view, ContatoModel model)
        {
            _view = view;
        }

        public void Buscar(string nome)
        {
            var contato = BuscarContato(nome);

            if (contato != null)
            {
                _view.NomeMostrado = contato.GetNomeCompleto();
            }
        }

        public IContatoModel BuscarContato(string nome)
        {
            return new ContatoModel();
        }
    }
}