namespace ZFSharp

type CheckInState =
    | Received = 0
    | Outdated = 1
    | Available = 2
    | Future = 3

type PlayerCheckIn () =
    class end //FIXME: 001_实现该类型，然后删除这行临时代码
