using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SecuGen.FDxSDKPro.Windows;
using System.Drawing;
using System.Drawing.Imaging;

namespace FingerPrint
{
    public class BiometricService
    {
        private SGFingerPrintManager m_FPM;
        private readonly IWebHostEnvironment _hostEnvironment;
        private bool m_LedOn = false;
        private Int32 m_ImageWidth;
        private Int32 m_ImageHeight;
        private Byte[] m_RegMin1 = null;
        private Byte[] m_RegMin2;
        private Byte[] m_VrfMin;
        SGFPMDeviceName device_name;
        Int32 device_id;
        Int32 iError;
        Int32 elap_time;
        Byte[] fp_image;

        private SGFPMDeviceList[] m_DevList; // Used for EnumerateDevice
       
        public BiometricService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            m_FPM = new SGFingerPrintManager();
            device_name = SGFPMDeviceName.DEV_AUTO;
            device_id = (Int32)(SGFPMPortAddr.USB_AUTO_DETECT);
        }

        public ResultModel Scan()
        {
            try
            {
                elap_time = Environment.TickCount;
                fp_image = new Byte[260 * 300];
                iError = m_FPM.Init(device_name);
                iError = m_FPM.OpenDevice(device_id);
                iError = m_FPM.GetImage(fp_image);

                if (iError == (Int32)SGFPMError.ERROR_NONE)
                {
                    elap_time = Environment.TickCount - elap_time;

                    var img = Convert.ToBase64String(fp_image);

                    string base64String = Convert.ToBase64String(fp_image, 0, fp_image.Length);
                    var ImageUrl = "data:image/png;base64," + base64String;
                    return new ResultModel
                    {
                        Data = this.DrawImage(fp_image, new object()),
                        Success = true,
                        Message = ""
                    };

                }
                else
                {

                    return new ResultModel
                    {
                        Message = this.DisplayError("GetImage()", iError),
                        Success = false,
                        Data = null
                    };

                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Message = ex.Message,Success=false };
            }
        }
        public  string DrawImage(Byte[] imgData, object picBox)
        {
            try
            {
                int colorval;
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = "yymmssfff.png";
                Bitmap bmp = new Bitmap(260, 300);
                picBox = (Image)bmp;

                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        colorval = (int)imgData[(j * 260) + i];
                        bmp.SetPixel(i, j, Color.FromArgb(colorval, colorval, colorval));
                    }
                }
                Image image = (Image)bmp;

                image.Save(fileName, ImageFormat.Png);
                image.Dispose();
                var base64 = ConvertImageToBase64String(Image.FromFile(fileName));
                File.Delete(fileName);
                return base64;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public static string ConvertImageToBase64String(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                image.Dispose();
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public string DisplayError(string funcName, int iError)
        {
            string text = "";

            switch (iError)
            {
                case 0:                             //SGFDX_ERROR_NONE				= 0,
                    text = "Error none";
                    break;

                case 1:                             //SGFDX_ERROR_CREATION_FAILED	= 1,
                    text = "Can not create object";
                    break;

                case 2:                             //   SGFDX_ERROR_FUNCTION_FAILED	= 2,
                    text = "Function Failed";
                    break;

                case 3:                             //   SGFDX_ERROR_INVALID_PARAM	= 3,
                    text = "Invalid Parameter";
                    break;

                case 4:                          //   SGFDX_ERROR_NOT_USED			= 4,
                    text = "Not used function";
                    break;

                case 5:                                //SGFDX_ERROR_DLLLOAD_FAILED	= 5,
                    text = "Can not create object";
                    break;

                case 6:                                //SGFDX_ERROR_DLLLOAD_FAILED_DRV	= 6,
                    text = "Can not load device driver";
                    break;
                case 7:                                //SGFDX_ERROR_DLLLOAD_FAILED_ALGO = 7,
                    text = "Can not load sgfpamx.dll";
                    break;

                case 51:                //SGFDX_ERROR_SYSLOAD_FAILED	   = 51,	// system file load fail
                    text = "Can not load driver kernel file";
                    break;

                case 52:                //SGFDX_ERROR_INITIALIZE_FAILED  = 52,   // chip initialize fail
                    text = "Failed to initialize the device";
                    break;

                case 53:                //SGFDX_ERROR_LINE_DROPPED		   = 53,   // image data drop
                    text = "Data transmission is not good";
                    break;

                case 54:                //SGFDX_ERROR_TIME_OUT			   = 54,   // getliveimage timeout error
                    text = "Time out";
                    break;

                case 55:                //SGFDX_ERROR_DEVICE_NOT_FOUND	= 55,   // device not found
                    text = "Device not found";
                    break;

                case 56:                //SGFDX_ERROR_DRVLOAD_FAILED	   = 56,   // dll file load fail
                    text = "Can not load driver file";
                    break;

                case 57:                //SGFDX_ERROR_WRONG_IMAGE		   = 57,   // wrong image
                    text = "Please Re-Input Image";
                    break;

                case 58:                //SGFDX_ERROR_LACK_OF_BANDWIDTH  = 58,   // USB Bandwith Lack Error
                    text = "Lack of USB Bandwith";
                    break;

                case 59:                //SGFDX_ERROR_DEV_ALREADY_OPEN	= 59,   // Device Exclusive access Error
                    text = "Device is already opened";
                    break;

                case 60:                //SGFDX_ERROR_GETSN_FAILED		   = 60,   // Fail to get Device Serial Number
                    text = "Device serial number error";
                    break;

                case 61:                //SGFDX_ERROR_UNSUPPORTED_DEV		   = 61,   // Unsupported device
                    text = "Unsupported device";
                    break;

                // Extract & Verification error
                case 101:                //SGFDX_ERROR_FEAT_NUMBER		= 101, // utoo small number of minutiae
                    text = "The number of minutiae is too small";
                    break;

                case 102:                //SGFDX_ERROR_INVALID_TEMPLATE_TYPE		= 102, // wrong template type
                    text = "Template is invalid";
                    break;

                case 103:                //SGFDX_ERROR_INVALID_TEMPLATE1		= 103, // wrong template type
                    text = "1st template is invalid";
                    break;

                case 104:                //SGFDX_ERROR_INVALID_TEMPLATE2		= 104, // vwrong template type
                    text = "2nd template is invalid";
                    break;

                case 105:                //SGFDX_ERROR_EXTRACT_FAIL		= 105, // extraction fail
                    text = "Minutiae extraction failed";
                    break;

                case 106:                //SGFDX_ERROR_MATCH_FAIL		= 106, // matching  fail
                    text = "Matching failed";
                    break;

            }

            //text = funcName + " Error # " + iError + " :" + text;
            text =  text;
            return text;
        }
    }
}
