using AsmResolver.PE.DotNet.Cil;
using Microsoft.AspNetCore.Components;

namespace Dekompiler.Core.Statements.Pointers;
public class InitobjStatement : CompleteStatement
{
    private IStatement _target = null!;

    public InitobjStatement(MethodContext context) : base(context)
    {
    }

    public override CilCode[] Codes => [CilCode.Initobj];

    public override void Deserialize(CilInstruction instruction)
    {
        _target = Context.Stack.Pop();
    }

    public override RenderFragment Render()
    {
        var render = Context.Renderer;
        var sym = render.Constants;
        return render.Concat(_target.Render(), sym.Space, sym.Equal, sym.Space, sym.Default);
    }
}
