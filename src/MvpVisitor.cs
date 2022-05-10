using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.MSBuild;

namespace TP5
{
    public class MvpVisitor : CSharpSyntaxVisitor
    {
        private readonly List<string> _targetTypes;
        private readonly List<string> _usedTypes;
        private readonly SyntaxTree _tree;
        private bool _hasVisited;
        private readonly SemanticModel _model;
        private readonly SyntaxGenerator _syntaxGenerator;

        public MvpVisitor(string filePath, MSBuildWorkspace workspace)
        {
            _targetTypes = new List<string>();
            _usedTypes = new List<string>();
            _tree = ParseProgramFile(filePath);
            _hasVisited = false;
            _model = GetSemanticModel();
            _syntaxGenerator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);
        }

        #region Public methods

        #region Analysis methods

        public void StartVisiting()
        {
            if (_hasVisited)
                return;

            VisitAllNodes(_tree.GetRoot());
            _hasVisited = true;
        }

        public string[] GetTargetTypes()
        {
            StartVisiting();
            return _targetTypes.ToArray();
        }

        public string[] GetUsedTypes()
        {
            StartVisiting();
            return _usedTypes.ToArray();
        }

        #endregion

        #region Overriden methods

        #region Target type finding

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            _targetTypes.AddIfNotContains(node.Identifier.ToString());
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            _targetTypes.AddIfNotContains(node.Identifier.ToString());
        }

        #endregion

        #region Used type finding

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            _usedTypes.AddIfNotContains(node.ReturnType.GetFirstToken().ToString());

            if (node.ParameterList.Parameters.Any())
            {
                foreach (var parameter in node.ParameterList.Parameters)
                {
                    _usedTypes.AddIfNotContains(parameter.Type.GetFirstToken().ToString());
                }
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            _usedTypes.AddIfNotContains(node.Type.GetFirstToken().ToString());
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            _usedTypes.AddIfNotContains(node.Type.GetFirstToken().ToString());
            
        }

        #endregion

        #endregion

        #endregion

        #region Private methods

        public static SyntaxTree ParseProgramFile(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8);
            return CSharpSyntaxTree.ParseText(streamReader.ReadToEnd());
        }

        private void VisitAllNodes(SyntaxNode root)
        {
            VisitChildrenNodes(root);
        }

        private void VisitChildrenNodes(SyntaxNode node)
        {
            foreach (var childNode in node.ChildNodes())
            {
                Visit(childNode);

                if (childNode.ChildNodes().Any())
                {
                    VisitChildrenNodes(childNode);
                }
            }
        }

        private SemanticModel GetSemanticModel()
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("TP5", syntaxTrees: new[] { _tree }, references: new[] { mscorlib });
            return compilation.GetSemanticModel(_tree);
        }

        #endregion
    }
}
