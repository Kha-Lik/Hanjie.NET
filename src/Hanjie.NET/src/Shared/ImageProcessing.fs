namespace Shared.ImageProcessing

open Microsoft.FSharp.Core
open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats
open SixLabors.ImageSharp.Processing

module Processors =

    type BlurMode =
        | Gaussian
        | Box
        | Median

    type BlurParameter =
        | Amount of float32
        | Radius of int

    type ResamplingMode =
        | NearestNeighbor
        | BoxSampling
        | Bicubic
        | Lanczos2
        | Lanczos3
        | Lanczos5
        | RobidouxSharp
        | Spline

    let private getSampler mode =
        match mode with
        | NearestNeighbor -> KnownResamplers.NearestNeighbor
        | BoxSampling -> KnownResamplers.Box
        | Bicubic -> KnownResamplers.Bicubic
        | Lanczos2 -> KnownResamplers.Lanczos2
        | Lanczos3 -> KnownResamplers.Lanczos3
        | Lanczos5 -> KnownResamplers.Lanczos5
        | RobidouxSharp -> KnownResamplers.RobidouxSharp
        | Spline -> KnownResamplers.Spline

    let grayscale (mode: GrayscaleMode) (amount: float32) (image: Image<Rgba32>)=
        image.Mutate(fun ctx -> ctx.Grayscale(mode, amount) |> ignore)
        image

    let resize mode (width: int) (height: int) (image: Image<Rgba32>) =
        let sampler = getSampler mode
        image.Mutate(fun ctx -> ctx.Resize(width, height, sampler) |> ignore)
        image

    let rotate (degrees: float32) (image: Image<Rgba32>)=
        image.Mutate(fun ctx -> ctx.Rotate(degrees) |> ignore)
        image

    let contrast (amount: float32) (image: Image<Rgba32>) =
        image.Mutate(fun ctx -> ctx.Contrast(amount) |> ignore)
        image

    let binaryThreshold (mode: BinaryThresholdMode) (threshold: float32) (image: Image<Rgba32>) =
        image.Mutate(fun ctx -> ctx.BinaryThreshold(threshold, mode) |> ignore)
        image

    let blur mode parameter (image: Image<Rgba32>) =
        match mode, parameter with
        | Gaussian, Amount amount -> image.Mutate(fun ctx -> ctx.GaussianBlur(amount) |> ignore)
        | Box, Radius radius -> image.Mutate(fun ctx -> ctx.BoxBlur(radius) |> ignore)
        | Median, Radius radius -> image.Mutate(fun ctx -> ctx.MedianBlur(radius, true) |> ignore)
        | _ -> failwith "Invalid blur mode or parameter"
        image

    let sobelEdgeDetection (image: Image<Rgba32>) =
        image.Mutate(fun ctx -> ctx.DetectEdges() |> ignore)
        image

module Transformers =
    open Processors

    type Transformer = Image<Rgba32> -> Image<Rgba32>
    type TransformerDescriptor =
        | GrayScale of mode: GrayscaleMode * amount: float32
        | Resize of mode: ResamplingMode * width: int * height: int
        | Rotate of degrees: float32
        | Contrast of amount: float32
        | BinaryThreshold of mode: BinaryThresholdMode * threshold: float32
        | Blur of mode: BlurMode * parameter: BlurParameter
        | SobelEdgeDetection

    let applyTransformers image (transformers: Transformer list) =
        transformers |> List.fold (fun img transformer -> transformer img) image

    let createGrayScaleTransformer mode amount =
        grayscale mode amount

    let createResizeTransformer mode width height =
        resize mode width height

    let createRotateTransformer degrees =
        rotate degrees

    let createContrastTransformer amount =
        contrast amount

    let createBinaryThresholdTransformer mode threshold =
        binaryThreshold mode threshold

    let createBlurTransformer mode parameter =
        blur mode parameter

    let createSobelEdgeDetectionTransformer =
        sobelEdgeDetection