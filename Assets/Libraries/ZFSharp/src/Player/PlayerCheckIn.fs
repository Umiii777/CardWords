namespace ZFSharp

type CheckInState =
    | Received = 0
    | Outdated = 1
    | Available = 2
    | Future = 3
