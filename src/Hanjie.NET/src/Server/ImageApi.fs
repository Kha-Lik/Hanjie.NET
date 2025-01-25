module ImageApi

open System
open Shared
open Shared.ImageProcessing.Transformers
open SixLabors.ImageSharp
open SixLabors.ImageSharp.Formats.Png
open SixLabors.ImageSharp.PixelFormats

let private transformerSelector descriptor =
    match descriptor with
    | GrayScale (mode, amount) -> createGrayScaleTransformer mode amount
    | Resize (mode, width, height) -> createResizeTransformer mode width height
    | Rotate degrees -> createRotateTransformer degrees
    | Contrast amount -> createContrastTransformer amount
    | BinaryThreshold (mode, threshold) -> createBinaryThresholdTransformer mode threshold
    | Blur (mode, parameter) -> createBlurTransformer mode parameter
    | SobelEdgeDetection -> createSobelEdgeDetectionTransformer

let private loadImage image64 =
    let bytes = Convert.FromBase64String image64
    let image = Image.Load<Rgba32>(bytes)
    image

let private processImage request =
    let image = loadImage request.Image
    request.Transformers
    |> List.map transformerSelector
    |> applyTransformers image
    |> (_.ToBase64String(PngFormat.Instance))

let imageApi ctx = {
    getProcessedImage = fun request -> async {
        return { ProcessedImage = processImage request }
        }
    }