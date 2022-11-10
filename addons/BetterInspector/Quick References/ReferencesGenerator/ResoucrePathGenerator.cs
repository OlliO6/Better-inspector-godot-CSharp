using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class ResoucrePathGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            var root = syntaxTree.GetRoot();
            string expectedName = Path.GetFileNameWithoutExtension(syntaxTree.FilePath);

            ClassDeclarationSyntax @class = root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .FirstOrDefault(classNode => classNode.Identifier.ToString() == expectedName);

            if (@class == null)
                continue;

            foreach (var attributes in @class.AttributeLists)
            {
                if (attributes.Attributes.Any(attribute => attribute.Name.ToString() == "Resource"))
                {
                    GenerateSource(syntaxTree, @class);
                }
            }
        }

        void GenerateSource(SyntaxTree syntaxTree, ClassDeclarationSyntax @class)
        {
            var usings = syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>();

            var @namespace = syntaxTree.GetRoot().DescendantNodes()
                    .OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();

            StringBuilder sb = new();


            foreach (var use in usings)
                sb.AppendLine(use.ToString());
            sb.AppendLine();

            if (@namespace is not null)
            {
                sb.AppendLine($"namespace {@namespace?.Name.ToString()}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"[ResourceScriptPath(@\"{syntaxTree.FilePath}\")]");
            sb.AppendLine($"partial class {@class.Identifier} : {@class.BaseList?.Types.First()?.Type.ToString()}");
            sb.AppendLine("{ }");

            if (@namespace is not null)
                sb.AppendLine("}");

            context.AddSource($"{@class.Identifier}.g.cs", sb.ToString());
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }
}
