module Client.Modules.IndexModules.Greeting

open System
open Elmish
open Feliz
open SAFE
open Shared

type Model = {
    Name: string
    Greeting: RemoteData<string>
}

type GreetingMsg =
    | SetName of string
    | LoadGreeting of ApiCall<unit, string>
    | LoadGreetingWithName of ApiCall<string, string>

let greetingsApi = Api.makeProxy<IGreetingApi> ()

let init () = { Name = ""; Greeting = NotStarted }, Cmd.ofMsg (LoadGreeting(Start()))

let update msg model =
    match msg with
    | SetName value -> { model with Name = value }, Cmd.none
    | LoadGreeting msg ->
        match msg with
        | Start() ->
            let loadGreetingCmd = Cmd.OfAsync.perform greetingsApi.getGreeting () (Finished >> LoadGreeting)

            { model with Greeting = model.Greeting.StartLoading() }, loadGreetingCmd
        | Finished greeting -> { model with Greeting = Loaded greeting }, Cmd.none
    | LoadGreetingWithName msg ->
        match msg with
        | Start name ->
            let loadGreetingWithNameCmd =
                Cmd.OfAsync.perform greetingsApi.getGreetingWithName name (Finished >> LoadGreetingWithName)

            { model with Greeting = model.Greeting.StartLoading() }, loadGreetingWithNameCmd
        | Finished greeting -> { model with Greeting = Loaded greeting }, Cmd.none

module View =
    let greetingAction model dispatch =
        Html.div [
            prop.className "flex flex-col sm:flex-row mt-4 gap-4"
            prop.children [
                Html.input [
                    prop.className
                        "shadow appearance-none border rounded w-full py-2 px-3 outline-none focus:ring-2 ring-teal-300 text-grey-darker"
                    prop.value model.Name
                    prop.placeholder "What's your name?"
                    prop.autoFocus true
                    prop.onChange (SetName >> dispatch)
                    prop.onKeyPress (fun ev ->
                        if ev.key = "Enter" then
                            dispatch (LoadGreetingWithName(Start model.Name)))
                ]
                Html.button [
                    prop.className
                        "flex-no-shrink p-2 px-12 rounded bg-teal-600 outline-none focus:ring-2 ring-teal-300 font-bold text-white hover:bg-teal disabled:opacity-30 disabled:cursor-not-allowed"
                    prop.disabled (String.IsNullOrWhiteSpace model.Name)
                    prop.onClick (fun _ -> dispatch (LoadGreetingWithName(Start model.Name)))
                    prop.text "Get Greeting"
                ]
            ]
        ]

    let greeting model dispatch =
        Html.div [
            prop.className "bg-white/80 rounded-md shadow-md p-4 w-5/6 lg:w-3/4 lg:max-w-2xl"
            prop.children [
                Html.div [
                    prop.className "text-center text-2xl font-bold text-teal-600 mb-3"
                    prop.children [
                        match model.Greeting with
                        | NotStarted -> Html.text "Not Started."
                        | Loading None -> Html.text  "Loading..."
                        | Loading (Some greeting)
                        | Loaded greeting -> Html.text  greeting
                    ]
                ]

                greetingAction model dispatch
            ]
        ]