#r "nuget:SixLabors.ImageSharp"

open System.IO
open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats
open SixLabors.ImageSharp.Processing


let img2Hanjie (imgPath: string) (targetSize: int) luminosityThreshold contrast=
    use img = Image.Load<Rgba32>(imgPath)
    let (width, height) = (img.Width, img.Height)
    // if the image's width nad height are not equal, we can't use it
    match width = height with
    | false -> failwith "Image must be square"
    | true ->
        img.Mutate(fun ctx -> ctx.Contrast(contrast)
                                  .Grayscale()                                  
                                 .Resize(targetSize, targetSize) |> ignore)
        img.Save($"./{Path.GetFileNameWithoutExtension(imgPath)}_gray_contrast_resized.png")
        img.Mutate(fun ctx -> ctx.BinaryThreshold(luminosityThreshold) |> ignore)
        img.Save($"./{Path.GetFileNameWithoutExtension(imgPath)}_bw_resized.png")
        
        let rows = Array.init targetSize (fun _ -> Array.empty)
        let cols = Array.init targetSize (fun _ -> Array.empty)

        let black : Rgba32 = Color.Black
        for y in 0 .. targetSize - 1 do
            let mutable isGroup = false
            let mutable groupCount = 0
            let mutable pixelCount = 0
            let mutable groups = []
            for x in 0 .. targetSize - 1 do
                let isBlack = img[x, y] = black
                match isBlack, isGroup with
                | true, false -> isGroup <- true; groupCount <- groupCount + 1; pixelCount <- 1
                | true, true -> pixelCount <- pixelCount + 1
                | false, true -> groups <- groups @ [pixelCount]; isGroup <- false; pixelCount <- 0
                | false, false -> ()
            rows.[y] <- groups |> List.toArray
        
        for x in 0 .. targetSize - 1 do
            let mutable isGroup = false
            let mutable groupCount = 0
            let mutable pixelCount = 0
            let mutable groups = []
            for y in 0 .. targetSize - 1 do
                let isBlack = img[x, y] = black
                match isBlack, isGroup with
                | true, false -> isGroup <- true; groupCount <- groupCount + 1; pixelCount <- 1
                | true, true -> pixelCount <- pixelCount + 1
                | false, true -> groups <- groups @ [pixelCount]; isGroup <- false; pixelCount <- 0
                | false, false -> ()
            cols.[x] <- groups |> List.toArray
            
        (rows, cols)
        
// prettyPrint function prints the rows and columns hints of a hanjie in a form of grid
// e.g. having rows = [| [|1; 1|]; [|1|]; [|3|] |] and cols = [| [|1; 1|]; [|1|]; [|3|] |]
// the output will be:
// [ ] 1 _ _
// [ ] 1 1 3
// 1 1 * * *
// _ 1 * * *
// _ 3 * * *
let prettyPrint cols rows size=
    let printEmptySpace maxRowLength =
        let str = "[" + (String.replicate (maxRowLength - 1) " ")  + "] "
        printf $"%s{str}"
        
    let maxRowLength = rows |> Array.map Array.length |> Array.max
    let maxColLength = cols |> Array.map Array.length |> Array.max
    for i in maxColLength - 1 .. 0 do
        printEmptySpace maxRowLength
        for j in 0 .. size - 1 do
            let col = cols.[j]
            if i < col.Length then
                printf $"%d{col.[i]} " 
            else
                printf $"_ "
        printfn ""
    
    let stars = String.replicate size "* "
    for i in 0 .. size - 1 do
        for j in 0 .. maxRowLength - 1 do
            let row = rows.[i]
            if j < row.Length then
                printf $"%d{row.[j]} "
            else
                printf $"_ "
        printfn $"%s{stars}"

let imgPath = "C:\Users\KoliaV\Pictures\smile.png"

// current directory
let currentDir = Directory.GetCurrentDirectory()