using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SDILReader;

namespace Teste
{
    public class VerificadorDeBinding
    {
        public VerificadorDeBinding()
        {
            Globals.LoadOpCodes();
        }

        public string GetBingingErrors(IEnumerable<Type> types)
        {
            string erros = "";
            foreach (var type in types)
            {
                //Busca propriedades que permitem escrita e retornam um IEnumerable
                var properties = type.GetProperties()
                                .Where(x => x.CanWrite && EhLista(x));

                //Processa cada propriedade
                foreach (var property in properties)
                {
                    erros += ProcessProperty(type, property);
                }
            }

            return erros;
        }

        private string ProcessProperty(Type type, PropertyInfo property)
        {
            //Carrega o código da propriedade e busca uma referência ao componente
            string erros = "";
            var modelType = GetModel(property);
            var methodReader = new MethodBodyReader(property.GetSetMethod());
            if (methodReader.instructions != null)
            {
                foreach (ILInstruction instruction in methodReader.instructions)
                {
                    //Busca onde está usando o componente
                    if (instruction.Code.Name == "ldfld")
                    {
                        //achou o nome da variável do componente e o seu tipo
                        string fieldName = (instruction.Operand as FieldInfo).Name;
                        Type controlType = (instruction.Operand as FieldInfo).FieldType;

                        object instance = Activator.CreateInstance(type);
                        object componentInstance = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance);

                        //processa o componente verificando os bindings
                        erros += ProcessComponent(modelType, type, fieldName, componentInstance);
                    }
                }
            }

            return erros;
        }

        private string ProcessComponent(Type modelType, Type telaType, string componentName, object componentInstance)
        {
            //de acordo com o tipo do componente faz um processamento distinto
            string erros = "";
            if (componentInstance is DataGridView)
            {
                //Verifica em cada coluna se o DataPropertyName está correto
                DataGridView grid = (DataGridView)componentInstance;
                foreach (DataGridViewColumn column in grid.Columns)
                {
                    if (!String.IsNullOrEmpty(column.DataPropertyName))
                    {
                        if (!ModeloContemPropriedade(modelType, column.DataPropertyName))
                        {
                            erros += GetLog(column.DataPropertyName, modelType.Name, "DataPropertyName", componentName, telaType.FullName);
                        }
                    }
                }

            }
            else if (componentInstance is ComboBox)
            {
                //Verifica se o DisplayMember e o ValueMember estão corretos
                ComboBox combo = (ComboBox)componentInstance;
                if (!String.IsNullOrEmpty(combo.DisplayMember))
                {
                    if (!ModeloContemPropriedade(modelType, combo.DisplayMember))
                    {
                        erros += GetLog(combo.DisplayMember, modelType.Name, "DisplayMember", componentName, telaType.FullName);
                    }
                }

                if (!String.IsNullOrEmpty(combo.ValueMember))
                {
                    if (!ModeloContemPropriedade(modelType, combo.ValueMember))
                    {
                        erros += GetLog(combo.ValueMember, modelType.Name, "ValueMember", componentName, telaType.FullName);                       
                    }
                }
            }

            return erros;
        }

        private string GetLog(string nomePropriedade, string nomeModelo, string binding, string componente, string tela)
        {
            return String.Format("A propriedade '{0}' não existe no modelo '{1}' (definido na propriedade {2} do componente '{3}' da tela '{4}')\r\n",
                                nomePropriedade, nomeModelo, binding, componente, tela);
        }

        private bool ModeloContemPropriedade(Type modelType, string propertyName)
        {
            return modelType.GetProperty(propertyName) != null;
        }

        private bool EhLista(PropertyInfo propertyInfo)
        {
            return TipoEhEnumerable(propertyInfo.PropertyType) ||
                   propertyInfo.PropertyType.GetInterfaces().Any(x => TipoEhEnumerable(x));
        }

        private bool TipoEhEnumerable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private Type GetModel(PropertyInfo propertyInfo)
        {
            Type type = propertyInfo.PropertyType;
            if (!TipoEhEnumerable(propertyInfo.PropertyType))
            {
                type = propertyInfo.PropertyType.GetInterfaces().First(x => TipoEhEnumerable(x)); 
            }

            return type.GetGenericArguments()[0];
        }
    }
}
