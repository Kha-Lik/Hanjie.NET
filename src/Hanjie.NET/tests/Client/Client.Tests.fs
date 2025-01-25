module Client.Tests

open Client.Modules.TodoList
open Fable.Mocha

open Client.Modules.Index
open Shared
open SAFE

let client =
    testList "Client" [
        testCase "Added todo"
        <| fun _ ->
            let newTodo = Todo.create "new todo"
            let model, _ = init ()
            let model, _ = update (TodoMsg (SaveTodo(Finished [ newTodo ]))) model

            Expect.equal
                (model.TodoModel.Todos |> RemoteData.map _.Length |> RemoteData.defaultValue 0)
                1
                "There should be 1 todo"

            Expect.equal
                (model.TodoModel.Todos
                 |> RemoteData.map List.head
                 |> RemoteData.defaultValue (Todo.create ""))
                newTodo
                "Todo should equal new todo"
    ]

let all =
    testList "All" [
        #if FABLE_COMPILER // This preprocessor directive makes editor happy
        Shared.Tests.shared
#endif
                client
    ]

[<EntryPoint>]
let main _ = Mocha.runTests all