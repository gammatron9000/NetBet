namespace ViewControllers

open Microsoft.AspNetCore.Mvc

type HomeView() = 
    inherit Controller()

    [<Route("/")>]
    member this.Index() = 
        this.View()