using AsmResolver.PE.DotNet.Cil;
using Microsoft.AspNetCore.Components;

namespace Dekompiler.Core.Statements.Miscellaneous;

public class LabelStatement : CompleteStatement
{
    private int _id;

    public LabelStatement(MethodContext context, int id) : base(context)
    {
        _id = id;
    }

    public override CilCode[] Codes => Array.Empty<CilCode>();

    public override void Deserialize(CilInstruction instruction)
    {
    }

    public override RenderFragment Render()
    {
        return Context.Renderer.Text($"IL_{_id}:");
    }
}