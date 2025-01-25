module Client.Modules.Image

open SAFE
open Shared

let imageApi = Api.makeProxy<IImageApi> ()