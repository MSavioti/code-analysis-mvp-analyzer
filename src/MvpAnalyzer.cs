using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace TP5
{
    public class MvpAnalyzer
    {
        private string _modelsPath;
        private string _presentersPath;
        private string _viewsPath;
        private string[] _pagesBackEnd;
        private readonly List<string> _modelsTypes;
        private readonly List<string> _modelsUsedTypes;
        private readonly List<string> _presenterTypes;
        private readonly List<string> _viewTypes;
        private readonly List<string> _viewUsedTypes;
        private readonly MSBuildWorkspace _workspace;
        private string _sourcePath;

        public MvpAnalyzer(MSBuildWorkspace workspace)
        {
            _modelsTypes = new List<string>();
            _modelsUsedTypes = new List<string>();
            _presenterTypes = new List<string>();
            _viewTypes = new List<string>();
            _viewUsedTypes = new List<string>();
            _workspace = workspace;
        }

        public void Analyze(string srcPath)
        {
            _sourcePath = srcPath;
            PrintStartUpMessage();

            if (!ValidateDirectories(srcPath)) return;

            _pagesBackEnd = FindPagesBackEndFiles(srcPath);

            if (_pagesBackEnd == null) return;

            FindProjectTypes();

            if (!AnalyzeModels()) return;

            if (!AnalyzeViews()) return;

            PrintSuccessMessage(srcPath);
        }

        private bool ValidateDirectories(string srcPath)
        {
            _modelsPath = FindDirectory(srcPath, "*model*", "Model", "Models");
            _presentersPath = FindDirectory(srcPath, "*presenter*", "Presenter", "Presenters");
            _viewsPath = FindDirectory(srcPath, "*view*", "View", "Views");

            if ((_modelsPath.IsEmpty()) || (_presentersPath.IsEmpty()) || (_viewsPath.IsEmpty()))
                return false;

            return true;
        }

        private string[] FindPagesBackEndFiles(string srcPath)
        {
            var files = Directory.GetFiles(srcPath, "*.aspx.cs", SearchOption.AllDirectories);

            if (files.IsEmpty())
            {
                Console.WriteLine("Não foram encontrados arquivos de back-end para as páginas do projeto.");
                return null;
            }

            return files;
        }

        private bool AnalyzeModels()
        {
            foreach (var viewType in _viewTypes)
            {
                if (_modelsUsedTypes.Contains(viewType))
                {
                    string errorMessage = $"O tipo {viewType} é declarado dentro de ";
                    errorMessage += "um arquivo de domínio, o que não é permitido no padrão MVP.";
                    PrintErrorMessage(errorMessage);
                    return false;
                }
            }

            foreach (var presenterType in _presenterTypes)
            {
                if (_modelsUsedTypes.Contains(presenterType))
                {
                    string errorMessage = $"O tipo {presenterType} é declarado dentro de ";
                    errorMessage += "um arquivo de domínio, o que não é permitido no padrão MVP.";
                    PrintErrorMessage(errorMessage);
                    return false;
                }
            }
            
            return true;
        }

        private bool AnalyzeViews()
        {
            foreach (var modelType in _modelsTypes)
            {
                if (_viewUsedTypes.Contains(modelType))
                {
                    string errorMessage = $"O tipo {modelType} é declarado dentro de ";
                    errorMessage += "um arquivo de visão, o que não é permitido no padrão MVP.";
                    PrintErrorMessage(errorMessage);
                    return false;
                }
            }
            
            return true;
        }

        private string[] GetCsharpFiles(string directory)
        {
            return Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
        }

        private string[] GetAspxFiles(string directory)
        {
            return Directory.GetFiles(directory, "*.aspx", SearchOption.AllDirectories);
        }

        private void FindAllModelTypes()
        {
            foreach (var modelFile in GetCsharpFiles(_modelsPath))
            {
                var visitor = new MvpVisitor(modelFile, _workspace);
                _modelsTypes.AddMultipleIfNotContains(visitor.GetTargetTypes());
                _modelsUsedTypes.AddMultipleIfNotContains(visitor.GetUsedTypes());
            }
        }

        private void FindAllPresenterTypes()
        {
            foreach (var presenterFile in GetCsharpFiles(_presentersPath))
            {
                var visitor = new MvpVisitor(presenterFile, _workspace);
                _presenterTypes.AddMultipleIfNotContains(visitor.GetTargetTypes());
            }
        }

        private void FindAllViewTypes()
        {
            var viewFiles = GetCsharpFiles(_viewsPath);

            foreach (var viewFile in GetCsharpFiles(_viewsPath))
            {
                var visitor = new MvpVisitor(viewFile, _workspace);
                _viewTypes.AddMultipleIfNotContains(visitor.GetTargetTypes());
            }

            foreach (var viewFile in (_pagesBackEnd))
            {
                var visitor = new MvpVisitor(viewFile, _workspace);
                _viewTypes.AddMultipleIfNotContains(visitor.GetTargetTypes());
            }

            foreach (var aspxcsFile in _pagesBackEnd)
            {
                var visitor = new MvpVisitor(aspxcsFile, _workspace);
                _viewUsedTypes.AddMultipleIfNotContains(visitor.GetUsedTypes());
            }
        }

        private void PrintStartUpMessage()
        {
            Console.WriteLine("_______________________");
            Console.WriteLine("| INICIANDO OPERAÇÕES |");
            Console.WriteLine("_______________________\n");
        }

        private void PrintSuccessMessage(string projectPath)
        {
            Console.WriteLine("\n- - - - -  SUCESSO! - - - - -\n");
            Console.WriteLine($"A análise no projeto localizado em {_sourcePath} foi concluída com sucesso.\n");
            Console.WriteLine("A análise concluiu que o projeto ATENDE aos requisitos do padrão arquitetural MVP.");
            PrintOnProgramEnd();
        }

        private void PrintErrorMessage(string errorMessage)
        {
            Console.WriteLine("\n- - - - - - - ERRO! - - - - - -\n");
            Console.WriteLine($"A análise no projeto localizado em {_sourcePath} foi interrompida com erro:\n");
            Console.WriteLine(errorMessage);
            PrintOnProgramEnd();
        }

        private void PrintOnProgramEnd()
        {
            Console.WriteLine("\nExecução do programa finalizada");
        }

        private void FindProjectTypes()
        {
            FindAllModelTypes();
            FindAllPresenterTypes();
            FindAllViewTypes();
        }

        private void Debug()
        {
            Console.WriteLine("\nModel types");
            foreach (var type in _modelsTypes)
            {
                Console.WriteLine(type);
            }

            Console.WriteLine("\nModel used types");
            foreach (var type in _modelsUsedTypes)
            {
                Console.WriteLine(type);
            }

            Console.WriteLine("\nPresenter types");
            foreach (var type in _presenterTypes)
            {
                Console.WriteLine(type);
            }

            Console.WriteLine("\nView types");
            foreach (var type in _viewTypes)
            {
                Console.WriteLine(type);
            }

            Console.WriteLine("\nView used types");
            foreach (var type in _viewUsedTypes)
            {
                Console.WriteLine(type);
            }
        }

        private string FindDirectory(string srcPath, string wildCard, string singularName, string pluralName)
        {
            var paths = Directory.GetDirectories(srcPath, wildCard, SearchOption.AllDirectories);

            if (paths.Length == 0)
            {
                Console.WriteLine($"Não foi possível encontrar um diretório com o nome {singularName} ou {pluralName}.");
                return string.Empty;
            }
            
            if (paths.Length == 1)
            {
                Console.WriteLine($"Usando o diretório \"{paths[0]}\" para {pluralName}.");
                return paths[0];
            }

            paths = Directory.GetDirectories(srcPath, singularName, SearchOption.AllDirectories);

            if (paths.Length == 1)
            {
                Console.WriteLine($"Usando o diretório \"{paths[0]}\" para {pluralName}.");
                return paths[0];
            }

            paths = Directory.GetDirectories(srcPath, pluralName, SearchOption.AllDirectories);

            if (paths.Length == 1)
            {
                Console.WriteLine($"Usando o diretório \"{paths[0]}\" para {pluralName}.");
                return paths[0];
            }

            Console.WriteLine($"Não foi possível encontrar um diretório com o nome {singularName} ou {pluralName}.");
            return string.Empty;
        }
    }
}
