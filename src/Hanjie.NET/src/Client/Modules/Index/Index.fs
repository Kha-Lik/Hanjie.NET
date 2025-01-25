module Client.Modules.Index

open Client.Modules.IndexModules
open Elmish
open Feliz.Router

type Model = {
    TodoModel: TodoList.Model
    GreetingModel: Greeting.Model
}

type Msg =
    | TodoMsg of TodoList.TodoMsg
    | GreetingMsg of Greeting.GreetingMsg

let init () =
    let initTodo = TodoList.init ()
    let initGreeting = Greeting.init ()
    let initialModel = { TodoModel = fst initTodo; GreetingModel = fst initGreeting }
    let initialCmd = Cmd.batch [
        snd initTodo |> Cmd.map TodoMsg
        snd initGreeting |> Cmd.map GreetingMsg
        ]

    initialModel, initialCmd


let update msg model =
    match msg with
    | TodoMsg msg ->
        let updatedModel, cmd = TodoList.update msg model.TodoModel
        {model with TodoModel = updatedModel}, Cmd.map TodoMsg cmd
    | GreetingMsg msg ->
        let updatedModel, cmd = Greeting.update msg model.GreetingModel
        {model with GreetingModel = updatedModel}, Cmd.map GreetingMsg cmd

open Feliz

module ViewComponents =
    let greeting = Greeting.View.greeting

    let todoList = TodoList.View.todoList

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
                    ViewComponents.todoList model.TodoModel (fun msg -> dispatch (TodoMsg msg))
                    ViewComponents.greeting model.GreetingModel (fun msg -> dispatch (GreetingMsg msg))
                ]
            ]
        ]
    ]