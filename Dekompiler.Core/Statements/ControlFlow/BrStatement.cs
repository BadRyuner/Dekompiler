using AsmResolver.PE.DotNet.Cil;
using Microsoft.AspNetCore.Components;

namespace Dekompiler.Core.Statements.ControlFlow;
public class BrStatement : CompleteStatement
{
    private int _id;

    public BrStatement(MethodContext context) : base(context)
    {
    }

    public override CilCode[] Codes => new[] { CilCode.Br, CilCode.Br_S };

    public override void Deserialize(CilInstruction instruction)
    {
        _id = ((ICilLabel)instruction.Operand!).Offset;
    }

    public override RenderFragment Render()
    {
        return Context.Renderer.Concat(Context.Renderer.Constants.Goto, Context.Renderer.Text($" IL_{_id}"));
    }
}