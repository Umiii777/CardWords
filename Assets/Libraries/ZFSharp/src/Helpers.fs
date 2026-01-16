namespace ZFSharp

[<AutoOpen>]
module internal Helpers =

//#region 引用 / 别名
    type Task = System.Threading.Tasks.Task
    type FTask = System.Func<Task>
    type Action = System.Action
    type TextMP = TMPro.TextMeshProUGUI

    let taskDone = Task.CompletedTask
//#endregion

    let delay value () = value

    let wait = delay taskDone
