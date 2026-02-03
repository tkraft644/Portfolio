using Portfolio.Models;

namespace Portfolio.Services;

public interface IPortfolioContentService
{
    HomeIndexViewModel GetHomeIndexModel();
    AboutViewModel GetAboutModel();
    ProjectsViewModel GetProjectsModel();
    ContactViewModel GetContactInfo();
}
