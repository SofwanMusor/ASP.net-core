using Microsoft.AspNetCore.Mvc;
namespace FileInterfaces
{
    public interface IFileService
    {
        public Task<ActionResult>  UploadFile (IFormFile file);

        public ActionResult GetFileName (string directoryName);
    }
}