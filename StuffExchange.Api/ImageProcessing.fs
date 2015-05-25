module StuffExchange.Api.ImageProcessing

open System.Drawing

let saveThumbnail (filename: string) =
    let maxDimensions = 100

    let makeThumbnail (image: Bitmap) =
        let maxPixels = 100.0
        let widthAsDouble = System.Convert.ToDouble(image.Width)
        let heightAsDouble = System.Convert.ToDouble(image.Height)
        let scaling = 
            if image.Width > image.Height
            then maxPixels / widthAsDouble
            else maxPixels / heightAsDouble
        let toInts (x: float, y: float) =
            System.Convert.ToInt32(x), System.Convert.ToInt32(y)

        let size = widthAsDouble * scaling, heightAsDouble * scaling
        let newWidth, newHight = toInts size
        new Bitmap(image.GetThumbnailImage(newWidth, newHight, null, System.IntPtr.Zero))

    let image = new Bitmap(filename)
    let thumbnail = 
        if image.Width > maxDimensions || image.Height > maxDimensions
        then makeThumbnail image
        else image

    let thumbnailPath = filename.Split('.').[0] + "_thumb.jpg"
    thumbnail.Save(thumbnailPath)
    thumbnailPath

