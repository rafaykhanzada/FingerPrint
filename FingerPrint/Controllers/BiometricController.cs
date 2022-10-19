using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FingerPrint.Controllers
{
    [Route("api/biometric")]
    [ApiController]
    public class BiometricController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BiometricController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/<BiometricController>
        [HttpGet]
        public object Get()
        {
            BiometricService biometricService = new BiometricService(_webHostEnvironment);
            var data = biometricService.Scan();
            return data;
        }
        [HttpPost]
        public object Get([FromBody]string base64Img)
        {
            BiometricService biometricService = new BiometricService(_webHostEnvironment);
            var data = biometricService.Verifition(base64Img);
            return data;
        }

      
    }
}
