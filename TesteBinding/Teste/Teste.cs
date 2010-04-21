using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using SDILReader;
using TesteBinding;
using System.Diagnostics;

namespace Teste
{
    [TestFixture]
    public class Teste
    {
        [Test]
        public void VerificaBindings()
        {
            var verificador = new VerificadorDeBinding();

            //Busca todos os tipos que extendem Form no assembly do projeto
            var types = typeof (Form1).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof (Form)));

            //Verifica os bindings
            string erros = verificador.GetBingingErrors(types);
            
            //Se retornou algo, exibe o erro
            if (!String.IsNullOrEmpty(erros))
            {
                Assert.Fail(erros);
            }
        }
    }
}
