module Client.Image

open SAFE
open Shared

let imageApi = Api.makeProxy<IImageApi> ()