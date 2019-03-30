namespace ViewControllers

open Microsoft.AspNetCore.Mvc

type TestView() = 
    inherit Controller()

    [<Route("/test")>]
    member this.Index() = 
        this.View()
