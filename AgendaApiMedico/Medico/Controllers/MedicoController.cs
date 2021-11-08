using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
//using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.IO;

namespace AgendaApiMedico.Medico.Controllers
{
    [Route("medico")]
    [ApiController]
    public class MedicoController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public MedicoController (IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost("imagen/upload")]
        public ActionResult UploadFile([FromForm] List<IFormFile> files, [FromForm] string rut)
        {
            if (files == null || (files != null && files.Count > 1))
            {
                return BadRequest("Solo puede subir una imagen");
            }

            if (rut == null || (rut != null && rut == ""))
            {
                return BadRequest("El rut es requerido");
            }

            rut = rut.Trim();

            var filePath = Path.GetTempPath();
            using (var stream = System.IO.File.Create(filePath + "/" + files[0].FileName))
            {
                files[0].CopyTo(stream);
            }


            var client = new RestClient { BaseUrl = new Uri(Configuration.GetValue<string>("MicroserviceCloudinary")) };
            
            var request = new RestRequest { Resource = "/api/v1/files/upload" };
            //request.AlwaysMultipartFormData = true;
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("files", System.IO.File.ReadAllBytes(filePath + "/" + files[0].FileName), files[0].FileName);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("accept", "application/json");
            request.AddParameter("nameImage", rut, ParameterType.RequestBody);


            var response = client.Execute(request, Method.POST);
            System.IO.File.Delete(filePath + "/" + files[0].FileName);
            string data = response.Content;
            Dictionary<string, object> responseTransform = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

            var responseForSave = new
            {
                ptdRut = Comunes.PrepararRut(rut),
                ancho = responseTransform.GetValueOrDefault("width"),
                alto = responseTransform.GetValueOrDefault("height"),
                nombreImagen = rut,
                formato = responseTransform.GetValueOrDefault("format"),
                fechaCreacion = responseTransform.GetValueOrDefault("createdAt")
            };

            Console.WriteLine(responseTransform.GetValueOrDefault("error"));

            return Ok(responseForSave);
        }


        [HttpGet("imagen/test")]
        public ActionResult TestRoute()
        {
            try
            {
                var client = new RestClient { BaseUrl = new Uri(Configuration.GetValue<string>("MicroserviceCloudinary")) };
                var request = new RestRequest { Resource = "/api/v1/files/test" };

                var response = client.Execute(request, Method.GET);

                Console.WriteLine("response.Content");
                Console.WriteLine(response.ErrorMessage);

                if (response.ErrorMessage != null)
                {
                    return BadRequest(response.ErrorMessage);
                }

                return Ok(response.Content);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
        }
    }
}
