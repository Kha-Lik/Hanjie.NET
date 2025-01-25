namespace Shared

open System
open Shared.ImageProcessing.Transformers

type Todo = { Id: Guid; Description: string }

module Todo =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create (description: string) = {
        Id = Guid.NewGuid()
        Description = description
    }

type ITodosApi = {
    getTodos: unit -> Async<Todo list>
    addTodo: Todo -> Async<Todo list>
}

type IGreetingApi = {
    getGreeting: unit -> Async<string>
    getGreetingWithName: string -> Async<string>
}

type ImageProcessingRequest = {
    Transformers: TransformerDescriptor list
    Image: string
}

type ImageProcessingResponse = {
    ProcessedImage: string
}

type IImageApi = {
    getProcessedImage: ImageProcessingRequest -> Async<ImageProcessingResponse>
}