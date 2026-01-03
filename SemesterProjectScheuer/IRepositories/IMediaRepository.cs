namespace SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;

public interface IMediaRepository
{
    bool CreateMedia(RegisterMedia newMedia);
    MediaElement GetMediaById(int mediaId);
    List<MediaElement> GetAllMedias();
    bool DeleteMediaById(int mediaId);
    MediaElement ChangeMedia(ChangeMedia updatedMedia);
}