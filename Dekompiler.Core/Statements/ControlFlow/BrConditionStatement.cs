using AsmResolver.PE.DotNet.Cil;
using Microsoft.AspNetCore.Components;

namespace Dekompiler.Core.Statements.ControlFlow;

public class BrConditionStatement : CompleteStatement
{
    private int _id;
    private IStatement _firstValue = null!;
    private IStatement? _secondValue;
    private CilCode _code;

    public BrConditionStatement(MethodContext context) : base(context)
    {
    }

    public override CilCode[] Codes => new[] 
    {
        CilCode.Brfalse, CilCode.Brfalse_S,
        CilCode.Brtrue, CilCode.Brtrue_S,
        CilCode.Beq, CilCode.Ble, CilCode.Bge, CilCode.Bgt, CilCode.Blt,
        CilCode.Beq_S, CilCode.Ble_S, CilCode.Bge_S, CilCode.Bgt_S, CilCode.Blt_S,
        CilCode.Bne_Un, CilCode.Ble_Un, CilCode.Bge_Un, CilCode.Bgt_Un, CilCode.Blt_Un,
        CilCode.Bne_Un_S, CilCode.Ble_Un_S, CilCode.Bge_Un_S, CilCode.Bgt_Un_S, CilCode.Blt_Un_S,
    };

    public override void Deserialize(CilInstruction instruction)
    {
        _code = instruction.OpCode.Code;
        _id = ((ICilLabel)instruction.Operand!).Offset;
        switch(_code)
        {
            case CilCode.Beq:
            case CilCode.Ble:
            case CilCode.Bge:
            case CilCode.Bgt:
            case CilCode.Blt:
            case CilCode.Ble_Un:
            case CilCode.Bge_Un:
            case CilCode.Bgt_Un:
            case CilCode.Blt_Un:
            case CilCode.Bne_Un:
                _secondValue = Context.Stack.Pop();
                _firstValue = Context.Stack.Pop();
                break;
            default:
                _firstValue = Context.Stack.Pop()!;
                break;
        }
    }

    public override RenderFragment Render()
    {
        var render = Context.Renderer;
        var space = render.Constants.Space;
        var rightBracket = render.Constants.RightBracket;
        var leftBracket = render.Constants.LeftBracket;
        var result = render.Concat(render.Constants.If, space, rightBracket); // if (

        result = _code switch
        {
            CilCode.Brtrue => render.Concat(result, _firstValue.Render()),
            CilCode.Brfalse => render.Concat(result, render.Constants.Bang, _firstValue.Render()),
            _ when _secondValue is not null => _code switch
            {
                CilCode.Beq => render.Concat(result, _firstValue.Render(), space, render.Constants.Equal, render.Constants.Equal, space, _secondValue.Render()),
                CilCode.Bne_Un => render.Concat(result, _firstValue.Render(), space, render.Constants.NotEqual, space, _secondValue.Render()),
                CilCode.Blt => render.Concat(result, _firstValue.Render(), space, render.Constants.RightThan, space, _secondValue.Render()),
                CilCode.Bgt => render.Concat(result, _firstValue.Render(), space, render.Constants.LeftThan, space, _secondValue.Render()),
                CilCode.Ble => render.Concat(result, _firstValue.Render(), space, render.Constants.RightThanOrEqual, space, _secondValue.Render()),
                CilCode.Bge => render.Concat(result, _firstValue.Render(), space, render.Constants.LeftThanOrEqual, space, _secondValue.Render()),
                CilCode.Blt_Un => render.Concat(result, render.Constants.Unchecked, rightBracket,  _firstValue.Render(), space, render.Constants.RightThan, space, _secondValue.Render(), leftBracket),
                CilCode.Bgt_Un => render.Concat(result, render.Constants.Unchecked, rightBracket, _firstValue.Render(), space, render.Constants.LeftThan, space, _secondValue.Render(), leftBracket),
                CilCode.Ble_Un => render.Concat(result, render.Constants.Unchecked, rightBracket, _firstValue.Render(), space, render.Constants.RightThanOrEqual, space, _secondValue.Render(), leftBracket),
                CilCode.Bge_Un => render.Concat(result, render.Constants.Unchecked, rightBracket, _firstValue.Render(), space, render.Constants.LeftThanOrEqual, space, _secondValue.Render(), leftBracket),
                _ => render.Concat(result, render.Span($"/* unsupported op {_code} */", "comment"))
            },
            _ => render.Concat(result, render.Span($"/* unsupported op {_code} */", "comment"))
        };

        return render.Concat(result, leftBracket, // condition)
            space, render.Constants.Goto, render.Text($" IL_{_id}")); // goto IL_id;
    }
}
