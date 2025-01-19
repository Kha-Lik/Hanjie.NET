module Server

open Giraffe
open SAFE
open Saturn
open Shared

module Storage =
    let todos =
        ResizeArray [
            Todo.create "Create new SAFE project"
            Todo.create "Write your app"
            Todo.create "Ship it!!!"
        ]

    let addTodo todo =
        if Todo.isValid todo.Description then
            todos.Add todo
            Ok()
        else
            Error "Invalid todo"

let todosApi ctx = {
    getTodos = fun () -> async { return Storage.todos |> List.ofSeq }
    addTodo =
        fun todo -> async {
            return
                match Storage.addTodo todo with
                | Ok() -> Storage.todos |> List.ofSeq
                | Error e -> failwith e
        }
}

let greetingsApi ctx = {
    getGreeting = fun () -> async { return "Hello from Saturn!" }
    getGreetingWithName =
        fun name -> async { return sprintf "Hello %s from Saturn!" name }
}

let todoApiHandler = Api.make todosApi
let greetingApiHandler = Api.make greetingsApi

let app = application {
    use_router (choose [
         todoApiHandler
         greetingApiHandler
    ])
    memory_cache
    use_static "public"
    use_gzip
}

[<EntryPoint>]
let main _ =
    run app
    0