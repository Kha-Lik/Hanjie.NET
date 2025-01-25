module Client.IndexModules.Todo

open Elmish
open Feliz
open SAFE
open Shared

type TodoModel = {
    Todos: RemoteData<Todo list>
    Input: string
}

type TodoMsg =
    | SetInput of string
    | LoadTodos of ApiCall<unit, Todo list>
    | SaveTodo of ApiCall<string, Todo list>

let todosApi = Api.makeProxy<ITodosApi> ()

let updateTodo msg model =
    match msg with
    | SetInput value -> { model with Input = value }, Cmd.none
    | SaveTodo msg ->
        match msg with
        | Start todoText ->
            let saveTodoCmd =
                let todo = Todo.create todoText
                Cmd.OfAsync.perform todosApi.addTodo todo (Finished >> SaveTodo)

            { model with Input = "" }, saveTodoCmd
        | Finished todos ->
            { model with Todos = RemoteData.Loaded todos },
            Cmd.none
    | LoadTodos msg ->
        match msg with
        | Start() ->
            let loadTodosCmd = Cmd.OfAsync.perform todosApi.getTodos () (Finished >> LoadTodos)

            { model with Todos = model.Todos.StartLoading() }, loadTodosCmd
        | Finished todos -> { model with Todos = Loaded todos }, Cmd.none

module TodoView =
    let todoAction model dispatch =
        Html.div [
            prop.className "flex flex-col sm:flex-row mt-4 gap-4"
            prop.children [
                Html.input [
                    prop.className
                        "shadow appearance-none border rounded w-full py-2 px-3 outline-none focus:ring-2 ring-teal-300 text-grey-darker"
                    prop.value model.Input
                    prop.placeholder "What needs to be done?"
                    prop.autoFocus false
                    prop.onChange (SetInput >> dispatch)
                    prop.onKeyPress (fun ev ->
                        if ev.key = "Enter" then
                            dispatch (SaveTodo(Start model.Input)))
                ]
                Html.button [
                    prop.className
                        "flex-no-shrink p-2 px-12 rounded bg-teal-600 outline-none focus:ring-2 ring-teal-300 font-bold text-white hover:bg-teal disabled:opacity-30 disabled:cursor-not-allowed"
                    prop.disabled (Todo.isValid model.Input |> not)
                    prop.onClick (fun _ -> dispatch (SaveTodo(Start model.Input)))
                    prop.text "Add"
                ]
            ]
        ]

    let todoList model dispatch =
        Html.div [
            prop.className "bg-white/80 rounded-md shadow-md p-4 w-5/6 lg:w-3/4 lg:max-w-2xl"
            prop.children [
                Html.ol [
                    prop.className "list-decimal ml-6"
                    prop.children [
                        match model.Todos with
                        | NotStarted -> Html.text "Not Started."
                        | Loading None -> Html.text "Loading..."
                        | Loading (Some todos)
                        | Loaded todos ->
                            for todo in todos do
                                Html.li [ prop.className "my-1"; prop.text todo.Description ]
                    ]
                ]

                todoAction model dispatch
            ]
        ]
