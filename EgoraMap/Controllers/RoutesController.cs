using EgoraMap.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EgoraMap.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RoutesController : ApiController
    {
        DbEgoraContext db;
        List<ViewRoute> vrList = new List<ViewRoute>();
        public RoutesController()
        {
            try
            {
                db = new DbEgoraContext();
            }
            catch (Exception e) { }

            if (db.Routes.Any())
            {
                foreach (Route route in db.Routes)
                {
                    ViewRoute vr = new ViewRoute();
                    vr.Id = route.Id;
                    vr.Name = route.Name;
                    vr.Description = route.Description;
                    vr.ImageMap = route.RouteImage;
                    vr.PhotoPath = GetPhotos(route.Id);
                    vrList.Add(vr);
                }
            }
        }

        // GET: api/Routes
        public IHttpActionResult Get()
        {
            if (vrList.LongCount() > 0)
            {
                return Json(vrList);
            }
            else
                return Json("[{}]");
        }

        // GET: api/Routes/5
        public IHttpActionResult Get(int Id)
        {
            Route route = db.Routes.FirstOrDefault(x => x.Id == Id);
            if (route == null)
                return NotFound();
            ViewRoute vr = new ViewRoute();
            vr.Id = route.Id;
            vr.Name = route.Name;
            vr.Description = route.Description;
            vr.ImageMap = route.RouteImage;
            vr.PhotoPath = GetPhotos(route.Id);
            return Json(vr);
        }

        // POST: api/Routes
        public IHttpActionResult Post()
        {
            string urlimg, urlkml;
            HttpPostedFile uploadImage = null;
            HttpPostedFile uploadKML = null;
            IEnumerable<HttpPostedFile> ffile = null;
            HttpFileCollection files = HttpContext.Current.Request.Files;
            uploadImage = files.Get("uploadImage");
            uploadKML = files.Get("uploadKML");
            ffile = files.GetMultiple("ffile");
            int nfiles = files.Count;
            
            string strName = HttpContext.Current.Request.Form.Get("Name");
            string strDescription = HttpContext.Current.Request.Form.Get("Description");
            if (strName == "")
            {
                return Content(HttpStatusCode.BadRequest, "Не указано название карты");
            }
            if (uploadImage == null || uploadKML == null)
            {
                return Content(HttpStatusCode.BadRequest, "Нет файла изображения маршрута или файла KML");
            }
            // получаем имя файла из переданных параметров
            string fileNameImage = System.IO.Path.GetFileName(uploadImage.FileName);
            string fileNameKML = System.IO.Path.GetFileName(uploadKML.FileName);
            //формируем имя файла для сохранения в БД
            urlimg = String.Format("{0}_{1}{2}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Guid.NewGuid(), Path.GetExtension(fileNameImage));
            urlkml = String.Format("{0}_{1}{2}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Guid.NewGuid(), Path.GetExtension(fileNameKML));
            List<Photo> photos = new List<Photo>();
            if (ffile != null)
            {
                foreach (HttpPostedFile photoFile in ffile)
                {
                    string photoNameImage = System.IO.Path.GetFileName(photoFile.FileName);
                    string urlphoto = String.Format("{0}_{1}{2}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Guid.NewGuid(), Path.GetExtension(photoNameImage));
                    Photo photo = new Photo();
                    photo.PhotoName = urlphoto;
                    photo.Photocreated = DateTime.Now;
                    photo.Description = photoNameImage;
                    photos.Add(photo);
                    photoFile.SaveAs(HttpContext.Current.Server.MapPath("~/Contents/Files/Photo/" + urlphoto));
                }
            }
            Route route = new Route();
            try
            {
                route.Name = strName;
                route.Description = strDescription;
                route.RouteImage = urlimg;
                route.RouteKML = urlkml;
                db.Routes.Add(route);
                int saved = db.SaveChanges();
                if (saved > 0)
                {

                    uploadImage.SaveAs(HttpContext.Current.Server.MapPath("~/Contents/Files/Img/" + urlimg));
                    uploadKML.SaveAs(HttpContext.Current.Server.MapPath("~/Contents/Files/Kml/" + urlkml));
                    foreach (var p in photos)
                    {
                        p.Route = route;
                    }
                }
                db.Photos.AddRange(photos);
                saved = db.SaveChanges();
                if (saved < 1)
                {
                    foreach (var photo in photos)
                    {
                        string filephoto = photo.PhotoName;
                        try
                        {
                            System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/Contents/Files/Photo/" + filephoto));
                        }
                        catch (Exception e)
                        {
                            return Content(HttpStatusCode.BadRequest, "Ошибка удаления файла");
                        }
                    }
                }

                return Json("Маршрут успешно добавлен.");
            }
            catch (Exception e) { return Content(HttpStatusCode.BadRequest, "Не удалось добавить запись"); }


        }

        // DELETE: api/Routes/5
        public IHttpActionResult Delete(int Id)
        {
            Route route = db.Routes.FirstOrDefault(x => x.Id == Id);
            if (route == null) return Content(HttpStatusCode.BadRequest, "Запись не найдена");
            string fileimg = route.RouteImage;
            string filekml = route.RouteKML;
            int routeId = route.Id;
            using (DbEgoraContext db2 = new DbEgoraContext())
            {
                IEnumerable<Photo> photos = db2.Photos.Where(x => x.RouteId == routeId);

                foreach (var photo in photos)
                {
                    string filephoto = photo.PhotoName;
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/Contents/Files/Photo/" + filephoto));
                    }
                    catch (Exception e)
                    {
                        return Content(HttpStatusCode.BadRequest, "Ошибка удаления файла");
                    }
                }
                if (photos != null)
                {
                    try
                    {
                        db2.Photos.RemoveRange(photos);
                        db2.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Content(HttpStatusCode.BadRequest,"Ошибка удаления в таблице фотографий. "+ e.Message);
                    }
                }

            }
            db.Routes.Remove(route);
            int saved = db.SaveChanges();
            if (saved > 0)
            {
                try
                {
                    System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/Contents/Files/Img/" + fileimg));
                    System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/Contents/Files/Kml/" + filekml));
                    return Ok("Маршрут успешно удален");
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
            }
            else return Content(HttpStatusCode.BadRequest, "Ошибка удаления записи в таблице Route");
        }

        private IEnumerable<string> GetPhotos(int id)
        {
            List<string> photopath = new List<string>();
            List<Photo> photos;

            using (var db2 = new DbEgoraContext())
            {
                photos = db2.Photos.Where(x => x.RouteId == id).ToList();
            }
            if (!photos.Any())
                return null;
            foreach (Photo ph in photos)
            {
                photopath.Add(ph.PhotoName);
            }
            return photopath;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
