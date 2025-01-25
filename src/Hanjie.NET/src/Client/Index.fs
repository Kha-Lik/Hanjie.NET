module Index

open Client.Greeting
open Client.IndexModules.Todo
open Elmish
open SAFE

type Model = {
    TodoModel: TodoModel
    GreetingModel: GreetingModel
}

type Msg =
    | Todo of TodoMsg
    | Greeting of GreetingMsg

let init () =
    let initialModel = { TodoModel = {Todos = NotStarted; Input = ""}; GreetingModel = { Name = ""; Greeting = NotStarted } }
    let initialCmd = Cmd.batch [
        Cmd.ofMsg (Todo (LoadTodos(Start())))
        Cmd.ofMsg (Greeting (LoadGreeting(Start())))
    ]

    initialModel, initialCmd

let update msg model =
    match msg with
    | Todo msg ->
        let updatedModel, cmd = updateTodo msg model.TodoModel
        {model with TodoModel = updatedModel}, Cmd.map Todo cmd
    | Greeting msg ->
        let updatedModel, cmd = updateGreeting msg model.GreetingModel
        {model with GreetingModel = updatedModel}, Cmd.map Greeting cmd

open Feliz

module ViewComponents =
    let greeting = GreetingView.greeting

    let todoList = TodoView.todoList

let view model dispatch =
    Html.section [
        prop.className "h-screen w-screen"
        prop.style [
            style.backgroundSize "cover"
            style.backgroundImageUrl "https://unsplash.it/1200/900?random"
            style.backgroundPosition "no-repeat center center fixed"
        ]

        prop.children [
            Html.a [
                prop.href "https://safe-stack.github.io/"
                prop.className "absolute block ml-12 h-12 w-12 bg-teal-300 hover:cursor-pointer hover:bg-teal-400"
                prop.children [ Html.img [ prop.src "/favicon.png"; prop.alt "Logo" ] ]
            ]

            Html.div [
                prop.className "flex flex-col items-center justify-center h-full"
                prop.children [
                    Html.h1 [
                        prop.className "text-center text-5xl font-bold text-white mb-3 rounded-md p-4"
                        prop.text "Hanjie.NET"
                    ]
                    ViewComponents.todoList model.TodoModel (fun msg -> dispatch (Todo msg))
                    ViewComponents.greeting model.GreetingModel (fun msg -> dispatch (Greeting msg))
                ]
            ]
        ]
    ]