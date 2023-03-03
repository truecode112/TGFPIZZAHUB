import base64
import win32print
from io import BytesIO
import win32ui
from PIL import Image, ImageWin
import cgi,os,sys,win32print,pywintypes

print("Access-Control-Allow-Origin: *")
form = cgi.FieldStorage()

try:
    base64_data = form.getvalue('d')
    # Base64-encoded image data
    image_base64 = base64_data
    # Decode the base64-encoded image into bytes
    image_data = base64.b64decode(image_base64)
    with open('orderimg.png', 'wb') as fh:
        fh.write(image_data)
    HORZRES = 8
    VERTRES = 10

    PHYSICALWIDTH = 499
    PHYSICALHEIGHT = 709

    PHYSICALOFFSETX = 20
    PHYSICALOFFSETY = 20

    file_name = "orderimg.png"

    # Open a handle to the default printer
    printer_name = win32print.GetDefaultPrinter()
    hDC = win32ui.CreateDC ()
    hDC.CreatePrinterDC (printer_name)
    printable_area = hDC.GetDeviceCaps (HORZRES), hDC.GetDeviceCaps (VERTRES)
    printer_size = hDC.GetDeviceCaps (PHYSICALWIDTH), hDC.GetDeviceCaps (PHYSICALHEIGHT)
    printer_margins = hDC.GetDeviceCaps (PHYSICALOFFSETX), hDC.GetDeviceCaps (PHYSICALOFFSETY)
    bmp = Image.open (file_name)
    #if bmp.size[0] > bmp.size[1]:
      #bmp = bmp.rotate (90)
    pa_list = list(printable_area)
    pa_list[0] = pa_list[0] - 300
    pa_list[1] = pa_list[1] - 300
    printable_area = tuple(pa_list)
    print(printable_area)
    print(bmp.size)
    ratios = [1.0 * printable_area[0] / bmp.size[0], 1.0 * printable_area[1] / bmp.size[1]]
    scale = min (ratios)
    print(scale)
    hDC.StartDoc (file_name)
    hDC.StartPage ()

    dib = ImageWin.Dib (bmp)
    scaled_width, scaled_height = [int (scale * i) for i in bmp.size]
    print(scaled_width, scaled_height)
    #x1 = int ((printer_size[0] - scaled_width) / 2)
    #y1 = int ((printer_size[1] - scaled_height) / 2)
    x1 = 150
    y1 = 150
    x2 = x1 + scaled_width
    y2 = y1 + scaled_height
    dib.draw (hDC.GetHandleOutput (), (x1, y1, x2, y2))

    hDC.EndPage ()
    hDC.EndDoc ()
    hDC.DeleteDC ()
    os.remove("orderimg.png")
    print("OK")
except Exception as e:
    print(e)
