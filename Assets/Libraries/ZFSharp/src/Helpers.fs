namespace ZFSharp

[<AutoOpen>]
module internal Helpers =

//#region 引用 / 别名
    type Task = System.Threading.Tasks.Task
    type FTask = System.Func<Task>
    type FUnitTask = System.Func<unit, Task>
    type Action = System.Action
    type Enum = System.Enum
    type Mono = UnityEngine.MonoBehaviour
    type TextMP = TMPro.TextMeshProUGUI
    type UI = SystemUIManager
    type AP<'T when 'T :> AdProcessor<'T>> = AdProcessor<'T>

    let taskDone = Task.CompletedTask
//#endregion

    let delay value () = value

    let wait = delay taskDone
