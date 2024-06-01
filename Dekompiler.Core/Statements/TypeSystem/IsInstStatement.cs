using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Cil;
using Dekompiler.Core.TypeSystem;
using Microsoft.AspNetCore.Components;

namespace Dekompiler.Core.Statements.TypeSystem;
internal class IsInstStatement : PushableStatement
{
    private ITypeDefOrRef _type = null!;
    private IStatement _target = null!;

    public IsInstStatement(MethodContext context) : base(context)
    {
    }

    public override CilCode[] Codes => [CilCode.Isinst];

    public override TypeSignature Type => _type.ToTypeSignature();

    public override void Deserialize(CilInstruction instruction)
    {
        _type = ((ITypeDefOrRef)instruction.Operand!);
        _target = Context.Stack.Pop();
        Context.Stack.Push(this);
    }

    public override RenderFragment Render()
    {
        var render = Context.Renderer;
        var space = Context.Renderer.Constants.Space;
        var type = new TypeRenderer(render, _type).RenderMember();
        return render.Concat(_target.Render(), space, render.Constants.Is, space, type);
    }
}
