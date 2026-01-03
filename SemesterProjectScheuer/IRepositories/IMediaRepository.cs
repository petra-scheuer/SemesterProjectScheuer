namespace SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;

public interface IMediaRepository
{
    bool CreateMedia(RegisterMedia newMedia);
    //bool UpdateMedia(RegisterMedia updatedMedia);
}