using ExemploMVP_WebForm.Presenters;
using ExemploMVP_WebForm.Views.Contato;

namespace ExemploMVP_WebForm.Models
{
    public class ContatoModel : IContatoModel
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }

        public string GetNomeCompleto()
        {
            //var teste = new ContatoPresenter();
            //var teste = new ContatoPresenter(new ContatoPage());
            return $"{Nome} {Sobrenome}";
        }
    }
}